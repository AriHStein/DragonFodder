﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BoardSize : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_xSizeText = default;
    [SerializeField] TextMeshProUGUI m_ySizeText = default;
    [SerializeField] Slider m_sliderX = default;
    [SerializeField] Slider m_sliderY = default;

    Vector2Int m_boardSize;
    Board m_board;

    private void Start()
    {
        m_board = FindObjectOfType<Board>();
        m_board.PlayModeChangedEvent += (mode) => {
            if (mode == PlayMode.SquadEditor)
                OnSquadModeEntered();
                };
    }

    bool m_ignoreSliderValueChanges = false;
    void OnSquadModeEntered()
    {
        m_ignoreSliderValueChanges = true;
        m_boardSize = new Vector2Int(m_board.Squares.GetLength(0), m_board.Squares.GetLength(1));
        m_sliderX.value = m_boardSize.x;
        m_sliderY.value = m_boardSize.y;
        m_xSizeText.text = m_boardSize.x.ToString();
        m_ySizeText.text = m_boardSize.y.ToString();
        m_ignoreSliderValueChanges = false;
        UpdateValues(false);
    }

    public void UpdateValues(bool changeBoard = true)
    {
        if (m_ignoreSliderValueChanges)
        {
            return;
        }

        m_boardSize.x = (int)m_sliderX.value;
        m_boardSize.y = (int)m_sliderY.value;

        m_xSizeText.text = m_boardSize.x.ToString();
        m_ySizeText.text = m_boardSize.y.ToString();
        if(changeBoard)
        {
            m_board.SetupSquares(m_boardSize.x, m_boardSize.y, true);
        }
    }
}
