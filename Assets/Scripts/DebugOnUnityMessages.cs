using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugOnUnityMessages : MonoBehaviour
{
    [SerializeField] string m_onAwake = default;
    [SerializeField] string m_onEnable = default;
    [SerializeField] string m_onDisable = default;
    [SerializeField] string m_onStart = default;
    [SerializeField] string m_onUpdate = default;
    [SerializeField] string m_onDestroy = default;

    private void Awake()
    {
        if(m_onAwake != null && m_onAwake != "")
            Debug.Log(m_onAwake);
    }

    void Start()
    {
        if (m_onStart != null && m_onStart != "")
            Debug.Log(m_onStart);
    }

    void OnEnable()
    {
        if (m_onEnable != null && m_onEnable != "")
            Debug.Log(m_onEnable);
    }

    void OnDisable()
    {
        if (m_onDisable != null && m_onDisable != "")
            Debug.Log(m_onDisable);
    }

    private void Update()
    {
        if (m_onUpdate != null && m_onUpdate != "")
            Debug.Log(m_onUpdate);
    }

    private void OnDestroy()
    {
        if (m_onDestroy != null && m_onDestroy != "")
            Debug.Log(m_onDestroy);
    }
}
