using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterButton : MonoBehaviour
{
    Encounter m_encounter;
    GameObject m_dungeonMapPanel;

    public void Initialize(Encounter encounter, GameObject dungeonMapPanel)
    {
        m_encounter = encounter;
        m_dungeonMapPanel = dungeonMapPanel;
    }

    public void LoadEncounter()
    {
        Board.Current.SetupEncounter(m_encounter);
        m_dungeonMapPanel.SetActive(false);
    }
}
