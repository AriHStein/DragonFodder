using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadFormationButton : MonoBehaviour
{
    [SerializeField] FormationSizeSlider m_formationSizeSlider;

    public void LoadFormation()
    {
        Board.Current.LoadAndPlaceEnemyFormation(m_formationSizeSlider.FormationSize);
    }
}
