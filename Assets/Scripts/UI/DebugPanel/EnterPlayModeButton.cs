using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterPlayModeButton : MonoBehaviour
{
    [SerializeField] PlayMode m_mode = default;
    Board m_board;

    private void Start()
    {
        m_board = FindObjectOfType<Board>();
    }

    public void OnButtonPressed()
    {
        m_board.EnterPlayMode(m_mode);
    }
}
