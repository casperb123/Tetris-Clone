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

    private Game game;
    private SavedGame[] savedGames;
    private AudioSource audioSource;

    private void Start()
    {
        game = Game.Instance;
        savedGames = new SavedGame[] { null, null, null };
        audioSource = GetComponent<AudioSource>();

        if (SaveSystem.DoesSaveGameExist("slot1"))
        {
            var (isValid, game) = SaveSystem.LoadGame("slot1");

            if (isValid)
            {
                TMP_Text timeStampText = slotOneButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");
                Button deleteButton = slotOneButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");

                savedGames[0] = game as SavedGame;
                timeStampText.text = savedGames[0].TimeStamp.Value.ToString(CultureInfo.CurrentCulture);
                deleteButton.gameObject.SetActive(true);
            }
        }
        if (SaveSystem.DoesSaveGameExist("slot2"))
        {
            var (isValid, game) = SaveSystem.LoadGame("slot1");

            if (isValid)
            {
                TMP_Text timeStampText = slotTwoButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");
                Button deleteButton = slotTwoButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");

                savedGames[1] = game as SavedGame;
                timeStampText.text = savedGames[1].TimeStamp.Value.ToString(CultureInfo.CurrentCulture);
                deleteButton.gameObject.SetActive(true);
            }
        }
        if (SaveSystem.DoesSaveGameExist("slot3"))
        {
            var (isValid, game) = SaveSystem.LoadGame("slot1");

            if (isValid)
            {
                TMP_Text timeStampText = slotThreeButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");
                Button deleteButton = slotThreeButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");

                savedGames[2] = game as SavedGame;
                timeStampText.text = savedGames[2].TimeStamp.Value.ToString(CultureInfo.CurrentCulture);
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
        SavedTetromino currentTetromino = new SavedTetromino(game.CurrentTetromino.name.Replace("(Clone)", ""), game.CurrentTetromino.transform.position.x, game.CurrentTetromino.transform.position.y, (int)game.CurrentTetromino.transform.rotation.eulerAngles.z);
        SavedTetromino nextTetromino = new SavedTetromino(game.NextTetromino.name.Replace("(Clone)", ""));
        SavedTetromino savedTetromino = null;

        if (game.SavedTetromino != null)
            savedTetromino = new SavedTetromino(game.SavedTetromino.name.Replace("(Clone)", ""));

        SavedGame saveGame = new SavedGame
        {
            Score = game.CurrentScore,
            Lines = game.TotalLinesCleared,
            CurrentTetromino = currentTetromino,
            NextTetromino = nextTetromino,
            SavedTetromino = savedTetromino
        };

        for (int x = 0; x < game.GridWidth; x++)
        {
            for (int y = 0; y < game.GridHeight; y++)
            {
                Transform mino = game.Grid[x, y];
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

                Vector2 pos = game.Round(mino.position);

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
        SaveSystem.SaveGame(savedGames[slot - 1], $"slot{slot}");
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
            timeStampText.text = savedGames[slot - 1].TimeStamp.Value.ToString(CultureInfo.CurrentCulture);
        }
        else if (slot == 2)
        {
            Button deleteButton = slotTwoButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");
            TMP_Text timeStampText = slotTwoButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");

            deleteButton.gameObject.SetActive(true);
            timeStampText.text = savedGames[slot - 1].TimeStamp.Value.ToString(CultureInfo.CurrentCulture);
        }
        else if (slot == 3)
        {
            Button deleteButton = slotThreeButton.GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");
            TMP_Text timeStampText = slotThreeButton.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "TimestampText");

            deleteButton.gameObject.SetActive(true);
            timeStampText.text = savedGames[slot - 1].TimeStamp.Value.ToString(CultureInfo.CurrentCulture);
        }
    }

    /// <summary>
    /// Deletes the game saved on the save slot
    /// </summary>
    /// <param name="slot">The save slot to delete</param>
    public void DeleteSave(int slot)
    {
        audioSource.Play();
        string slotName = $"slot{slot}";
        SaveSystem.DeleteSaveGame(slotName);
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
