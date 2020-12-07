using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RecruitUnitsPanel : MonoBehaviour
{
    const int BATTLE_SCENE_INDEX = 2;
    [SerializeField] GameState m_currentGameState = default;

    [SerializeField] Transform m_availablePanel = default;
    [SerializeField] Transform m_currentPanel = default;

    [SerializeField] GameObject m_recruitablePrefab = default;
    [SerializeField] GameObject m_currentPrefab = default;

    [SerializeField] TextMeshProUGUI m_goldText = default;

    List<CurrentUnitItem> m_currentUnits;
    List<RecruitableUnitItem> m_recruitableUnits;

    int m_currentGold;

    private void Start()
    {
        SetupPanel();
    }

    void SetupPanel()
    {
        ClearPanel();
        foreach(UnitData unit in m_currentGameState.Data.CurrentUnits)
        {
            AddCurrentUnit(unit);
        }

        foreach(RecruitableUnitCollection recruitableUnits in m_currentGameState.Data.RecruitableUnits)
        {
            for(int i = 0; i < recruitableUnits.Count; i++)
            {
                AddRecruitableUnit(recruitableUnits.Proto);
            }
        }

        m_currentGold = m_currentGameState.Data.Gold;
        m_goldText.text = m_currentGold.ToString() + " Gold";
    }

    void ClearPanel()
    {
        if(m_currentUnits != null)
        {
            foreach(CurrentUnitItem item in m_currentUnits)
            {
                Destroy(item.gameObject);
            }
        }

        if(m_recruitableUnits != null)
        {
            foreach(RecruitableUnitItem item in m_recruitableUnits)
            {
                Destroy(item.gameObject);
            }
        }

        m_currentUnits = new List<CurrentUnitItem>();
        m_recruitableUnits = new List<RecruitableUnitItem>();
    }

    void AddCurrentUnit(UnitData unit)
    {
        GameObject go = Instantiate(m_currentPrefab, m_currentPanel);
        CurrentUnitItem item = go.GetComponent<CurrentUnitItem>();
        item.Initialize(unit, this);
        m_currentUnits.Add(item);
    }

    void AddRecruitableUnit(UnitPrototype proto)
    {
        GameObject go = Instantiate(m_recruitablePrefab, m_availablePanel);
        RecruitableUnitItem item = go.GetComponent<RecruitableUnitItem>();
        item.Initialize(proto, this);
        m_recruitableUnits.Add(item);
    }

    public void RecruitUnit(RecruitableUnitItem item)
    {
        if(m_currentGold < item.Unit.RecruitCost)
        {
            Debug.Log($"Not enough gold!");
            return;
        }

        m_currentGold -= item.Unit.RecruitCost;
        m_goldText.text = m_currentGold.ToString() + " Gold";

        UnitData newUnit = new UnitData(item.Unit, Faction.Player);
        AddCurrentUnit(newUnit);
        m_recruitableUnits.Remove(item);
        Destroy(item.gameObject);
    }

    public void EnterDungeon()
    {
        List<UnitData> currentUnits = new List<UnitData>();
        foreach(CurrentUnitItem item in m_currentUnits)
        {
            currentUnits.Add(item.Unit);
        }

        Dictionary<UnitPrototype, int> recruitableUnits = new Dictionary<UnitPrototype, int>();
        foreach(RecruitableUnitItem item in m_recruitableUnits)
        {
            if(!recruitableUnits.ContainsKey(item.Unit))
            {
                recruitableUnits[item.Unit] = 0;
            }

            recruitableUnits[item.Unit]++;
        }

        List<RecruitableUnitCollection> recruitableCollection = new List<RecruitableUnitCollection>();
        foreach(UnitPrototype proto in recruitableUnits.Keys)
        {
            recruitableCollection.Add(new RecruitableUnitCollection(proto, recruitableUnits[proto]));
        }

        m_currentGameState.Data.CurrentUnits = currentUnits;
        m_currentGameState.Data.RecruitableUnits = recruitableCollection;
        m_currentGameState.Data.Gold = m_currentGold;

        m_currentGameState.SaveGame();

        SceneManager.LoadScene(BATTLE_SCENE_INDEX);
    }
}
