using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EncounterButton : MonoBehaviour
{
    Encounter m_encounter;
    Button m_button;
    //GameObject m_dungeonMapPanel;

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    public void Initialize(Encounter encounter)
    {
        m_encounter = encounter;
        //m_dungeonMapPanel = dungeonMapPanel;
    }

    public void LoadEncounter()
    {
        Board.Current.SetupEncounter(m_encounter);
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
