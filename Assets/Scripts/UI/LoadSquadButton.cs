using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadSquadButton : MonoBehaviour
{
    //[SerializeField] bool m_mirrorBoard = false;
    [SerializeField] string m_defaultSquadName = default;
    [SerializeField] string m_saveDirectory = default;

    //[SerializeField] TextMeshProUGUI m_squadNameInputText = default;
    [SerializeField] TMP_InputField m_inputField = default;

    public void LoadSquad()
    {

        string squadName;
        if (m_inputField == null || m_inputField.text == null || m_inputField.text == "") 
        {
            squadName = m_defaultSquadName; 
        }
        else
        {
            squadName = m_inputField.text;
        }

        SquadData squad = UnitSaveLoadUtility.LoadSquad(squadName, m_saveDirectory);
        if(squad.Units == null || squad.Units.Count == 0)
        {
            return;
        }

        Vector2Int boardSize = squad.Size + squad.SquadOrigin + Vector2Int.one;
        Board.Current.SetupSquares(boardSize.x, boardSize.y);
        Board.Current.ClearUnits();
        Board.Current.TryPlaceSquad(squad, Vector2Int.zero, false);
    }
}
