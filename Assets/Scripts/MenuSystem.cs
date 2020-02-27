using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    /// <summary>
    /// Pretty self explanatory
    /// </summary>
    public void PlayAgain()
    {
        SceneManager.LoadScene("Level");
        Game.CurrentScore = 0;
    }

    /// <summary>
    /// Pretty self explanatory
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
}
