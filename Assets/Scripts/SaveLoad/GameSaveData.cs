using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public string fileName;
    public int Gold;
    public List<UnitData> CurrentUnits;
    public List<UnitPrototype> RecruitableUnits;
    public List<Encounter> Encounters;
}
