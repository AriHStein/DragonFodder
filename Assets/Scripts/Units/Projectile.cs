using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float m_travelTime = 0.2f;
    //[SerializeField] int m_damage = 1;

    Unit m_target;

    public void Initialize(Unit target)
    {
        if(target == null)
        {
            Destroy(gameObject);
            return;
        }
        
        m_target = target;
        m_targetPosition = target.transform.position + Vector3.up;
        m_startPosition = transform.position;
        m_timeElapsed = 0;
    }

    Vector3 m_startPosition;
    Vector3 m_targetPosition;
    float m_timeElapsed;
    // Update is called once per frame
    void Update()
    {
        m_timeElapsed += Time.deltaTime;
        if (m_timeElapsed >= m_travelTime)
        {
            HitTarget();
            return;
        }

        transform.position = Vector3.Lerp(m_startPosition, m_targetPosition, m_timeElapsed / m_travelTime);
    }

    void HitTarget()
    {
        //if(m_target != null)
        //{
        //    m_target.TakeDamage(m_damage);
        //}

        Destroy(gameObject);
    }
}
