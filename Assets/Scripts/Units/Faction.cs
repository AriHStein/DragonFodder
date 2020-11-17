using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction { Player, Enemy }
public static class FactionExtensions
{
    public static Faction Opposite(this Faction faction)
    {
        if (faction == Faction.Enemy)
        {
            return Faction.Player;
        }

        return Faction.Enemy;
    }
}