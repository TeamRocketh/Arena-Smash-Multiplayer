using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [HideInInspector]
    public int score1, score2, epicWinner;
    public Image player1scoresprite, player2scoresprite;
    public Sprite P1zeroSprite, P1oneSprite, P1twoSprite, P1threeSprite;
    public Sprite P2zeroSprite, P2oneSprite, P2twoSprite, P2threeSprite;
    public static ScoreManager instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
        ResetEpicWinner();
    }

    private void OnEnable()
    {
        UpdateScore();
    }

    void Update()
    {
        if (Time.timeSinceLevelLoad < 0.1f)
        {
        }

        if (epicWinner != 0)
        {
            StartCoroutine(Delay(epicWinner));
            ResetEpicWinner();
        }
    }

    IEnumerator Delay(int x)
    {
        yield return new WaitForSeconds(2);
        if (x == 1)
            SceneManager.LoadScene("Player1Wins");
        if (x == 2)
            SceneManager.LoadScene("Player2Wins");
    }

    public void UpdateScore()
    {
        player1scoresprite.sprite = ReturnScore1Sprite();
        player2scoresprite.sprite = ReturnScore2Sprite();
    }

    public void ResetEpicWinner()
    {
        epicWinner = 0;
    }

    public void ResetScore()
    {
        score1 = score2 = 0;
    }

    Sprite ReturnScore1Sprite()
    {
        if (score1 == 0)
            return P1zeroSprite;
        else if (score1 == 1)
            return P1oneSprite;
        else if (score1 == 2)
            return P1twoSprite;
        else if (score1 == 3)
            return P1threeSprite;
        return null;
    }

    Sprite ReturnScore2Sprite()
    {
        if (score2 == 0)
            return P2zeroSprite;
        else if (score2 == 1)
            return P2oneSprite;
        else if (score2 == 2)
            return P2twoSprite;
        else if (score2 == 3)
            return P2threeSprite;
        return null;
    }
}