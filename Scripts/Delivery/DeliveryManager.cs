using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


// Keep track of current delivery locations and be able to pick new ones
public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance;

    public float m_DeliveryTimeMin = 30.0f;
    public float m_DeliveryTimeMax = 60.0f;

    public float m_BasePrice = 10.0f;
    public float m_MaxTip = 5.0f;

    public Vector3 m_DeliverLocationOffset;
    public GameObject[] m_DeliveryLocationIndicators;
    public GameObject[] m_DeliveryLocationArrows;

    private GameObject[] m_Players;
    private List<DeliveryLocation> m_DeliveryLocations;
    private DeliveryLocation[] m_CurrentLocations;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        if(m_DeliveryLocationIndicators.Length == 0)
        {
            Debug.LogError("Deliver Location Inidicator not set!");
            return;
        }

        m_CurrentLocations = new DeliveryLocation[4];

        m_DeliveryLocations = new List<DeliveryLocation>(Object.FindObjectsOfType<DeliveryLocation>());
        Shuffle(m_DeliveryLocations);
    }

    public void Init(GameObject[] players)
    {
        m_Players = players;

        Debug.Log("Giving players initial delivery locations");
        for (int playerIndex = 0; playerIndex < m_Players.Length; ++playerIndex)
        {
            StartNextDeliveryLocation(playerIndex);
        }
    }

    public void StartNextDeliveryLocation(int player)
    {
        Debug.Log("Starting new deliver for player " + player.ToString());

        if(m_DeliveryLocations.Count == 0)
        {
            Debug.LogError("No more delivery locations!");
            return;
        }

        // Get next one in line
        DeliveryLocation location = m_DeliveryLocations[0];
        m_DeliveryLocations.RemoveAt(0);

        // Store
        m_CurrentLocations[player] = location;

        location.NewDelivery(player, Mathf.Floor(Random.Range(m_DeliveryTimeMin, m_DeliveryTimeMax)));
        m_Players[player].GetComponent<Deliverer>().NewDeliveryLocation(location.transform.position);

        // Listen for compeltion
        location.pizzaDeliveredEvent.AddListener(LocationDelivered);
    }

    public DeliveryLocation GetCurrentDeliveryLocation(int playerIndex)
    {
        return m_CurrentLocations[playerIndex];
    }

    public void LocationDelivered(DeliveryLocation location, int originalPlayer, int deliveredPlayer)
    {
        m_DeliveryLocations.Add(m_CurrentLocations[originalPlayer]);
        m_CurrentLocations[originalPlayer].pizzaDeliveredEvent.RemoveListener(LocationDelivered);
        m_CurrentLocations[originalPlayer] = null;

        if(originalPlayer != deliveredPlayer)
        {
            m_DeliveryLocations.Add(m_CurrentLocations[deliveredPlayer]);
            m_CurrentLocations[deliveredPlayer].pizzaDeliveredEvent.RemoveListener(LocationDelivered);
            m_CurrentLocations[deliveredPlayer] = null;
            StartNextDeliveryLocation(deliveredPlayer);
        }

        StartNextDeliveryLocation(originalPlayer);
    }

    private void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n+1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
