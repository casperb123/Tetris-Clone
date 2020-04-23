using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private GameObject optionsMenu;
    [SerializeField]
    private GameObject controlsMenu;
    [SerializeField]
    private Button okButton;
    [SerializeField]
    private Button applyButton;
    [SerializeField]
    private Button cancelButton;
    [SerializeField]
    private Button backButton;
    [SerializeField]
    private GameObject template;

    [Header("General Settings")]
    [SerializeField]
    private Dialog dialog;

    private List<SavedControl> controls;
    private SavedControl changingControl;
    private AudioSource audioSource;
    private bool controlsChanged;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GetControls();
    }

    private void Update()
    {
        CheckUserInput();
    }

    /// <summary>
    /// Checks the users input
    /// </summary>
    private void CheckUserInput()
    {
        if (!Input.anyKeyDown || changingControl is null)
            return;

        foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
        {
            if (code == KeyCode.Mouse0 ||
                code == KeyCode.Mouse1 ||
                code == KeyCode.Mouse2 ||
                code == KeyCode.Mouse3 ||
                code == KeyCode.Mouse4 ||
                code == KeyCode.Mouse5 ||
                code == KeyCode.Mouse6)
                continue;

            if (Input.GetKey(code))
            {
                TMP_Text buttonText = GetButtonText(changingControl);

                changingControl.Key = code;
                changingControl = null;
                buttonText.text = code.ToString();
                return;
            }
        }
    }

    /// <summary>
    /// Gets the controls
    /// </summary>
    private void GetControls(bool makeButtons = true)
    {
        controls = SaveSystem.GetControls();

        if (makeButtons)
        {
            foreach (Transform child in template.transform.parent)
            {
                if (child != template.transform)
                    Destroy(child.gameObject);
            }

            foreach (SavedControl control in controls)
            {
                control.Changed.AddListener(ControlsUpdated);

                GameObject controlObject = Instantiate(template, template.transform.parent);
                controlObject.SetActive(true);
                TMP_Text controlText = controlObject.transform.GetChild(0).GetComponent<TMP_Text>();
                Button controlKeyButton = controlObject.transform.GetChild(1).GetComponent<Button>();
                TMP_Text controlKeyButtonText = controlKeyButton.transform.GetChild(0).GetComponent<TMP_Text>();

                controlText.text = control.Name;
                controlKeyButtonText.text = control.Key.ToString();
                controlKeyButton.onClick.AddListener(() => ChangeControl(control));
            }
        }
        else
        {
            foreach (SavedControl control in controls)
            {
                int index = controls.IndexOf(control) + 1;
                control.Changed.AddListener(ControlsUpdated);

                Transform controlTransform = template.transform.parent.GetChild(index);
                Button controlKeyButton = controlTransform.GetChild(1).GetComponent<Button>();
                TMP_Text controlKeyButtonText = controlKeyButton.transform.GetChild(0).GetComponent<TMP_Text>();

                controlKeyButtonText.text = control.Key.ToString();
                controlKeyButton.onClick.RemoveAllListeners();
                controlKeyButton.onClick.AddListener(() => ChangeControl(control));
            }
        }
    }

    /// <summary>
    /// Gets the button text for the control
    /// </summary>
    /// <param name="control">The control to get the text for</param>
    /// <returns>The button text for the control</returns>
    private TMP_Text GetButtonText(SavedControl control)
    {
        int index = controls.IndexOf(control) + 1;
        Transform controlTransform = template.transform.parent.GetChild(index);
        Transform buttonTransform = controlTransform.GetChild(1);
        TMP_Text buttonText = buttonTransform.GetChild(0).GetComponent<TMP_Text>();

        return buttonText;
    }

    /// <summary>
    /// Change the key for the control
    /// </summary>
    /// <param name="control">The contol to change</param>
    private void ChangeControl(SavedControl control)
    {
        TMP_Text buttonText = GetButtonText(control);

        CancelChange();
        changingControl = control;
        buttonText.text = "Press a key...";
    }

    /// <summary>
    /// Cancels the control change
    /// </summary>
    private void CancelChange()
    {
        if (changingControl is null)
            return;

        TMP_Text buttonText = GetButtonText(changingControl);
        buttonText.text = changingControl.Key.ToString();
    }

    /// <summary>
    /// Toggles the buttons
    /// </summary>
    private void ControlsUpdated()
    {
        okButton.gameObject.SetActive(true);
        applyButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
        controlsChanged = true;
    }

    /// <summary>
    /// Applies and saves the changes and goes back
    /// </summary>
    public void Ok()
    {
        Apply();
        Back();
    }

    /// <summary>
    /// Applies and saves the changes
    /// </summary>
    public void Apply()
    {
        audioSource.Play();
        SaveSystem.SaveControls(controls);
        if (Game.Instance != null)
            Game.Instance.GetControls();

        controlsChanged = false;
        okButton.gameObject.SetActive(false);
        applyButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Cancels the changes
    /// </summary>
    public void Cancel(bool goBack)
    {
        audioSource.Play();

        if (goBack && controlsChanged)
        {
            dialog.OnResult += (Dialog.DialogResult result) =>
            {
                if (result == Dialog.DialogResult.Yes)
                {
                    GetControls(false);
                    controlsChanged = false;
                    okButton.gameObject.SetActive(false);
                    applyButton.gameObject.SetActive(false);
                    cancelButton.gameObject.SetActive(false);
                    backButton.gameObject.SetActive(true);
                    Back();
                }
            };

            dialog.Open(Dialog.DialogType.YesNo, "Are you sure that you want to cancel the changes and go back?");
        }
        else
        {
            if (controlsChanged)
            {
                GetControls(false);
                controlsChanged = false;
                okButton.gameObject.SetActive(false);
                applyButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(true);
            }

            if (goBack)
                Back();
        }
    }

    /// <summary>
    /// Goes back to the options menu
    /// </summary>
    private void Back()
    {
        audioSource.Play();
        controlsMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
}
