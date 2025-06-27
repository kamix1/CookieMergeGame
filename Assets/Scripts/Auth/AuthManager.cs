using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance;
    public static FirebaseAuth Auth;
    public static FirebaseUser User;
    public static bool IsLoggedIn => User != null;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase Auth готов");
            }
            else
            {
                Debug.LogError("Firebase Auth ошибка: " + task.Result);
            }
        });
    }

    public void Register(string login, string password, Action onSuccess = null)
    {
        string email = login + "@fake.com";
        Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && task.IsCompleted)
            {
                User = task.Result.User;
                Debug.Log("Регистрация успешна: " + User.Email);
                onSuccess?.Invoke();
            }
            else
            {
                Debug.LogError("Ошибка регистрации: " + task.Exception);
            }
        });
    }

    public void Login(string login, string password, Action onSuccess = null)
    {
        string email = login + "@fake.com";
        Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && task.IsCompleted)
            {
                User = task.Result.User;
                Debug.Log("Вход выполнен: " + User.Email);
                onSuccess?.Invoke();
            }
            else
            {
                Debug.LogError("Ошибка входа: " + task.Exception);
            }
        });
    }
}
