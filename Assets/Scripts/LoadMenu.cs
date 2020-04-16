using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using SFB;
using System;
using System.Collections.Generic;

public class LoadMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private GameObject gameMenu;
    [SerializeField]
    private GameObject loadMenu;
    [SerializeField]
    private GameObject importMenu;
    [SerializeField]
    private TextMeshProUGUI noSavedGamesText;
    [SerializeField]
    private GameObject buttonsContainer;

    [SerializeField]
    private Button slotTemplateButton;
    [SerializeField]
    private Button importButton;

    [Header("Sound Settings")]
    [SerializeField]
    private AudioClip buttonClick;

    public SavedGame[] TempSavedGames;
    private AudioSource audioSource;
    private ImportMenu importMenuScript;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        TempSavedGames = new SavedGame[SlotButtons.SaveSlots];
        importMenuScript = GetComponent<ImportMenu>();
        ManageSaves();
    }

    /// <summary>
    /// Loades the game saved on the save slot
    /// </summary>
    /// <param name="slot">The save slot to load from</param>
    public void LoadGame(int slot)
    {
        audioSource.PlayOneShot(buttonClick);
        TempSavedGames[slot - 1].LastLoaded = DateTime.Now;
        if (!SaveSystem.SaveGame(TempSavedGames[slot - 1], slot))
            return;

        Game.SaveGame = TempSavedGames[slot - 1];
        SceneManager.LoadScene("Level");
    }

    /// <summary>
    /// Imports a unity save file to the game
    /// </summary>
    public void ImportSave()
    {
        audioSource.PlayOneShot(buttonClick);

        ExtensionFilter[] extensionFilters = new ExtensionFilter[]
        {
            new ExtensionFilter("Unity Save Files", "sav"),
            new ExtensionFilter("All Files", "*")
        };

        string[] files = StandaloneFileBrowser.OpenFilePanel("Open Save File", "", extensionFilters, false);

        if (files.Length == 1)
        {
            ImportMenu.ImportSavePath = files[0];
            importMenuScript.GetSaves();
            loadMenu.SetActive(false);
            importMenu.SetActive(true);
        }
    }

    /// <summary>
    /// Checks if any of the save slots exists and activates the save slots
    /// </summary>
    /// <param name="useLoadedSaves">If the loaded saves should be used</param>
    public void ManageSaves(bool useLoadedSaves = true)
    {
        var (buttons, savedGames) = SlotButtons.GetSaves(slotTemplateButton, TempSavedGames, hideIfNoSave: true);
        foreach (Button slotButton in buttons)
        {
            int index = buttons.IndexOf(slotButton);
            Button deleteButton = slotButton.transform.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");
            TMP_Text timeStampText = slotButton.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault(x => x.gameObject.name == "TimestampText");
            SavedGame savedGame = null;

            if (useLoadedSaves)
                savedGame = savedGames.FirstOrDefault(x => x.Slot == index + 1);
            else if (TempSavedGames[index] != null)
                savedGame = TempSavedGames[index];

            if (savedGame != null)
            {
                slotButton.onClick.AddListener(() => LoadGame(index + 1));
                deleteButton.onClick.AddListener(() => DeleteSave(index + 1));
                if (useLoadedSaves)
                    TempSavedGames[index] = savedGame;
            }
        }

        if (savedGames.Count == 0)
        {
            noSavedGamesText.gameObject.SetActive(true);
            buttonsContainer.SetActive(false);
        }
        else
        {
            noSavedGamesText.gameObject.SetActive(false);
            buttonsContainer.SetActive(true);
        }
    }

    /// <summary>
    /// Deletes a save slot
    /// </summary>
    /// <param name="slot">The save slot to delete</param>
    public void DeleteSave(int slot)
    {
        audioSource.PlayOneShot(buttonClick);
        SaveSystem.DeleteSaveGame(slot);
        TempSavedGames[slot - 1] = null;
        ManageSaves(false);
    }

    /// <summary>
    /// Goes back to the game menu
    /// </summary>
    public void Back()
    {
        audioSource.PlayOneShot(buttonClick);
        loadMenu.SetActive(false);
        gameMenu.SetActive(true);
        importButton.interactable = true;
        ManageSaves(false);
    }
}
