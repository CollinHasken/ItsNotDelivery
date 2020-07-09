using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CashChangedEvent : UnityEvent<GameObject> { }

public class Deliverer : MonoBehaviour
{
    public decimal Cash { get; private set; }
    public int PizzasDelivered { get; private set; }

    public CashChangedEvent cashChangedEvent
    {
        get
        {
            if (m_CashChangedEvent == null)
                m_CashChangedEvent = new CashChangedEvent();
            return m_CashChangedEvent;
        }
    }

    private CashChangedEvent m_CashChangedEvent;
    private GameObject DeliveryArrow;
    private Vector3 DeliveryLocation;

    // Start is called before the first frame update
    void Start()
    {
        Cash = 0m;
    }

    // Update is called once per frame
    void Update()
    {
        if(DeliveryArrow == null)
        {
            return;                
        }

       DeliveryArrow.transform.LookAt(DeliveryLocation);
        DeliveryArrow.transform.Rotate(90, 0, 0);
    }

    public void NewDeliveryLocation(Vector3 location)
    {
        if(DeliveryArrow == null)
        {
            Player player = GetComponent<Player>();
            DeliveryArrow = Instantiate(DeliveryManager.Instance.m_DeliveryLocationArrows[player.playerIndex], transform);
            DeliveryArrow.transform.localPosition = new Vector3(0, 3.3f, 0);
        }
        DeliveryLocation = location;
    }

    public void OnPizzaDelivered(decimal cash)
    {
        PizzasDelivered++;

        Cash += cash;
        Debug.Log("Pizza delivered for $" + cash.ToString() + ". Total: $" + Cash.ToString());

        Player player = GetComponent<Player>();
        player.OnPizzaDelivered();

        cashChangedEvent.Invoke(gameObject);
    }
}
