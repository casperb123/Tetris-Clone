using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    private Game game;

    private void Start()
    {
        game = Game.Instance;
    }

    /// <summary>
    /// Pretty self explanatory
    /// </summary>
    public void PlayAgain()
    {
        SceneManager.LoadScene("Level");
        game.CurrentScore = 0;
    }

    /// <summary>
    /// Pretty self explanatory
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
}
