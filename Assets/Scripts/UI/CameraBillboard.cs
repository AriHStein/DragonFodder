using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBillboard : MonoBehaviour
{
    [SerializeField] bool m_billboardX = true;
    [SerializeField] bool m_billboardY = true;
    [SerializeField] bool m_billboardZ = true;

    [SerializeField] float m_distanceFromCamera = 1f;
    protected Vector3 m_localStartPosition;

    // Start is called before the first frame update
    void Start()
    {
        m_localStartPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

        if(!m_billboardX || !m_billboardY || !m_billboardZ)
        {
            transform.rotation = Quaternion.Euler(
                m_billboardX ? transform.rotation.eulerAngles.x : 0f,
                m_billboardY ? transform.rotation.eulerAngles.y : 0f,
                m_billboardZ ? transform.rotation.eulerAngles.z : 0f);
        }

        transform.localPosition = m_localStartPosition;
        transform.position += transform.rotation * Vector3.forward * m_distanceFromCamera;
    }
}
