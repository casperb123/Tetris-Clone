using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotButtons : MonoBehaviour
{
    public static int SaveSlots = 10;

    public static (List<Button> buttons, List<SavedGame> savedGames) GetSaves(Button buttonTemplate, SavedGame[] savedGames = null, bool canDelete = true, bool hideIfNoSave = false)
    {
        List<Button> buttons = new List<Button>();
        List<SavedGame> tempSavedGames = new List<SavedGame>();

        if (buttonTemplate.transform.parent.childCount > 1)
            buttons = buttonTemplate.transform.parent.GetComponentsInChildren<Button>(true).Where(x => x != buttonTemplate && x.gameObject.name != "DeleteButton").ToList();
        else
        {
            for (int i = 0; i < SaveSlots; i++)
            {
                Button button = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
                TMP_Text buttonText = button.transform.GetChild(0).GetComponent<TMP_Text>();

                button.gameObject.name = $"Slot{i + 1}Button";
                buttonText.text = $"Slot {i + 1}";
                button.gameObject.SetActive(true);
                buttons.Add(button);
            }
        }

        for (int i = 0; i < SaveSlots; i++)
        {
            Button button = buttons[i];
            TMP_Text timeStampText = button.transform.GetChild(1).GetComponent<TMP_Text>();
            Button deleteButton = button.transform.GetChild(2).GetComponent<Button>();

            if (savedGames == null)
            {
                var (isValid, savedGame) = SaveSystem.LoadGame(i + 1);

                if (isValid)
                {
                    tempSavedGames.Add(savedGame);
                    timeStampText.text = savedGame.TimeStamp.ToString(CultureInfo.CurrentCulture);

                    if (canDelete)
                        deleteButton.gameObject.SetActive(true);
                    else
                        deleteButton.gameObject.SetActive(false);
                }
                else
                {
                    deleteButton.gameObject.SetActive(false);
                    timeStampText.text = "";

                    if (hideIfNoSave)
                        button.gameObject.SetActive(false);
                }
            }
            else
            {
                SavedGame savedGame = savedGames[i];

                if (savedGame != null)
                {
                    tempSavedGames.Add(savedGame);
                    timeStampText.text = savedGame.TimeStamp.ToString(CultureInfo.CurrentCulture);

                    if (canDelete)
                        deleteButton.gameObject.SetActive(true);
                    else
                        deleteButton.gameObject.SetActive(false);

                    button.gameObject.SetActive(true);
                }
                else
                {
                    deleteButton.gameObject.SetActive(false);
                    timeStampText.text = "";

                    if (hideIfNoSave)
                        button.gameObject.SetActive(false);
                }
            }
        }

        foreach (Button slotButton in buttons)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.transform.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
        }

        return (buttons, tempSavedGames);
    }
}
