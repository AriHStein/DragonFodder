using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile : MonoBehaviour
{
    [SerializeField] float m_travelTime = 0.2f;
    [SerializeField] float m_impactLength = 0.5f;
    [SerializeField] float m_spawnDelay = 0f;
    [SerializeField] VisualEffect m_spawnVFX = default;
    [SerializeField] VisualEffect m_travelVFX = default;
    [SerializeField] VisualEffect m_impactVFX = default;
    //[SerializeField] int m_damage = 1;

    public void Initialize(Transform target)
    {
        if(target == null)
        {
            Destroy(gameObject);
            return;
        }
        
        m_targetPosition = target.position + Vector3.up;
        m_startPosition = transform.position;
        m_timeElapsed = 0;
        m_travelStartTime = Time.time + m_spawnDelay;
        m_hitTime = m_travelStartTime + m_travelTime;

        if(m_spawnVFX != null)
            m_spawnVFX.Play();

        if(m_travelVFX != null)
            m_travelVFX.Stop();

        if(m_impactVFX != null)
            m_impactVFX.Stop();
    }

    Vector3 m_startPosition;
    Vector3 m_targetPosition;
    float m_timeElapsed;
    float m_travelStartTime;
    float m_hitTime;

    bool m_travelStarted = false;
    bool m_hit = false;

    // Update is called once per frame
    void Update()
    {
        if(m_hit)
        {
            return;
        }

        m_timeElapsed += Time.deltaTime;

        if (!m_travelStarted && Time.time >= m_travelStartTime)
        {
            StartTravel();
            return;
        }

        if (!m_hit && Time.time >= m_hitTime)
        {
            HitTarget();
            return;
        }

        transform.position = Vector3.Lerp(m_startPosition, m_targetPosition, m_timeElapsed / m_travelTime);
    }

    void StartTravel()
    {
        m_travelStarted = true;
        if (m_travelVFX != null)
            m_travelVFX.Play();
    }

    void HitTarget()
    {
        //if(m_target != null)
        //{
        //    m_target.TakeDamage(m_damage);
        //}
        m_hit = true;
        if(m_impactVFX != null)
            m_impactVFX.Play();
        //{

            //Instantiate(m_impactVFX, transform.position, Quaternion.identity, transform.parent);
        //}

        if(m_travelVFX != null)
            m_travelVFX.Stop();

        Destroy(gameObject, m_impactLength);
    }
}
