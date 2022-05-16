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

    [SerializeField]
    Button settingsButton;

    [SerializeField]
    private RectTransform mainMenu;

    [SerializeField]
    private RectTransform settingsMenu;

    public Toggle toggle;

    public bool paused = false;

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
        //Show(mainMenu);
        Show(resumeButton);
        //Show(settingsButton);
        Show(exitButton);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void UnpauseGame()
    {
        Time.timeScale = 1;
        paused = false;
        //Hide(mainMenu);
        Hide(resumeButton);
        //Hide(settingsButton);
        Hide(exitButton);
        //Hide(settingsMenu);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void ExitGame()
    {
        Time.timeScale = 1;
        paused = false;
        scr_Scenes.LoadPreviousScene();
    }

    public void ShowSettingsMenu()
    {
        Show(settingsMenu);
        Hide(mainMenu);
    }
    public void ShowMainMenu()
    {
        Show(mainMenu);
        Hide(settingsMenu);
    }

    public void GraphicsToggled()
    {
        PlayerPrefs.SetInt("Graphics", toggle.isOn ? 1 : 0);
    }
}
