using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_MainMenuController : MonoBehaviour
{
    [SerializeField]
    private RectTransform mainMenu;

    [SerializeField]
    private RectTransform settingsMenu;

    public Slider slider;

    [SerializeField]
    AudioSource audioSource;

    private void Start()
    {
        
    }
    private void Update()
    {
        audioSource.volume = slider.value;
    }
    private static void Show(Component c)
    {
        c.gameObject.SetActive(true);
    }
    private static void Hide(Component c)
    {
        c.gameObject.SetActive(false);
    }
    public void StartGame()
    {
        scr_Scenes.LoadNextScene();
    }
    public void ExitGame()
    {
        scr_Scenes.ExitGame();
    }
    public void ShowMainMenu()
    {
        Show(mainMenu);
        Hide(settingsMenu);
    }
    public void ShowSettingsMenu()
    {
        Show(settingsMenu);
        Hide(mainMenu);
    }
}
