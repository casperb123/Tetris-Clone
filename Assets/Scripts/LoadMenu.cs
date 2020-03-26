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
    private Button slotOneButton;
    [SerializeField]
    private Button slotTwoButton;
    [SerializeField]
    private Button slotThreeButton;

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
        ManageLoadButtons();
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
            ManageLoadButtons();
        }
    }

    public void ImportSave()
    {
        string[] files = StandaloneFileBrowser.OpenFilePanel("Open Save File", "", "sav", false);

        if (files.Length == 1)
        {
            title.text = "Select Slot";
            importSavePath = files[0];
            slotOneButton.gameObject.SetActive(true);
            slotTwoButton.gameObject.SetActive(true);
            slotThreeButton.gameObject.SetActive(true);
        }
    }

    private void ManageLoadButtons()
    {
        if (SaveGameSystem.DoesSaveGameExist("slot1"))
        {
            TMP_Text timeStampText = slotOneButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");

            savedGames[0] = SaveGameSystem.LoadGame("slot1") as MySaveGame;
            timeStampText.text = savedGames[0].TimeStamp.ToString(CultureInfo.CurrentCulture);
            slotOneButton.gameObject.SetActive(true);
        }
        else
            slotOneButton.gameObject.SetActive(false);

        if (SaveGameSystem.DoesSaveGameExist("slot2"))
        {
            TMP_Text timeStampText = slotTwoButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");

            savedGames[1] = SaveGameSystem.LoadGame("slot2") as MySaveGame;
            timeStampText.text = savedGames[1].TimeStamp.ToString(CultureInfo.CurrentCulture);
            slotTwoButton.gameObject.SetActive(true);
        }
        else
            slotTwoButton.gameObject.SetActive(false);

        if (SaveGameSystem.DoesSaveGameExist("slot3"))
        {
            TMP_Text timeStampText = slotThreeButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");

            savedGames[2] = SaveGameSystem.LoadGame("slot3") as MySaveGame;
            timeStampText.text = savedGames[2].TimeStamp.ToString(CultureInfo.CurrentCulture);
            slotThreeButton.gameObject.SetActive(true);
        }
        else
            slotThreeButton.gameObject.SetActive(false);
    }

    public void DeleteSave(int slot)
    {
        audioSource.PlayOneShot(buttonClick);
        string slotName = $"slot{slot}";
        SaveGameSystem.DeleteSaveGame(slotName);
        savedGames[slot - 1] = null;

        if (slot == 1)
            slotOneButton.gameObject.SetActive(false);
        else if (slot == 2)
            slotTwoButton.gameObject.SetActive(false);
        else if (slot == 3)
            slotThreeButton.gameObject.SetActive(false);
    }

    public void Back()
    {
        audioSource.PlayOneShot(buttonClick);
        loadMenu.SetActive(false);
        gameMenu.SetActive(true);
        title.text = "Load Game";
        importSavePath = null;
        ManageLoadButtons();
    }
}
