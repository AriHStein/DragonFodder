using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveSquadButton : MonoBehaviour
{

    [SerializeField] bool m_mirrorBoard = false;
    [SerializeField] string m_squadName = default;
    [SerializeField] string m_saveDirectory = default;

    [SerializeField] TMP_InputField m_inputField = default;

    public void SaveSquad()
    {
        Board board = FindObjectOfType<Board>();
        if(m_inputField == null || m_inputField.text == null || m_inputField.text == "")
        {
            board.SaveBoardAsSquad(m_squadName, m_saveDirectory, m_mirrorBoard);
        }
        else
        {
            board.SaveBoardAsSquad(m_inputField.text, m_saveDirectory, m_mirrorBoard);
        }
    }
}
