using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImportMenu : MonoBehaviour
{
    public static string ImportSavePath;

    [Header("UI Settings")]
    [SerializeField]
    private Button slotTemplateButton;
    [SerializeField]
    private GameObject loadMenu;
    [SerializeField]
    private GameObject importMenu;

    [Header("Sound Settings")]
    [SerializeField]
    private AudioClip buttonClick;

    private LoadMenu loadMenuScript;
    private AudioSource audioSource;
    private SavedGame[] tempSavedGames;

    private void Start()
    {
        loadMenuScript = GetComponent<LoadMenu>();
        audioSource = GetComponent<AudioSource>();
        tempSavedGames = new SavedGame[SlotButtons.SaveSlots];
        GetSaves();
    }

    /// <summary>
    /// Gets the saved games
    /// </summary>
    public void GetSaves()
    {
        var (buttons, savedGames) = SlotButtons.GetSaves(slotTemplateButton, canDelete: false);

        foreach (Button slotButton in buttons)
        {
            int index = buttons.IndexOf(slotButton);
            slotButton.onClick.AddListener(() => ImportSave(index + 1));
            SavedGame savedGame = savedGames.FirstOrDefault(x => x.Slot == index + 1);

            if (savedGame != null)
                tempSavedGames[index] = savedGame;
        }
    }

    /// <summary>
    /// Imports a saved game to the save slot
    /// </summary>
    /// <param name="slot">The save slot to import to</param>
    public void ImportSave(int slot)
    {
        audioSource.PlayOneShot(buttonClick);

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            byte[] bytes = File.ReadAllBytes(ImportSavePath);

            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;

            if (formatter.Deserialize(stream) is SavedGame savedGame && savedGame.CurrentTetromino != null && savedGame.NextTetromino != null && savedGame.Minos != null)
            {
                if (SaveSystem.SaveGame(savedGame, slot))
                    tempSavedGames[slot - 1] = savedGame;
            }
        }
        catch (SerializationException)
        {
            Back();
        }

        Back();
    }

    public void Cancel()
    {
        audioSource.PlayOneShot(buttonClick);
        Back();
    }

    private void Back()
    {
        ImportSavePath = string.Empty;
        loadMenuScript.TempSavedGames = tempSavedGames;
        loadMenuScript.ManageSaves(false);
        importMenu.SetActive(false);
        loadMenu.SetActive(true);
    }
}
