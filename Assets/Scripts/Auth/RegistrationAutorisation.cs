using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegistrationAutorisation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI login;
    [SerializeField] private TextMeshProUGUI password;
    [SerializeField] private Button submitLoginButton;
    [SerializeField] private Button submitRegistrationButton;

    private void Start()
    {
        submitLoginButton.onClick.AddListener(SubmitLogin);
        submitRegistrationButton.onClick.AddListener(SubmitRegistration);
    }

    private void SubmitLogin()
    {
        AuthManager.Instance.Login(login.text,password.text, GoToNextScene);
    }

    private void SubmitRegistration()
    {
        AuthManager.Instance.Register(login.text, password.text, GoToNextScene);
    }

    private void GoToNextScene()
    {
        SceneManager.LoadScene("Scene");
    }
}
