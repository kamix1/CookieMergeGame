using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip placingSound;
    [SerializeField] private AudioClip platePlacingSound;
    [SerializeField] private AudioClip mergeSound;
    [SerializeField] private AudioClip mergeGingerbreadMan;
    [SerializeField] private AudioClip mixerSound;
    [SerializeField] private AudioClip microwaveSound;

    
    private void OnEnable()
    {
        GameManager.OnPlatePlacing += GameManager_OnPlatePlacing;
        GameManager.OnPlacing += GameManager_OnPlacing;
        GameManager.OnMerge += GameManager_OnMerge;
        GameManager.OnMergeGingerbreadMans += GameManager_OnMergeGingerbreadMans;
        GameManager.OnMicrowaved += GameManager_OnMicrowaved;
        GameManager.OnMixered += GameManager_OnMixered;

    }

    private void GameManager_OnMixered(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(mixerSound, 1f);
    }

    private void GameManager_OnMicrowaved(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(microwaveSound);
    }

    private void GameManager_OnMergeGingerbreadMans(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(mergeGingerbreadMan);
    }

    private void GameManager_OnMerge(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(mergeSound);
    }

    private void GameManager_OnPlacing(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(placingSound);
    }

    private void GameManager_OnPlatePlacing(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(platePlacingSound);
    }

    private void OnDisable()
    {
        GameManager.OnPlatePlacing -= GameManager_OnPlatePlacing;
        GameManager.OnPlacing -= GameManager_OnPlacing;
        GameManager.OnMerge -= GameManager_OnMerge;
        GameManager.OnMergeGingerbreadMans -= GameManager_OnMergeGingerbreadMans;
        GameManager.OnMicrowaved -= GameManager_OnMicrowaved;
        GameManager.OnMixered -= GameManager_OnMixered;
    }
}
