using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject optionsMenu;

    private void Start()
    {
        resumeButton.onClick.AddListener(OnClickResumeButton);
        exitButton.onClick.AddListener(OnClickExitButton);
    }

    private void OnClickResumeButton()
    {
        optionsMenu.SetActive(false);
    }

    private void OnClickExitButton()
    {
        Application.Quit();
    }

}
