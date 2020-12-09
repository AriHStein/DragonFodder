using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterPlayModeButton : MonoBehaviour
{
    [SerializeField] PlayMode m_mode = default;
    Board_Base m_board;

    private void Start()
    {
        m_board = FindObjectOfType<Board_Base>();
    }

    public void OnButtonPressed()
    {
        m_board.EnterPlayMode(m_mode);
    }
}
