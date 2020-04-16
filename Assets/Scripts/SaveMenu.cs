using System;
using System.Collections.Generic;
using System.Linq;
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
    private Button slotTemplateButton;

    private SavedGame[] tempSavedGames;
    private AudioSource audioSource;

    private void Start()
    {
        tempSavedGames = new SavedGame[SlotButtons.SaveSlots];
        audioSource = GetComponent<AudioSource>();
        GetSaves();
    }

    /// <summary>
    /// Gets all the saved games and activates/deactivates the save buttons
    /// </summary>
    public void GetSaves(bool useLoadedSaves = true)
    {
        var (buttons, savedGames) = SlotButtons.GetSaves(slotTemplateButton, tempSavedGames, true);

        foreach (Button slotButton in buttons)
        {
            int index = buttons.IndexOf(slotButton);
            Button deleteButton = buttons[index].GetComponentsInChildren<Button>(true).FirstOrDefault(x => x.gameObject.name == "DeleteButton");
            SavedGame savedGame = null;

            if (useLoadedSaves)
                savedGame = savedGames.FirstOrDefault(x => x.Slot == index + 1);
            else if (tempSavedGames[index] != null)
                savedGame = tempSavedGames[index];

            slotButton.onClick.AddListener(() => SaveGame(index + 1));

            if (savedGame != null)
                deleteButton.onClick.AddListener(() => DeleteSave(index + 1));
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

        SavedGame saveGame = new SavedGame(slot, DateTime.Now, Game.Instance.Name, Game.Instance.CurrentScore, Game.Instance.TotalLinesCleared, currentTetromino, nextTetromino, savedTetromino);

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

        if (!SaveSystem.SaveGame(saveGame, slot))
            return;

        tempSavedGames[slot - 1] = saveGame;
        GetSaves(false);
    }

    /// <summary>
    /// Deletes the game saved on the save slot
    /// </summary>
    /// <param name="slot">The save slot to delete</param>
    public void DeleteSave(int slot)
    {
        audioSource.Play();
        if (!SaveSystem.DeleteSaveGame(slot))
            return;

        tempSavedGames[slot - 1] = null;
        GetSaves(false);
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
