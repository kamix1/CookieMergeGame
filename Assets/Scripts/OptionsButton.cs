using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsButton : MonoBehaviour
{
    [SerializeField] private Button optionsButton;

    private void Start()
    {
        optionsButton.onClick.AddListener(OptionsButtonOnClick);
    }

    private void OptionsButtonOnClick()
    {
        PauseMenu.Instance.SetPaused(true);
    }
}
