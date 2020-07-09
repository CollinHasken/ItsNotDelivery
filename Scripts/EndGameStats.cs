using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class EndGameStats : MonoBehaviour
{

    public LevelManager levelManager;
    public List<TextMeshProUGUI> finalScoreTexts = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> pizzasDelivered = new List<TextMeshProUGUI>();
    public TextMeshProUGUI winnerText;
    public EventSystem eventSystem;
    public GameObject mainMenuButton;

    public Animator anim;

    private GameObject[] m_players;


    // Start is called before the first frame update
    void Start()
    {
        levelManager.m_LevelTimedOut.AddListener(ShowEndGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShowEndGame()
    {
        m_players = levelManager.GetPlayers();

        int idx = 0;
        int highestCashIdx = 0;
        decimal highestCashValue = 0;
        foreach(GameObject player in m_players)
        {
            Deliverer playerDeliverer = player.GetComponent<Deliverer>();
            decimal playerCash = playerDeliverer.Cash;
            string playerCashText = "$" + playerCash;
            finalScoreTexts[idx].SetText(playerCashText);

            if(playerCash > highestCashValue)
            {
                highestCashValue = playerCash;
                highestCashIdx = idx;
            }

            int pizzas = playerDeliverer.PizzasDelivered;
            string pizzaText = pizzas.ToString();
            pizzasDelivered[idx].SetText(pizzaText);

            idx++; 
        }

        GameObject winningPlayer = m_players[highestCashIdx];
        Color playerColor = winningPlayer.GetComponent<Player>().GetColor();
        string WinnerText = "Player " + (highestCashIdx + 1) + "\n WINS!";
        winnerText.SetText(WinnerText);
        winnerText.color = playerColor;

        eventSystem.SetSelectedGameObject(mainMenuButton);
        anim.gameObject.SetActive(true);
        anim.SetBool("GameEnd", true);
    }
}
