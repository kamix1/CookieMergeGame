using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    private int score;

    private void Awake()
    {
        Instance = this;
        score = 0;
    }

    private void Start()
    {
        UpdateScoreUI();
    }
    private void Update()
    {

    }

    public void resetScore()
    {
        score = 0;
        UpdateScoreUI();
    }
    public void SetHighScoreText()
    {

        highScoreText.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
    }
    public void IncreaseScore(int increase)
    {
        score += increase;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = score.ToString();
        gameOverScoreText.text = score.ToString();
    }

    public void CheckAndSaveHighScore()
    {
        int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save(); // обязательно сохраняем
            Debug.Log("New High Score: " + score);
        }
    }

    public int CookieTypeToScore(Cell.CookieType cookieType)
    {
        switch (cookieType)
        {
            case Cell.CookieType.cookie:
                return 5;
            case Cell.CookieType.toast:
                return 40;
            case Cell.CookieType.mafin:
                return 200;
            case Cell.CookieType.pankeki:
                return 500;
            case Cell.CookieType.cake:
                return 1500;
            case Cell.CookieType.doubleCake:
                return 5000;
            case Cell.CookieType.doubleGlazurCake:
                return 15000;
            case Cell.CookieType.gingerbreadManCoctail:
                return 1000;
            case Cell.CookieType.gingerbreadManSet:
                return 5000;
            default:
                return 0;
        }
    }
}
