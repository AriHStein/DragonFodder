using System.Collections.Generic;
using UnityEngine;

public class Board_Debugging : Board_Base
{
    [System.Serializable]
    public class UnitPositionPair
    {
        public UnitPrototype Unit;
        public Vector2Int Position;
    }

    [SerializeField] List<UnitPositionPair> m_playerUnits = default;
    [SerializeField] List<UnitPositionPair> m_enemyUnits = default;

    protected override void StartGame()
    {
        SetupBoardState();
        EnterPlayMode(PlayMode.Battle);
    }

    void SetupBoardState()
    {
        ClearBoard();
        SetupSquares(m_defaultBoardSize.x, m_defaultBoardSize.y);

        foreach(UnitPositionPair pair in m_playerUnits)
        {
            TryPlaceUnit(new UnitData(pair.Unit, Faction.Player), pair.Position);
        }

        foreach(UnitPositionPair pair in m_enemyUnits)
        {
            TryPlaceUnit(new UnitData(pair.Unit, Faction.Enemy), pair.Position);
        }
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
    }
}
