using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] SceneReference m_recruitUnitsScene = default;
    //const int RECRUIT_UNITS_SCENE_INDEX = 1;
    [SerializeField] GameState m_currentGameState = default;

    [SerializeField] Button m_continueButton = default;

    public string FileName;

    private void Start()
    {
        RefreshPanel();
    }

    public void RefreshPanel()
    {
        m_continueButton.interactable = SaveLoadUtility.SaveFileExists(PrefKeys.ContinueFileName);
    }

    public void Continue()
    {
        m_currentGameState.Data = SaveLoadUtility.LoadGame(PrefKeys.ContinueFileName);
        if(m_currentGameState.Data.GameOver)
        {
            Debug.LogError("Loaded a game that is Game Over.");
            return;
        }
        SceneManager.LoadScene(m_recruitUnitsScene);
    }

    public void StartGame(string saveFileName)
    {
        if(SaveLoadUtility.SaveFileExists(saveFileName))
        {
            m_currentGameState.Data = SaveLoadUtility.LoadGame(saveFileName);
        }
        else
        {
            m_currentGameState.LoadNewGameState();
            m_currentGameState.Data.fileName = saveFileName;
            m_currentGameState.SaveGame();
        }

        SceneManager.LoadScene(m_recruitUnitsScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
