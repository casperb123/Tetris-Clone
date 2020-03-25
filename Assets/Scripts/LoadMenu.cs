using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Globalization;
using UnityEngine.SceneManagement;

public class LoadMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private GameObject gameMenu;
    [SerializeField]
    private GameObject loadMenu;

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
    private GameMenu gameMenuScript;
    private AudioSource audioSource;

    private void Start()
    {
        gameMenuScript = GetComponent<GameMenu>();
        audioSource = GetComponent<AudioSource>();
        savedGames = new MySaveGame[] { null, null, null };

        if (SaveGameSystem.DoesSaveGameExist("slot1"))
        {
            TMP_Text timeStampText = slotOneButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");

            savedGames[0] = SaveGameSystem.LoadGame("slot1") as MySaveGame;
            timeStampText.text = savedGames[0].TimeStamp.ToString(CultureInfo.CurrentCulture);
            slotOneButton.gameObject.SetActive(true);
        }
        if (SaveGameSystem.DoesSaveGameExist("slot2"))
        {
            TMP_Text timeStampText = slotTwoButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");

            savedGames[1] = SaveGameSystem.LoadGame("slot2") as MySaveGame;
            timeStampText.text = savedGames[1].TimeStamp.ToString(CultureInfo.CurrentCulture);
            slotTwoButton.gameObject.SetActive(true);
        }
        if (SaveGameSystem.DoesSaveGameExist("slot3"))
        {
            TMP_Text timeStampText = slotThreeButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");

            savedGames[2] = SaveGameSystem.LoadGame("slot3") as MySaveGame;
            timeStampText.text = savedGames[2].TimeStamp.ToString(CultureInfo.CurrentCulture);
            slotThreeButton.gameObject.SetActive(true);
        }
    }

    public void LoadGame(int slot)
    {
        Game.SaveGame = savedGames[slot - 1];
        SceneManager.LoadScene("Level");
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
        loadMenu.SetActive(false);
        gameMenu.SetActive(true);
    }
}
