using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBordButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Canvas leaderboardCanvas;
    void Start()
    {
        button.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        bool isActive = leaderboardCanvas.gameObject.activeSelf;
        leaderboardCanvas.gameObject.SetActive(!isActive);
    }
    void Update()
    {
        
    }
}
