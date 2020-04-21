using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private TMP_Text textField;
    [SerializeField]
    private GameObject yesNo;
    [SerializeField]
    private GameObject ok;
    [SerializeField]
    private GameObject saveYesNo;

    [Header("General Settings")]
    [SerializeField]
    private GameObject container;

    private AudioSource audioSource;
    private Image image;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        image = GetComponent<Image>();
    }

    public enum DialogResult
    {
        Yes,
        No,
        Ok,
        Save
    }

    public enum DialogType
    {
        YesNo,
        Ok,
        SaveYesNo
    }

    public UnityAction<DialogResult> OnResult = (result) =>
    {
        Debug.Log($"Result: {result}");
    };

    public void Open(DialogType type, string text)
    {
        yesNo.SetActive(false);
        ok.SetActive(false);
        saveYesNo.SetActive(false);

        if (type == DialogType.YesNo)
            yesNo.SetActive(true);
        else if (type == DialogType.Ok)
            ok.SetActive(true);
        else if (type == DialogType.SaveYesNo)
            saveYesNo.SetActive(true);

        textField.text = text;
        image.enabled = true;
        container.SetActive(true);
    }

    public void Result(int result)
    {
        audioSource.Play();
        OnResult.Invoke((DialogResult)result);
        container.SetActive(false);
        image.enabled = false;
    }
}
