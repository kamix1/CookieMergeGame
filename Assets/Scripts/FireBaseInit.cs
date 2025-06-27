using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FireBaseInit : MonoBehaviour
{
    public static FirebaseDatabase database;
    public static bool IsReady = false;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri("https://cookiemergegame-default-rtdb.firebaseio.com/");
                database = FirebaseDatabase.DefaultInstance;
                IsReady = true;
                Debug.Log("Firebase инициализирован");
            }
            else
            {
                Debug.LogError("Firebase error: " + status);
            }
        });
    }
}