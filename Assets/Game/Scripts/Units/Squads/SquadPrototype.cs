using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SquadProtoype", menuName = "Units/SquadPrototype", order = 111)]
public class SquadPrototype : ScriptableObject
{
    public string Type;
    [System.Serializable]
    public class UnitArray
    {
        public UnitPrototype[] Items;

        public int Length { get { return Items.Length; } }

        public UnitArray(int size)
        {
            Items = new UnitPrototype[size];
        }

        public UnitPrototype this[int i]
        {
            get { return Items[i]; }
            set { Items[i] = value; }
        }

    }
    public UnitArray[] Units;
    public Vector2Int Size;
    public Faction Faction;
    public bool IsBoss;

    public int Difficulty { get; protected set; }


    public void UpdateSize()
    {
        if(Units != null && 
            Size.x == Units.Length &&
            Size.y == Units[0].Length)
        {
            return;
        }
        
        UnitArray[] newUnits = new UnitArray[Size.x];
        for(int x = 0; x < Size.x; x++)
        {
            newUnits[x] = new UnitArray(Size.y);
        }

        if(Units != null && Units.Length > 0 && Units[0] != null)
        {
            int xMax = Mathf.Min(Units.Length, Size.x);
            int yMax = Mathf.Min(Units[0].Length, Size.y);

            for(int x = 0; x < xMax; x++)
            {
                for(int y = 0; y < yMax; y++)
                {
                    newUnits[x][y] = Units[x][y];
                }
            }
        }

        Units = newUnits;
    }

    private void OnValidate()
    {
        UpdateSize();
        UpdateDifficulty();
        if(Type != null)
        {
            SquadPrototypeLookup.AddPrototype(this);
        }
    }

    void UpdateDifficulty()
    {
        Difficulty = 0;
        if (Units == null)
        {
            return;
        }

        for(int x = 0; x < Units.Length; x++)
        {
            for(int y = 0; y < Units[0].Length; y++)
            {
                if(Units[x] != null && Units[x][y] != null)
                {
                    Difficulty += Units[x][y].Difficulty;
                }
            }
        }
    }
}
