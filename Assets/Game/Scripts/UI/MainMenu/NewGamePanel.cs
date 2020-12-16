using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewGamePanel : MonoBehaviour
{
    [SerializeField] MainMenu m_mainMenu = default;
    [SerializeField] string m_defaultFileName = "Game";
    [SerializeField] TMP_InputField m_inputField = default;

    public void ShowPanel()
    {
        m_inputField.text = "";
        gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        m_inputField.text = "";
        gameObject.SetActive(false);
        m_mainMenu.RefreshPanel();
    }

    public void StartGame()
    {
        if(m_inputField.text == null || m_inputField.text == "")
        {
            m_mainMenu.StartGame(m_defaultFileName);
        }
        else
        {
            m_mainMenu.StartGame(m_inputField.text);
        }
    }
}
