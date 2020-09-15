using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterPlayModeButton : MonoBehaviour
{
    [SerializeField] PlayMode m_mode = default;
    
    public void OnButtonPressed()
    {
        Board.Current.EnterPlayMode(m_mode);
    }
}
