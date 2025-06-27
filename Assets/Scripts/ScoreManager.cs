using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Extensions;
using Firebase.Database;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI leaderboardText;
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
        UploadScore( AuthManager.User.Email.Split('@')[0] , PlayerPrefs.GetInt("HighScore", 0));
        highScoreText.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
    }

    public void GetTopScores(int count = 10)
    {
        Debug.Log("gettopScores");
        FireBaseInit.database.RootReference.Child("leaderboard")
            .OrderByChild("score")
            .LimitToLast(count)
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    List<(string name, int score)> leaderboard = new List<(string, int)>();

                    foreach (var child in snapshot.Children)
                    {
                        string name = child.Child("username").Value.ToString();
                        int score = int.Parse(child.Child("score").Value.ToString());
                        leaderboard.Add((name, score));
                    }

                // ќтсортировать по убыванию (Firebase возвращает по возрастанию)
                leaderboard.Sort((a, b) => b.score.CompareTo(a.score));

                // —обрать строку
                string result = "";
                    for (int i = 0; i < leaderboard.Count; i++)
                    {
                        result += $"{i + 1}. {leaderboard[i].name} Ч {leaderboard[i].score}\n";
                    }

                    leaderboardText.text = result;
                }
                else
                {
                    leaderboardText.text = "ќшибка загрузки таблицы лидеров.";
                }
            });
    }
    public void UploadScore(string username, int score)
    {
        if (!FireBaseInit.IsReady)
        {
            Debug.LogWarning("Firebase ещЄ не готов!");
            return;
        }

        string key = AuthManager.User.UserId;

        var entry = new Dictionary<string, object>();
        entry["username"] = username;
        entry["score"] = score;

        FireBaseInit.database.RootReference.Child("leaderboard").Child(key).SetValueAsync(entry);
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
            PlayerPrefs.Save(); // об€зательно сохран€ем
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
