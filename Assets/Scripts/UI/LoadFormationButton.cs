using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadFormationButton : MonoBehaviour
{
    [SerializeField] FormationSizeSlider m_formationSizeSlider = default;
    Board m_board;

    private void Start()
    {
        m_board = FindObjectOfType<Board>();
    }


    public void LoadFormation()
    {
        m_board.LoadAndPlaceEnemyFormation(m_formationSizeSlider.FormationSize);
    }
}
