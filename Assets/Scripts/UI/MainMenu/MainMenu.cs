﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    const int BATTLE_SCENE_BUILD_INDEX = 1;
    [SerializeField] GameState m_currentGameState = default;

    [SerializeField] Button m_continueButton = default;

    public string FileName;
    const string CONTINUE_SAVEFILE_NAME = "AutoSave";

    private void Start()
    {
        m_continueButton.interactable = SaveLoadUtility.SaveFileExists(CONTINUE_SAVEFILE_NAME);
    }

    public void Continue()
    {
        Debug.Log("Continue");
        m_currentGameState.Data = SaveLoadUtility.LoadGame(CONTINUE_SAVEFILE_NAME);
        SceneManager.LoadScene(BATTLE_SCENE_BUILD_INDEX);
    }

    //public void NewGame()
    //{
    //    Debug.Log("New Game");
    //    m_currentGameState.LoadDefaultState();
    //    SceneManager.LoadScene(BATTLE_SCENE_BUILD_INDEX);
    //}

    //public void LoadGame()
    //{
    //    Debug.Log("Load Game");
    //    m_currentGameState.Data = SaveLoadUtility.LoadGame(FileName);
    //    SceneManager.LoadScene(BATTLE_SCENE_BUILD_INDEX);
    //}

    public void StartGame(string saveFileName)
    {
        if(SaveLoadUtility.SaveFileExists(saveFileName))
        {
            m_currentGameState.Data = SaveLoadUtility.LoadGame(saveFileName);
        }
        else
        {
            m_currentGameState.LoadDefaultState();
            m_currentGameState.Data.fileName = saveFileName;
            m_currentGameState.SaveGame();
        }

        SceneManager.LoadScene(BATTLE_SCENE_BUILD_INDEX);
    }
}
