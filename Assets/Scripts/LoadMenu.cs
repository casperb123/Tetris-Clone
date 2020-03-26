using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Globalization;
using UnityEngine.SceneManagement;
using SFB;
using System.IO;

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

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        savedGames = new MySaveGame[] { null, null, null };
        ManageSaves();
    }

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

    public void ImportSave()
    {
        ExtensionFilter[] extensionFilters = new ExtensionFilter[]
        {
            new ExtensionFilter("Unity Save Files", "sav")
        };

        string[] files = StandaloneFileBrowser.OpenFilePanel("Open Save File", "", extensionFilters, false);

        if (files.Length == 1)
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

    private void ManageSaves(bool loadSaves = true)
    {
        if (SaveGameSystem.DoesSaveGameExist("slot1"))
        {
            TMP_Text timeStampText = slotOneButton.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault(x => x.gameObject.name == "TimestampText");
            Button deleteButton = slotOneButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");

            if (loadSaves)
                savedGames[0] = SaveGameSystem.LoadGame("slot1") as MySaveGame;

            timeStampText.text = savedGames[0].TimeStamp.ToString(CultureInfo.CurrentCulture);
            deleteButton.gameObject.SetActive(true);
            slotOneButton.gameObject.SetActive(true);
        }
        else
            slotOneButton.gameObject.SetActive(false);

        if (SaveGameSystem.DoesSaveGameExist("slot2"))
        {
            TMP_Text timeStampText = slotTwoButton.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault(x => x.gameObject.name == "TimestampText");
            Button deleteButton = slotTwoButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");

            if (loadSaves)
                savedGames[1] = SaveGameSystem.LoadGame("slot2") as MySaveGame;

            timeStampText.text = savedGames[1].TimeStamp.ToString(CultureInfo.CurrentCulture);
            deleteButton.gameObject.SetActive(true);
            slotTwoButton.gameObject.SetActive(true);
        }
        else
            slotTwoButton.gameObject.SetActive(false);

        if (SaveGameSystem.DoesSaveGameExist("slot3"))
        {
            TMP_Text timeStampText = slotThreeButton.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault(x => x.gameObject.name == "TimestampText");
            Button deleteButton = slotThreeButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");

            if (loadSaves)
                savedGames[2] = SaveGameSystem.LoadGame("slot3") as MySaveGame;

            timeStampText.text = savedGames[2].TimeStamp.ToString(CultureInfo.CurrentCulture);
            deleteButton.gameObject.SetActive(true);
            slotThreeButton.gameObject.SetActive(true);
        }
        else
            slotThreeButton.gameObject.SetActive(false);

        if (savedGames[0] == null && savedGames[1] == null && savedGames[2] == null)
            noSavedGamesText.gameObject.SetActive(true);
        else
            noSavedGamesText.gameObject.SetActive(false);
    }

    public void DeleteSave(int slot)
    {
        audioSource.PlayOneShot(buttonClick);
        string slotName = $"slot{slot}";
        SaveGameSystem.DeleteSaveGame(slotName);
        savedGames[slot - 1] = null;
        ManageSaves(false);
    }

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
