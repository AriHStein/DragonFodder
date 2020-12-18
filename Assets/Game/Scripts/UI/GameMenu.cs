using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [SerializeField] SceneReference MainMenuScene = default;

    public void ShowMenu()
    {
        gameObject.SetActive(true);
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(MainMenuScene);
    }
}
