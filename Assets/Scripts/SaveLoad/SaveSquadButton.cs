using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSquadButton : MonoBehaviour
{

    [SerializeField] bool m_mirrorBoard = false;
    [SerializeField] string m_squadName = default;

    public void SaveSquad()
    {
        Board.Current.SaveBoardAsSquad(m_squadName, m_mirrorBoard);
    }
}
