using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    public void PlayAgain()
    {
        SceneManager.LoadScene("Level");
        Game.CurrentScore = 0;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
