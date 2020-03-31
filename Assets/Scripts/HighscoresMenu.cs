using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighscoresMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private GameObject backMenu;
    [SerializeField]
    private GameObject highscoresMenu;

    [Header("Sound Settings")]
    [SerializeField]
    private AudioClip buttonClick;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Back()
    {
        if (buttonClick == null)
            audioSource.Play();
        else
            audioSource.PlayOneShot(buttonClick);

        highscoresMenu.SetActive(false);
        backMenu.SetActive(true);
    }
}
