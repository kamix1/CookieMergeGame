using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Canvas leaderboardCanvas;

    private void Awake()
    {
        leaderboardCanvas.gameObject.SetActive(false);
    }
    void Start()
    {
        button.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        bool isActive = leaderboardCanvas.gameObject.activeSelf;
        leaderboardCanvas.gameObject.SetActive(!isActive);
        ScoreManager.Instance.GetTopScores();
    }
    void Update()
    {
        
    }
}
