using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[System.Serializable]
public class PlayerFadeCompletedEvent : UnityEvent<int> { }

public class MenuManager : MonoBehaviour
{
    public enum FadeState
    {
        NoFade,
        FadeIn,
        FadeOut
    }

    public static MenuManager Instance;

    public GameObject MainMenu;
    public GameObject PlayerSelectMenu;
    public GameObject CreditsMenu;
    public GameObject PauseMenu;
    public List<GameObject> m_Player_info = new List<GameObject>();
    public Image[] PlayerFades = new Image[4];


    private PlayerFadeCompletedEvent[] playerFadeCompletedEvents = new PlayerFadeCompletedEvent[] { new PlayerFadeCompletedEvent(), new PlayerFadeCompletedEvent(), new PlayerFadeCompletedEvent(), new PlayerFadeCompletedEvent() };
    private FadeState[] playerFading = new FadeState[] { FadeState.NoFade, FadeState.NoFade, FadeState.NoFade, FadeState.NoFade };
    private float[] playerFadingScale = new float[] { 0, 0, 0, 0 };

    private bool isPaused = false;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.inGame == false)
        {
            ShowMainMenu();
        }
        else
        {
            PauseMenu.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int playerIndex = 0; playerIndex < 4; ++playerIndex)
        {
            if(playerFading[playerIndex] == FadeState.NoFade)
            {
                continue;
            }

            Color fade = PlayerFades[playerIndex].color;
            fade.a += (playerFading[playerIndex] == FadeState.FadeOut ? Time.deltaTime : Time.deltaTime * -1) * playerFadingScale[playerIndex];
            fade.a = Mathf.Clamp(fade.a, 0, 1);
            PlayerFades[playerIndex].color = fade;

            if (fade.a == 0 || fade.a == 1)
            {
                playerFading[playerIndex] = FadeState.NoFade;
                playerFadeCompletedEvents[playerIndex].Invoke(playerIndex);
            }
        }
    }

    void HideAll()
    {
        if (MainMenu != null)
        {
            MainMenu.SetActive(false);
        }
        if (PlayerSelectMenu != null)
        {
            PlayerSelectMenu.SetActive(false);
        }
        if (CreditsMenu != null)
        {
            CreditsMenu.SetActive(false);
        }
        if (PauseMenu != null)
        {
            PauseMenu.SetActive(false);
        }
    }

    public void ShowMainMenu()
    {
        HideAll();
        if (MainMenu != null)
        {
            MainMenu.SetActive(true);
        }
    }

    public void ShowPlayerSelect()
    {
        HideAll();
        PlayerSelectMenu.SetActive(true);
    }

    public void ShowCredits()
    {
        HideAll();
        CreditsMenu.SetActive(true);
    }

    public void SetPause(bool paused)
    {
        isPaused = paused;
        Pause(isPaused);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Pause(isPaused);
    }

    private void Pause(bool pause)
    {
        if (pause == true)
        {
            PauseMenu.SetActive(pause);
            Time.timeScale = 0f;
        }
        else
        {
            PauseMenu.SetActive(pause);
            Time.timeScale = 1f;
        }
    }

    public void SetPlayerSelectVisible(int playerIndex, bool isVisible)
    {
        if(playerIndex < m_Player_info.Count && m_Player_info[playerIndex] != null)
        {
            m_Player_info[playerIndex].SetActive(isVisible);
        }
    }

    public void SetPlayerFade(int player, float fadeTime, FadeState fadeState, UnityAction<int> callback)
    {
        if(playerFading[player] == fadeState)
        {
            return;
        }

        playerFading[player] = fadeState;
        playerFadingScale[player] = 1 / fadeTime;
        playerFadeCompletedEvents[player].AddListener(callback);
    }

    public void ClearPlayerFadeListeners(int player)
    {
        playerFadeCompletedEvents[player].RemoveAllListeners();
    }

    public void LoadMainMenu()
    {
        GameManager.Instance.LoadMainMenu();
    }
}
