using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public string fileName;
    public bool GameOver;
    public int Gold;
    public List<UnitData> CurrentUnits;
    public List<RecruitableUnitCollection> RecruitableUnits;
    public List<Encounter> Encounters;
}
