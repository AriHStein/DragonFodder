using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXAnimator : MonoBehaviour
{
    [SerializeField] VisualEffect vfx = default;

    private void Start()
    {
        vfx.Stop();
    }

    public void PlayEffect()
    {
        vfx.Play();
    }

    public void StopEffect()
    {
        vfx.Stop();
    }
}
