using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesDestroyParent : MonoBehaviour
{
    /// <summary>
    /// Destroy parent to particle system when particle finishes playing
    /// </summary>
    public void OnParticleSystemStopped()
    {
        Destroy(transform.parent.gameObject);
    }
}
