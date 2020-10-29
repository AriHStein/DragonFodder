using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Threatened Condition", menuName = "Units/Conditions/Threatened", order = 122)]
public class Threatened : Condition
{
    public Faction Enemies;
    public int MaxEnemyDifficultyInRange;
    
    public override bool IsMet(Unit unit, Board board)
    {
        int threatLevel = 0;
        
        List<Unit> enemies = board.UnitManager.GetUnitsOfFaction(Enemies);
        foreach(Unit enemy in enemies)
        {
            if(enemy.CanTargetUnit(unit))
            {
                threatLevel += enemy.Proto.Difficulty;
            }
        }

        return threatLevel >= MaxEnemyDifficultyInRange;
    }

    public bool IsMetAt(BoardSquare square, Board board)
    {
        int threatLevel = 0;

        List<Unit> enemies = board.UnitManager.GetUnitsOfFaction(Enemies);
        foreach (Unit enemy in enemies)
        {
            if (enemy.CanTargetSquare(square))
            {
                threatLevel += enemy.Proto.Difficulty;
            }
        }

        return threatLevel >= MaxEnemyDifficultyInRange;
    }
}
