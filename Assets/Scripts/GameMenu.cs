using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [SerializeField]
    private Text levelText;

    public void PlayGame()
    {
        SceneManager.LoadScene("Level");
    }

    public void ChangedLevel(float level)
    {
        levelText.text = level.ToString();
    }
}
