using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject optionsMenu;
    private bool isPaused = false;

    private void Awake()
    {
        Instance = this;
        Instance.gameObject.SetActive(false);
    }

    public bool Paused()
    {
        return isPaused;
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
        optionsMenu.SetActive(paused);
    }
    private void Start()
    {
        resumeButton.onClick.AddListener(OnClickResumeButton);
        exitButton.onClick.AddListener(OnClickExitButton);
    }

    private void OnClickResumeButton()
    {
        SetPaused(false);
    }

    private void OnClickExitButton()
    {
        Application.Quit();
    }

}
