using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public class Board : Board_Base
{
    [SerializeField] GameState m_gameState = default;

    [SerializeField] UnitPlacementMenu m_unitPlacementModePanel = default;
    [SerializeField] GameObject m_gameOverPanel = default;
    [SerializeField] DungeonMap m_dungeonMap = default;
    [SerializeField] TMPro.TextMeshProUGUI m_currentGoldText = default;
    
    Encounter m_currentEncounter;
    int m_currentGold;

    [SerializeField] Vector2Int m_defaultSquadSize = Vector2Int.one;

    [SerializeField] float m_endOfBattleDelayLength = 5f;
    const string m_playerSquadsDirectoryName = "Player";
    const string m_defaultPlayerSquadFileName = "PlayerDefault";
    const string m_currentPlayerSquadFileName = "PlayerCurrent";

    protected void OnDisable()
    {
        m_gameState.Clear();
    }

    public void SetupEncounter(Encounter encounter)
    {
        if(encounter == null)
        {
            Debug.LogError("Encounter is null.");
            return;
        }

        m_currentEncounter = encounter;

        SetupSquares(encounter.BoardSize.x, encounter.BoardSize.y, false);
        TryPlaceSquad(encounter.Enemies, Vector2Int.zero, true);
        SetSquaresInteractable(Squares.GetLength(0), encounter.RowsAllowedForPlayerUnits);

        EnterPlayMode(PlayMode.UnitPlacement);
    }

    protected override void StartGame()
    {
        if(m_gameState.Data == null || m_gameState.Data.fileName == null || m_gameState.Data.fileName == "")
        {
            m_gameState.LoadQuickPlayState();
        }
        
        m_unitPlacementModePanel.Deactivate();
        m_gameOverPanel.SetActive(false);
        ClearBoard();

        ChangeGold(-m_currentGold);
        ChangeGold(m_gameState.Data.Gold);

        m_dungeonMap.SetupMap(this);
        EnterPlayMode(PlayMode.Dungeon);
    }

    public bool ChangeGold(int amount)
    {
        int newAmount = m_currentGold + amount;
        if(newAmount < 0)
        {
            return false;
        }

        m_currentGold = newAmount;
        UpdateGoldText();
        return true;
    }

    void UpdateGoldText()
    {
        m_currentGoldText.text = "Gold: " + m_currentGold.ToString();
    }

    #region PlayMode Changes
    public override void EnterPlayMode(PlayMode mode)
    {
        if(PlayMode == mode)
        {
            Debug.Log($"Already in {mode}.");
            return;
        }

        switch (mode)
        {
            case PlayMode.Battle:
                UnitManager.UnplacedUnits.AddRange(m_unitPlacementModePanel.GetUnplacedUnits());
                
                m_unitPlacementModePanel.Deactivate();
                m_gameOverPanel.SetActive(false);
                m_dungeonMap.Deactivate();
                break;

            case PlayMode.UnitPlacement:
                m_unitPlacementModePanel.Activate(
                    m_gameState.Data.CurrentUnits
                    );

                m_gameOverPanel.SetActive(false);
                m_dungeonMap.Deactivate();
                break;

            case PlayMode.Paused:
                break;

            case PlayMode.Dungeon:
                m_unitPlacementModePanel.Deactivate();
                m_gameOverPanel.SetActive(false);
                m_dungeonMap.Activate();
                break;

            default:
                Debug.LogError($"Enter PlayMode {mode} not implemented.");
                break;
        }

        base.EnterPlayMode(mode);
    }

    protected override void OnBattleComplete(Faction winner)
    {
        if (PlayMode != PlayMode.Battle)
        {
            Debug.Log("Exit Battle Mode called while not in BattleMode. Most likely, the battle ended with all units dead.");
            return;
        }

        UnitManager.TriggerVictoryAnimations();
        EnterPlayMode(PlayMode.Paused);
        if (winner == Faction.Player)
        {
            Invoke(nameof(BattleWon), m_endOfBattleDelayLength);
        }
        else
        {
            Invoke(nameof(BattleLost), m_endOfBattleDelayLength);
        }
    }

    void BattleWon()
    {
        ChangeGold(m_currentEncounter.Reward);

        UpdateSaveFile();
        m_dungeonMap.ExitEncounter(m_currentEncounter, true);
        EnterPlayMode(PlayMode.Dungeon);
    }

    void BattleLost()
    {
        UpdateSaveFile();
        m_dungeonMap.ExitEncounter(m_currentEncounter, false);
        
        EnterPlayMode(PlayMode.Paused);
        m_gameOverPanel.SetActive(true);
    }
    #endregion

    #region Save/Load
    //Squad GetCurrentPlayerSquad()
    //{
    //    return UnitSaveLoadUtility.LoadSquad(m_currentPlayerSquadFileName, m_playerSquadsDirectoryName);
    //}

    //Squad GetDefaultPlayerSquad()
    //{
    //    return UnitSaveLoadUtility.LoadSquad(m_defaultPlayerSquadFileName, m_playerSquadsDirectoryName);
    //}

    //public Squad LoadAndPlaceEnemyFormation(int size, bool mirror = false)
    //{
    //    Squad formation = SquadBuilder.GenerateFormationFromEnemySquads(size);

    //    Vector2Int boardSize = new Vector2Int(
    //        Mathf.Max(formation.Size.x + 1, m_defaultBoardSize.x),
    //        Mathf.Max(formation.Size.y + 1, m_defaultBoardSize.y));

    //    SetupSquares(boardSize.x, boardSize.y);
    //    TryPlaceSquad(formation, Vector2Int.zero, mirror);

    //    return formation;
    //}

    //public Squad LoadSquadToBoard(string name, string directory, Vector2Int offset, bool resizeBoard = false, bool mirror = false)
    //{
    //    Squad squad = UnitSaveLoadUtility.LoadSquad(name, directory);
    //    if(resizeBoard)
    //    {
    //        SetupSquares(squad.Size.x + 1, squad.Size.y + 1);
    //    }
        
    //    TryPlaceSquad(squad, offset, mirror);

    //    return squad;
    //}

    //public void SaveBoardAsSquad(string name, string dir, bool mirrorBoard = true)
    //{
    //    Squad squad = GetBoardAsSquad(mirrorBoard);
    //    UnitSaveLoadUtility.SaveSquad(squad, name, dir);
    //}

    void UpdateSaveFile()
    {
        if(m_gameState == null)
        {
            Debug.LogError($"No game state!");
            return;
        }

        List<Unit> units = UnitManager.Units;
        List<UnitData> unitDatas = new List<UnitData>();
        foreach(Unit unit in units)
        {
            unitDatas.Add(new UnitData(unit));
        }

        unitDatas.AddRange(UnitManager.UnplacedUnits);
        UnitManager.UnplacedUnits = new List<UnitData>();

        m_gameState.Data.CurrentUnits = unitDatas;
        m_gameState.Data.Gold = m_currentGold;
    }
    #endregion
}
