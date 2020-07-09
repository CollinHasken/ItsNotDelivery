using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool debugEnabled = false;
    public static GameManager Instance;
    public static bool inGame = false;

    public GameObject CameraPrefab;
    public string mainMenuScene;
    public string gameScene;

    public List<Color> playerColors = new List<Color>();
    public List<Material> playerMaterials = new List<Material>();

    public List<AudioClip> vehicleHitSFX = new List<AudioClip>();

    private List<GameObject> m_Players = new List<GameObject>();
    private List<GameObject> m_PlayerCameras = new List<GameObject>();
    private LevelManager m_LevelManager;
    private MenuManager m_MenuManager;
    private PlayerInputManager m_InputManager;

    private int m_playerLimit = 1;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_InputManager = GetComponent<PlayerInputManager>();
        if (!debugEnabled)
        {
            m_InputManager.DisableJoining();
        }
        m_MenuManager = FindObjectOfType<MenuManager>();

        m_LevelManager = FindObjectOfType<LevelManager>();
        if(m_LevelManager != null)
        {
            m_LevelManager.Init(m_Players, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetJoining(bool can_join)
    {
        if(can_join == true)
        {
            m_InputManager.EnableJoining();
        }
        else
        {
            m_InputManager.DisableJoining();
        }
    }

    public void LoadGame()
    {
        if (m_Players.Count >= m_playerLimit)
        {
            inGame = true;
            DontDestroyPlayers();
            SetJoining(false);
            StartCoroutine(LoadSceneAsync(gameScene));
        }
    }

    public void LoadMainMenu()
    {
        inGame = false;
        StartCoroutine(LoadSceneAsync(mainMenuScene));
    }

    void DontDestroyPlayers()
    {
        foreach(GameObject g in m_Players)
        {
            DontDestroyOnLoad(g);
        }
        foreach (GameObject g in m_PlayerCameras)
        {
            DontDestroyOnLoad(g);
        }
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        GameLoaded();
    }

    private void GameLoaded()
    {
        m_LevelManager = FindObjectOfType<LevelManager>();
        if (m_LevelManager == null)
        {
            Debug.LogError("Your level doesn't have a LevelManager!");
            return;
        }

        // Sort players list by index
        m_Players.Sort(delegate (GameObject playerA, GameObject playerB)
        {
            PlayerInput inputA = playerA.GetComponent<PlayerInput>();
            if (inputA == null)
            {
                Debug.LogErrorFormat("Player doesn't have PlayerInput from %s", playerA.name);
                return -1;
            }

            PlayerInput inputB = playerB.GetComponent<PlayerInput>();
            if (inputB == null)
            {
                Debug.LogErrorFormat("Player doesn't have PlayerInput from %s", playerB.name);
                return -1;
            }
            return inputA.playerIndex.CompareTo(inputB.playerIndex);
        });
        m_LevelManager.m_LevelTimedOut.AddListener(OnGameEnded);
        m_LevelManager.Init(m_Players, true);
    }

    private void OnGameEnded()
    {
        m_LevelManager.m_LevelTimedOut.RemoveListener(OnGameEnded);

        // Go to reward screen
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        Debug.Log("Player Joined");
        m_Players.Add(input.gameObject);
        if (m_MenuManager != null)
        {
            m_MenuManager.SetPlayerSelectVisible(input.playerIndex, false);
        }

        // Spawn camera
        GameObject cameraInstance = Instantiate(CameraPrefab);
        cameraInstance.GetComponent<CameraPlayerFollower>().playerObject = input.gameObject;
        input.camera = cameraInstance.GetComponent<Camera>();
        m_PlayerCameras.Add(cameraInstance);

        input.gameObject.GetComponent<Player>().SetColor(playerColors[input.playerIndex], playerMaterials[input.playerIndex]);

        if(m_LevelManager != null)
        {
            m_LevelManager.PlayerJoined(input.gameObject);
        }
    }

    public void OnPlayerLeft(PlayerInput input)
    {
        Debug.Log("Player Left");
        m_Players.Remove(input.gameObject);
        if (m_MenuManager != null)
        {
            m_MenuManager.SetPlayerSelectVisible(input.playerIndex, true);
        }
    }
}
