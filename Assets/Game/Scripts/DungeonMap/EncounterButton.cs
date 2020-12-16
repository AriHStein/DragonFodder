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

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    public void Initialize(Encounter encounter, Board board)
    {
        m_encounter = encounter;
        m_board = board;
    }

    public void LoadEncounter()
    {
        m_board.SetupEncounter(m_encounter);
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
