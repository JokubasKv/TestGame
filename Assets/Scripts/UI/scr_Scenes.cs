using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_Scenes
{
    public static void RestartScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public static void LoadNextScene() {
        LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public static void LoadPreviousScene() {
        LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public static void ExitGame() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    private static void LoadScene(int buildIndex) {
        SceneManager.LoadScene(buildIndex);
    }
}
