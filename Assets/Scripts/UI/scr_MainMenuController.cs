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

    public Toggle toggle;

    [SerializeField]
    AudioSource audioSource;

    private void Start()
    {
        float settings = PlayerPrefs.GetFloat("Audio");
        slider.value = settings;
    }
    private void Update()
    {
        audioSource.volume = slider.value;
        PlayerPrefs.SetFloat("Audio", slider.value);
        PlayerPrefs.SetInt("Graphics", toggle.isOn ? 1 : 0);
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
