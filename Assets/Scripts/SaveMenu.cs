using System;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject saveMenu;

    [SerializeField]
    private Button slotOneButton;
    [SerializeField]
    private Button slotTwoButton;
    [SerializeField]
    private Button slotThreeButton;

    private SavedGame[] savedGames;
    private AudioSource audioSource;

    private void Start()
    {
        savedGames = new SavedGame[] { null, null, null };
        audioSource = GetComponent<AudioSource>();

        if (SaveSystem.DoesSaveGameExist(1))
        {
            var (isValid, game) = SaveSystem.LoadGame(1);

            if (isValid)
            {
                TMP_Text timeStampText = slotOneButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");
                Button deleteButton = slotOneButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");

                savedGames[0] = game as SavedGame;
                timeStampText.text = savedGames[0].TimeStamp.ToString(CultureInfo.CurrentCulture);
                deleteButton.gameObject.SetActive(true);
            }
        }
        if (SaveSystem.DoesSaveGameExist(2))
        {
            var (isValid, game) = SaveSystem.LoadGame(2);

            if (isValid)
            {
                TMP_Text timeStampText = slotTwoButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");
                Button deleteButton = slotTwoButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");

                savedGames[1] = game as SavedGame;
                timeStampText.text = savedGames[1].TimeStamp.ToString(CultureInfo.CurrentCulture);
                deleteButton.gameObject.SetActive(true);
            }
        }
        if (SaveSystem.DoesSaveGameExist(3))
        {
            var (isValid, game) = SaveSystem.LoadGame(3);

            if (isValid)
            {
                TMP_Text timeStampText = slotThreeButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");
                Button deleteButton = slotThreeButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");

                savedGames[2] = game as SavedGame;
                timeStampText.text = savedGames[2].TimeStamp.ToString(CultureInfo.CurrentCulture);
                deleteButton.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Saves the current game
    /// </summary>
    /// <param name="slot">The slot to save to</param>
    public void SaveGame(int slot)
    {
        audioSource.Play();
        SavedTetromino currentTetromino = new SavedTetromino(Game.Instance.CurrentTetromino.name.Replace("(Clone)", ""), Game.Instance.CurrentTetromino.transform.position.x, Game.Instance.CurrentTetromino.transform.position.y, (int)Game.Instance.CurrentTetromino.transform.rotation.eulerAngles.z);
        SavedTetromino nextTetromino = new SavedTetromino(Game.Instance.NextTetromino.name.Replace("(Clone)", ""));
        SavedTetromino savedTetromino = null;

        if (Game.Instance.SavedTetromino != null)
            savedTetromino = new SavedTetromino(Game.Instance.SavedTetromino.name.Replace("(Clone)", ""));

        SavedGame saveGame = new SavedGame
        {
            LastLoaded = DateTime.Now,
            Score = Game.Instance.CurrentScore,
            Lines = Game.Instance.TotalLinesCleared,
            Name = Game.Instance.Name,
            CurrentTetromino = currentTetromino,
            NextTetromino = nextTetromino,
            SavedTetromino = savedTetromino
        };

        for (int x = 0; x < Game.Instance.GridWidth; x++)
        {
            for (int y = 0; y < Game.Instance.GridHeight; y++)
            {
                Transform mino = Game.Instance.Grid[x, y];
                if (mino is null || mino.parent == null)
                    continue;

                if (mino.parent.TryGetComponent(out Tetromino tetromino))
                {
                    if (tetromino.enabled)
                        continue;
                }

                string name = mino.name.Replace("(Clone)", "");
                int nameIndex = name.IndexOf(' ');

                if (nameIndex != -1)
                    name = mino.name.Substring(0, nameIndex);

                Vector2 pos = Game.Instance.Round(mino.position);

                SavedMino savedMino = new SavedMino
                {
                    Name = name,
                    PositionX = (int)pos.x,
                    PositionY = (int)pos.y
                };

                saveGame.Minos.Add(savedMino);
            }
        }

        savedGames[slot - 1] = saveGame;
        SaveSystem.SaveGame(savedGames[slot - 1], slot);
        ActivateSaveSlot(slot);
    }

    /// <summary>
    /// Activates the save slot
    /// </summary>
    /// <param name="slot">The slot to activate</param>
    private void ActivateSaveSlot(int slot)
    {
        if (slot == 1)
        {
            Button deleteButton = slotOneButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");
            TMP_Text timeStampText = slotOneButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");

            deleteButton.gameObject.SetActive(true);
            timeStampText.text = savedGames[slot - 1].TimeStamp.ToString(CultureInfo.CurrentCulture);
        }
        else if (slot == 2)
        {
            Button deleteButton = slotTwoButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");
            TMP_Text timeStampText = slotTwoButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");

            deleteButton.gameObject.SetActive(true);
            timeStampText.text = savedGames[slot - 1].TimeStamp.ToString(CultureInfo.CurrentCulture);
        }
        else if (slot == 3)
        {
            Button deleteButton = slotThreeButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");
            TMP_Text timeStampText = slotThreeButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");

            deleteButton.gameObject.SetActive(true);
            timeStampText.text = savedGames[slot - 1].TimeStamp.ToString(CultureInfo.CurrentCulture);
        }
    }

    /// <summary>
    /// Deletes the game saved on the save slot
    /// </summary>
    /// <param name="slot">The save slot to delete</param>
    public void DeleteSave(int slot)
    {
        audioSource.Play();
        SaveSystem.DeleteSaveGame(slot);
        savedGames[slot - 1] = null;

        if (slot == 1)
        {
            Button deleteButton = slotOneButton.GetComponentsInChildren<Button>().FirstOrDefault(x => x.gameObject.name == "DeleteButton");
            TMP_Text timeStampText = slotOneButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");

            deleteButton.gameObject.SetActive(false);
            timeStampText.text = string.Empty;
        }
        else if (slot == 2)
        {
            Button deleteButton = slotTwoButton.GetComponentsInChildren<Button>().FirstOrDefault(x => x.gameObject.name == "DeleteButton");
            TMP_Text timeStampText = slotTwoButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");

            deleteButton.gameObject.SetActive(false);
            timeStampText.text = string.Empty;
        }
        else if (slot == 3)
        {
            Button deleteButton = slotThreeButton.GetComponentsInChildren<Button>().FirstOrDefault(x => x.gameObject.name == "DeleteButton");
            TMP_Text timeStampText = slotThreeButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");

            deleteButton.gameObject.SetActive(false);
            timeStampText.text = string.Empty;
        }
    }

    /// <summary>
    /// Goes back to the pause menu
    /// </summary>
    public void Back()
    {
        audioSource.Play();
        saveMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
}
