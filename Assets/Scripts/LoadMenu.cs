using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Globalization;
using UnityEngine.SceneManagement;
using SFB;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Runtime.Serialization;

public class LoadMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private GameObject gameMenu;
    [SerializeField]
    private GameObject loadMenu;
    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI noSavedGamesText;

    [SerializeField]
    private Button slotOneButton;
    [SerializeField]
    private Button slotTwoButton;
    [SerializeField]
    private Button slotThreeButton;
    [SerializeField]
    private Button importButton;

    [Header("Sound Settings")]
    [SerializeField]
    private AudioClip buttonClick;

    private MySaveGame[] savedGames;
    private AudioSource audioSource;
    private string importSavePath = null;
    private MySaveGame tempSavedGame;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        savedGames = new MySaveGame[] { null, null, null };
        ManageSaves();
    }

    /// <summary>
    /// Loades the game saved on the save slot
    /// </summary>
    /// <param name="slot">The save slot to load from</param>
    public void LoadGame(int slot)
    {
        audioSource.PlayOneShot(buttonClick);

        if (importSavePath is null)
        {
            Game.SaveGame = savedGames[slot - 1];
            SceneManager.LoadScene("Level");
        }
        else
        {
            string newFilePath = Path.Combine(Application.persistentDataPath, $"slot{slot}.sav");
            File.Copy(importSavePath, newFilePath);
            importSavePath = null;
            title.text = "Load Game";
            importButton.interactable = true;
            ManageSaves();
        }
    }

    /// <summary>
    /// Imports a unity save file to the game
    /// </summary>
    public void ImportSave()
    {
        ExtensionFilter[] extensionFilters = new ExtensionFilter[]
        {
            new ExtensionFilter("Unity Save Files", "sav"),
            new ExtensionFilter("All Files", "*")
        };

        string[] files = StandaloneFileBrowser.OpenFilePanel("Open Save File", "", extensionFilters, false);

        if (files.Length == 1)
        {
            bool isValid = true;

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                byte[] bytes = File.ReadAllBytes(files[0]);

                stream.Write(bytes, 0, bytes.Length);
                stream.Position = 0;
                tempSavedGame = formatter.Deserialize(stream) as MySaveGame;
            }
            catch (SerializationException)
            {
                isValid = false;
            }

            if (isValid && tempSavedGame != null && tempSavedGame.CurrentTetromino != null && tempSavedGame.NextTetromino != null && tempSavedGame.TimeStamp.HasValue && tempSavedGame.Minos != null)
            {
                title.text = "Select Slot";
                importSavePath = files[0];
                importButton.interactable = false;
                slotOneButton.gameObject.SetActive(true);
                slotTwoButton.gameObject.SetActive(true);
                slotThreeButton.gameObject.SetActive(true);

                if (!SaveGameSystem.DoesSaveGameExist("slot1"))
                {
                    TMP_Text timeStampText = slotOneButton.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault(x => x.gameObject.name == "TimestampText");
                    Button deleteButton = slotOneButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");

                    timeStampText.text = "";
                    deleteButton.gameObject.SetActive(false);
                }
                if (!SaveGameSystem.DoesSaveGameExist("slot2"))
                {
                    TMP_Text timeStampText = slotTwoButton.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault(x => x.gameObject.name == "TimestampText");
                    Button deleteButton = slotTwoButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");

                    timeStampText.text = "";
                    deleteButton.gameObject.SetActive(false);
                }
                if (!SaveGameSystem.DoesSaveGameExist("slot3"))
                {
                    TMP_Text timeStampText = slotThreeButton.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault(x => x.gameObject.name == "TimestampText");
                    Button deleteButton = slotThreeButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");

                    timeStampText.text = "";
                    deleteButton.gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Checks if any of the save slots exists and activates the save slots
    /// </summary>
    /// <param name="loadSaves">If it should load the games from the save slots</param>
    private void ManageSaves(bool loadSaves = true)
    {
        if (SaveGameSystem.DoesSaveGameExist("slot1"))
        {
            var (isValid, game) = SaveGameSystem.LoadGame("slot1");

            if (isValid)
            {
                TMP_Text timeStampText = slotOneButton.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault(x => x.gameObject.name == "TimestampText");
                Button deleteButton = slotOneButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");

                if (loadSaves && tempSavedGame is null)
                    savedGames[0] = game as MySaveGame;
                else if (tempSavedGame != null)
                    savedGames[0] = tempSavedGame;

                timeStampText.text = savedGames[0].TimeStamp.Value.ToString(CultureInfo.CurrentCulture);
                deleteButton.gameObject.SetActive(true);
                slotOneButton.gameObject.SetActive(true);
            }
        }
        else
            slotOneButton.gameObject.SetActive(false);

        if (SaveGameSystem.DoesSaveGameExist("slot2"))
        {
            var (isValid, game) = SaveGameSystem.LoadGame("slot2");

            if (isValid)
            {
                TMP_Text timeStampText = slotTwoButton.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault(x => x.gameObject.name == "TimestampText");
                Button deleteButton = slotTwoButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");

                if (loadSaves && tempSavedGame is null)
                    savedGames[1] = game as MySaveGame;
                else if (tempSavedGame != null)
                    savedGames[1] = tempSavedGame;

                timeStampText.text = savedGames[1].TimeStamp.Value.ToString(CultureInfo.CurrentCulture);
                deleteButton.gameObject.SetActive(true);
                slotTwoButton.gameObject.SetActive(true);
            }
        }
        else
            slotTwoButton.gameObject.SetActive(false);

        if (SaveGameSystem.DoesSaveGameExist("slot3"))
        {
            var (isValid, game) = SaveGameSystem.LoadGame("slot3");

            if (isValid)
            {
                TMP_Text timeStampText = slotThreeButton.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault(x => x.gameObject.name == "TimestampText");
                Button deleteButton = slotThreeButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");

                if (loadSaves && tempSavedGame is null)
                    savedGames[2] = game as MySaveGame;
                else if (tempSavedGame != null)
                    savedGames[2] = tempSavedGame;

                timeStampText.text = savedGames[2].TimeStamp.Value.ToString(CultureInfo.CurrentCulture);
                deleteButton.gameObject.SetActive(true);
                slotThreeButton.gameObject.SetActive(true);
            }
        }
        else
            slotThreeButton.gameObject.SetActive(false);

        if (savedGames[0] == null && savedGames[1] == null && savedGames[2] == null)
            noSavedGamesText.gameObject.SetActive(true);
        else
            noSavedGamesText.gameObject.SetActive(false);

        tempSavedGame = null;
    }

    /// <summary>
    /// Deletes a save slot
    /// </summary>
    /// <param name="slot">The save slot to delete</param>
    public void DeleteSave(int slot)
    {
        audioSource.PlayOneShot(buttonClick);
        string slotName = $"slot{slot}";
        SaveGameSystem.DeleteSaveGame(slotName);
        savedGames[slot - 1] = null;
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
        title.text = "Load Game";
        importSavePath = null;
        importButton.interactable = true;
        ManageSaves(false);
    }
}
