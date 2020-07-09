using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[System.Serializable]
public class PizzaDeliveredEvent : UnityEvent<DeliveryLocation, int, int>{}

// Keep track of time remaining and listen for players to deliver here
public class DeliveryLocation : MonoBehaviour
{
    private float m_ActiveDeliveryTime;
    private float m_ActiveDeliveryTimeMax;
    private int m_ActivePlayerIndex;
    private bool m_DeliverExpired;

    private GameObject m_Indicator;

    public float GetMaxDeliverTime() { return m_ActiveDeliveryTimeMax; }
    public float GetCurrentDeliverTime() { return m_ActiveDeliveryTime; }

    public PizzaDeliveredEvent pizzaDeliveredEvent
    {
        get
        {
            if (m_PizzaDeliveredEvent == null)
                m_PizzaDeliveredEvent = new PizzaDeliveredEvent();
            return m_PizzaDeliveredEvent;
        }
    }

    private PizzaDeliveredEvent m_PizzaDeliveredEvent;

    // Start is called before the first frame update
    void Start()
    {
        m_ActivePlayerIndex = Constants.InvalidPlayerIndex;
        m_ActiveDeliveryTime = 0.0f;
        m_DeliverExpired = false;
    }

    // Update is called once per frame
    void Update()
    {
        // No active delivery
        if(m_ActivePlayerIndex < 0)
        {
            return;
        }

        if (!m_DeliverExpired)
        {
            m_ActiveDeliveryTime -= Time.deltaTime;
            if (m_ActiveDeliveryTime <= 0.0f)
            {
                OnDeliveryExpired();
            }
        }
    }

    public void NewDelivery(int playerIndex, float deliveryTime)
    {
        m_ActivePlayerIndex = playerIndex;
        m_ActiveDeliveryTimeMax = m_ActiveDeliveryTime = deliveryTime;
        m_DeliverExpired = false;

        Vector3 spawnLocation = transform.position + DeliveryManager.Instance.m_DeliverLocationOffset;
        GameObject indicator = DeliveryManager.Instance.m_DeliveryLocationIndicators.Length > playerIndex ? DeliveryManager.Instance.m_DeliveryLocationIndicators[playerIndex] : DeliveryManager.Instance.m_DeliveryLocationIndicators[0];

        m_Indicator = Instantiate(indicator, spawnLocation, Quaternion.identity);

        Debug.Log("New delivery with " + m_ActiveDeliveryTimeMax.ToString() + " seconds for house " + transform.parent.name);
    }

    void OnDeliveryExpired()
    {
        Debug.Log("Deliver expired for player " + m_ActivePlayerIndex.ToString(), this);
        m_DeliverExpired = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (m_ActivePlayerIndex == Constants.InvalidPlayerIndex)
        {
            return;
        }

        GameObject otherRoot = other.transform.root.gameObject;

        PlayerInput input = otherRoot.GetComponent<PlayerInput>();
        if(input == null)
        {
            Debug.LogErrorFormat("Object with player collision doesn't have PlayerInput from %s", otherRoot.gameObject.name);
            return;
        }

        // If the time hasn't expired, check if this is the active player
        if(!m_DeliverExpired && input.playerIndex != m_ActivePlayerIndex)
        {
            Debug.Log("Player " + input.playerIndex.ToString() + " entered but is assigned to player " + m_ActivePlayerIndex.ToString(), this);
            return;
        }

        Deliverer playerDeliverer = otherRoot.GetComponent<Deliverer>();
        if (playerDeliverer == null)
        {
            Debug.LogErrorFormat("Object with player collision doesn't have Deliverer from %s", otherRoot.gameObject.name);
            return;
        }

        float cash = DeliveryManager.Instance.m_BasePrice;
        if(m_ActivePlayerIndex == input.playerIndex)
        {
            cash += (m_ActiveDeliveryTime / m_ActiveDeliveryTimeMax) * DeliveryManager.Instance.m_MaxTip;
            Debug.Log("Player " + m_ActivePlayerIndex.ToString() + " delivered pizza with " + m_ActiveDeliveryTime.ToString() + " time remaining");
        } else
        {
            // Someone stole it after time expired
            cash += 1;
            Debug.Log("Player " + input.playerIndex.ToString() + " stole delivery from player " + m_ActivePlayerIndex.ToString() + " with " + m_ActiveDeliveryTime.ToString() + " time remaining");
        }

        decimal cash_dec = decimal.Round((decimal)cash, 2);

        playerDeliverer.OnPizzaDelivered(cash_dec);

        // Reset active player
        int activePlayer = m_ActivePlayerIndex;
        m_ActivePlayerIndex = Constants.InvalidPlayerIndex;

        // Delete indicator
        Destroy(m_Indicator);

        pizzaDeliveredEvent.Invoke(this, activePlayer, input.playerIndex);
    }
}
