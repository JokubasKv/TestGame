using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class scr_GameOver : MonoBehaviour
{
    [SerializeField]
    Button resumeButton;

    [SerializeField]
    Button exitButton;

    bool paused = false;

    private void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if (paused) UnpauseGame();
            else PauseGame();
        }
    }
    private static void Show(Component c)
    {
        c.gameObject.SetActive(true);
    }
    private static void Hide(Component c)
    {
        c.gameObject.SetActive(false);
    }
    public void RestartGame()
    {
        scr_Scenes.RestartScene();
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        paused = true;
        Show(resumeButton);
        Show(exitButton);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void UnpauseGame()
    {
        Time.timeScale = 1;
        paused = false;
        Hide(resumeButton);
        Hide(exitButton);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void ExitGame()
    {
        Time.timeScale = 1;
        paused = false;
        scr_Scenes.LoadPreviousScene();
    }
    // Update is called once per frame
}
