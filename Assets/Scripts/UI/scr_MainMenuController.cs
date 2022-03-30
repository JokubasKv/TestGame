using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        scr_Scenes.LoadNextScene();
    }
    public void ExitGame()
    {
        scr_Scenes.ExitGame();
    }
}
