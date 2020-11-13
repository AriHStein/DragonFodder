using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EncounterButton : MonoBehaviour
{
    Encounter m_encounter;
    Button m_button;
    Board m_board;
    //GameObject m_dungeonMapPanel;

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    public void Initialize(Encounter encounter, Board board)
    {
        m_encounter = encounter;
        m_board = board;
        //m_dungeonMapPanel = dungeonMapPanel;
    }

    public void LoadEncounter()
    {
        m_board.SetupEncounter(m_encounter);
        //Board.Current.SetupEncounter(m_encounter);
        //m_dungeonMapPanel.SetActive(false);
    }

    public void EnableButton()
    {
        if(m_button == null)
        {
            m_button = GetComponent<Button>();
        }
        
        m_button.interactable = true;
    }

    public void DisableButton()
    {
        if (m_button == null)
        {
            m_button = GetComponent<Button>();
        }

        m_button.interactable = false;
    }
}
