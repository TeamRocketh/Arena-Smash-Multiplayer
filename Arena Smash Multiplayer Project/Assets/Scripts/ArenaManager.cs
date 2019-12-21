using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ArenaManager : MonoBehaviour
{
    public static ArenaManager instance;

    [HideInInspector] public int winner;
    public PlayerCircularController player1, player2;
    public GameObject playerWins;
    Text infoText;

    public GameObject shotgunBulletSpread;
    public Sprite P1Wall, P2Wall;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);

        winner = 0;

        player1.Init();
        player2.Init();
    }

    private void Start()
    {
        infoText = playerWins.GetComponent<Text>();
        StartGame();
    }

    private void Update()
    {
        if (winner != 0)
        {
            PlayerControlOverride(true);
            playerWins.SetActive(true);
            if (winner == 1)
            {
                infoText.text = "<color=#0095F9>BLUE WINS</color>";
            }
            else if (winner == 2)
            {
                infoText.text = "<color=#FF8600>ORANGE WINS</color>";
            }
            if (ScoreManager.instance.score1 < 3 && ScoreManager.instance.score2 < 3)
                StartCoroutine(RestartCoroutine());
        }
    }

    public void StartGame()
    {
        StartCoroutine(ReverseCount());
    }

    public void Restart()
    {
        winner = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    string ReturnBulletInfo(int x)
    {
        if (x == 1)
            return "Bouncy";
        if (x == 2)
            return "Shotgun";
        if (x == 3)
            return "Sticky";
        return "";
    }

    void PlayerControlOverride(bool sweech)
    {
        if (!sweech)
        {
            player1.overrideControl = false;
            player2.overrideControl = false;
        }
        if (sweech)
        {
            player1.overrideControl = true;
            player2.overrideControl = true;
        }
    }

    IEnumerator RestartCoroutine()
    {
        yield return new WaitForSeconds(3);
        Restart();
    }

    IEnumerator ReverseCount()
    {
        PlayerControlOverride(true);
        infoText.text = "GAME BEGINS IN";
        yield return new WaitForSeconds(1);
        infoText.text = "3";
        yield return new WaitForSeconds(1);
        infoText.text = "2";
        yield return new WaitForSeconds(1);
        infoText.text = "1";
        yield return new WaitForSeconds(1);
        infoText.text = "GO";
        yield return new WaitForSeconds(1);
        playerWins.SetActive(false);
        PlayerControlOverride(false);
    }
}