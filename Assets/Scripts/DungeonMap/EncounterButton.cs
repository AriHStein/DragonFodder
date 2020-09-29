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

    private void Start()
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
        m_button.interactable = true;
    }

    public void DisableButton()
    {
        m_button.interactable = false;
    }
}
