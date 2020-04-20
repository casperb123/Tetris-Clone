using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Dialog : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textField;
    [SerializeField]
    private GameObject yesNo;
    [SerializeField]
    private GameObject ok;
    [SerializeField]
    private GameObject saveQuit;

    public enum Result
    {
        Yes,
        No,
        Ok,
        Save
    }

    public enum Type
    {
        YesNo,
        Ok,
        Save
    }

    public UnityAction<Result> onResult = (result) =>
    {
        Debug.Log($"Result: {result}");
    };

    public void Open(Type type, string text)
    {
        if (type == Type.YesNo)
            yesNo.SetActive(true);
        else if (type == Type.Ok)
            ok.SetActive(true);
        else if (type == Type.Save)
            saveQuit.SetActive(true);

        textField.text = text;
        transform.gameObject.SetActive(true);
    }

    public void OnResult(int result)
    {
        onResult.Invoke((Result)result);
    }
}
