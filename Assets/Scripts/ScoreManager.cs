using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI scoreText;
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

    public void IncreaseScore(int increase)
    {
        score += increase;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = score.ToString();
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
