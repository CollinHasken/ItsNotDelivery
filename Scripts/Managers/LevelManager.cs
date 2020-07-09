using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public DeliveryManager m_DeliveryManager;
    public float m_TimeToPlaySeconds = 120;
    public float countDownTimer = 4;
    public TextMeshProUGUI countdownTimerText;

    public TextMeshProUGUI levelTimerText;
    public List<TextMeshProUGUI> playerCashText = new List<TextMeshProUGUI>();
    public List<Image> playerClocks = new List<Image>();
    public List<GameObject> playerAbilitieUIs = new List<GameObject>();
    public GameObject[] m_PlayerHeadquarters;
    public GameObject[] m_RepairShops;
    public AudioClip m_BGSong;

    public UnityEvent m_LevelTimedOut;
    private List<GameObject> m_Players = new List<GameObject>();

    private bool m_SongPlaying = false;
    private bool m_Playing;
    private float m_Timer;

    public GameObject[] GetPlayers()
    {
        return m_Players.ToArray();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(m_DeliveryManager == null)
        {
            m_DeliveryManager = FindObjectOfType<DeliveryManager>();
            if(m_DeliveryManager == null)
            {
                Debug.LogError("No Deliver Manager was found!");
            }
        }

        if (m_LevelTimedOut == null)
        {
            m_LevelTimedOut = new UnityEvent();
        }

        m_Playing = false;
    }

    public void Init(List<GameObject> players, bool playing)
    {
        Debug.Log("Initializing Level Manager");
        m_Players = new List<GameObject>(players);
        m_Timer = m_TimeToPlaySeconds;
        m_Playing = playing;

        if(GameManager.inGame == true)
        {
            Time.timeScale = 0;
            countdownTimerText.gameObject.SetActive(true);
        }

        int playerIndex = 0;
        foreach(GameObject player in m_Players)
        {
            InitPlayer(player, playerIndex);
            playerIndex++;
        }

        if (m_DeliveryManager != null)
        {
            m_DeliveryManager.Init(players.ToArray());
        }

        if(m_BGSong != null)
        {
            Camera camera = Camera.main;
            if(camera == null)
            {
                m_SongPlaying = false;
                return;
            }
            AudioSource cameraAudio = camera.GetComponent<AudioSource>();
            cameraAudio.Stop();
            cameraAudio.clip = m_BGSong;
            cameraAudio.Play();
            m_SongPlaying = true;
        }
    }

    public void PlayerJoined(GameObject newPlayer)
    {
        int playerIndex = m_Players.Count;
        m_Players.Add(newPlayer);
        InitPlayer(newPlayer, playerIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_SongPlaying && m_BGSong != null)
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                m_SongPlaying = false;
                return;
            }
            AudioSource cameraAudio = camera.GetComponent<AudioSource>();
            cameraAudio.Stop();
            cameraAudio.clip = m_BGSong;
            cameraAudio.Play();
            m_SongPlaying = true;
        }

        if (!m_Playing)
        {
            return;
        }

        if(countDownTimer > 0)
        {
            countDownTimer -= Time.unscaledDeltaTime;
            int countdownSeconds = (int)(countDownTimer % 60);
            countdownSeconds -= 1;

            string cdText = countdownSeconds.ToString();
            if (countdownSeconds <= 0)
            {
                cdText = "Start!";
            }

            countdownTimerText.SetText(cdText);

            if(countDownTimer <= 0)
            {
                Time.timeScale = 1;
                countdownTimerText.gameObject.SetActive(false);
            }
        }
        else
        {
            m_Timer -= Time.deltaTime;
        }
        

        int minutesRemaining = (int)(m_Timer / 60);
        int secondsRemaining = (int)(m_Timer % 60);
        levelTimerText.SetText((minutesRemaining < 10 ? "0": "") + minutesRemaining + ":" + (secondsRemaining < 10 ? "0" : "") + secondsRemaining);

        if (m_Timer <= 0)
        {
            m_Playing = false;
            m_LevelTimedOut.Invoke();
        }

        int idx = 0;
        foreach(GameObject player in m_Players)
        {
            DeliveryLocation location = m_DeliveryManager.GetCurrentDeliveryLocation(idx);
            float timePercent = location.GetCurrentDeliverTime() / location.GetMaxDeliverTime();

            playerClocks[idx].fillAmount = timePercent;
            idx += 1;

            if (playerAbilitieUIs[idx] != null)
            {
                bool playerHasAbility = player.GetComponent<BikePowerupManager>().hasPowerup;
                playerAbilitieUIs[idx].SetActive(playerHasAbility);
            }
        }
    }

    private void OnDestroy()
    {
        if (m_BGSong != null)
        {
            GameObject camera = Camera.main.gameObject;
            if (camera != null)
            {
                AudioSource cameraAudio = camera.GetComponent<AudioSource>();
                if (cameraAudio)
                {
                    cameraAudio.Stop();
                }
            }
        }
    }

    void OnPlayerCashUpdated(GameObject player)
    {
        Deliverer deliverer = player.GetComponent<Deliverer>();
        if(deliverer == null)
        {
            Debug.LogError("Player that updated cash doesn't have deliverer");
            return;
        }

        Player playerComp = player.GetComponent<Player>();

        decimal cashValue = deliverer.Cash;
        string cashString = "$" + cashValue;
        playerCashText[playerComp.playerIndex].SetText(cashString);
    }

    void OnPlayerCrashed(GameObject player)
    {
        Player playerComp = player.GetComponent<Player>();
        // Fade out camera
        MenuManager.Instance.SetPlayerFade(playerComp.playerIndex, 1.5f, MenuManager.FadeState.FadeOut, OnFadedOut);
    }

    void OnFadedOut(int playerIndex)
    {
        MenuManager.Instance.ClearPlayerFadeListeners(playerIndex);

        // Teleport back to start or repair shop
        GameObject player = m_Players[playerIndex];
        float shortestDist = (player.transform.position - m_PlayerHeadquarters[playerIndex].transform.position).sqrMagnitude;
        GameObject closestSpawn = m_PlayerHeadquarters[playerIndex];
        foreach (GameObject shop in m_RepairShops)
        {
            float newDist = (player.transform.position - shop.transform.position).sqrMagnitude;
            if (newDist < shortestDist)
            {
                closestSpawn = shop;
            }
        }

        player.GetComponent<Player>().Respawn(closestSpawn.transform);

        StartCoroutine("FadeIn", playerIndex);
    }

    IEnumerator FadeIn(int playerIndex)
    {
        yield return new WaitForSeconds(1f);

        MenuManager.Instance.SetPlayerFade(playerIndex, 1.5f, MenuManager.FadeState.FadeIn, OnFadedIn);

        yield return null;
    }

    void OnFadedIn(int playerIndex)
    {
        MenuManager.Instance.ClearPlayerFadeListeners(playerIndex);
    }

    private void InitPlayer(GameObject player, int playerIndex)
    {
        // Teleport to each player's starting location
        player.transform.position = m_PlayerHeadquarters[playerIndex].transform.position;
        player.transform.rotation = m_PlayerHeadquarters[playerIndex].transform.rotation;

        Deliverer deliverer = player.GetComponent<Deliverer>();
        if (deliverer == null)
        {
            Debug.Log("Player doesn't have deliverer");
            return;
        }
        deliverer.cashChangedEvent.AddListener(OnPlayerCashUpdated);

        Player playerComp = player.GetComponent<Player>();
        if (playerComp == null)
        {
            Debug.Log("Player doesn't have Player Script");
            return;
        }
        playerComp.playerCrashedEvent.AddListener(OnPlayerCrashed);
        playerComp.SetIndex(playerIndex);
        playerComp.PlaySpawnSFX();
    }
}