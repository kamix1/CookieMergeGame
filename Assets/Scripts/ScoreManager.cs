using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Extensions;
using Firebase.Database;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI leaderboardText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    private int score;
    private int highScore;

    private void Awake()
    {
        Instance = this;
        
    }

    private void Start()
    {
        UpdateScoreUI();
        SetHighScoreText();
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
        StartCoroutine(WaitAndLoadHighScore());
    }

    private IEnumerator WaitAndLoadHighScore()
    {
        while (!FireBaseInit.IsReady)
        {
            yield return null;
        }
        GetUserScore(AuthManager.User.UserId, score =>
        {
            highScore = score;
            highScoreText.text = highScore.ToString();
            Debug.Log("highScore: " + highScore);
        });
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

                // Отсортировать по убыванию (Firebase возвращает по возрастанию)
                leaderboard.Sort((a, b) => b.score.CompareTo(a.score));

                // Собрать строку
                string result = "";
                    for (int i = 0; i < leaderboard.Count; i++)
                    {
                        result += $"{i + 1}. {leaderboard[i].name} — {leaderboard[i].score}\n";
                    }

                    leaderboardText.text = result;
                }
                else
                {
                    leaderboardText.text = "Ошибка загрузки таблицы лидеров.";
                }
            });
    }

    public void GetUserScore(string username, Action<int> onResult)
    {
        if (!FireBaseInit.IsReady)
        {
            Debug.LogWarning("Firebase не готов!");
            onResult?.Invoke(-1); // или 0, или любой дефолт
            return;
        }

        FireBaseInit.database.RootReference
            .Child("leaderboard")
            .Child(username)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    var snapshot = task.Result;

                    if (snapshot.Exists && snapshot.HasChild("score"))
                    {
                        int score = int.Parse(snapshot.Child("score").Value.ToString());
                        onResult?.Invoke(score);
                    }
                    else
                    {
                        Debug.Log("Пользователь не найден или нет очков.");
                        onResult?.Invoke(0);
                    }
                }
                else
                {
                    Debug.LogError("Ошибка при получении очков: " + task.Exception);
                    onResult?.Invoke(-1);
                }
            });
        
    }
    public void UploadScore(string username, int score)
    {
        
        if (!FireBaseInit.IsReady)
        {
            Debug.LogWarning("Firebase ещё не готов!");
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
