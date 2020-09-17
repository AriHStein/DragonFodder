using System.Collections;
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

    private void Start()
    {
        Board.Current.PlayModeChangedEvent += (mode) => {
            if (mode == PlayMode.SquadEditor)
                OnSquadModeEntered();
                };
    }

    bool m_ignoreValueChanges = false;
    void OnSquadModeEntered()
    {
        m_ignoreValueChanges = true;
        m_sliderX.value = Board.Current.Squares.GetLength(0);
        m_sliderY.value = Board.Current.Squares.GetLength(1);
        m_ignoreValueChanges = false;
        UpdateValues(false);
    }

    public void UpdateValues(bool changeBoard = true)
    {
        if(m_ignoreValueChanges)
        {
            return;
        }

        m_boardSize.x = (int)m_sliderX.value;
        m_boardSize.y = (int)m_sliderY.value;

        m_xSizeText.text = m_boardSize.x.ToString();
        m_ySizeText.text = m_boardSize.y.ToString();
        if(changeBoard)
        {
            Board.Current.SetupSquares(m_boardSize.x, m_boardSize.y, true);
        }
    }
}
