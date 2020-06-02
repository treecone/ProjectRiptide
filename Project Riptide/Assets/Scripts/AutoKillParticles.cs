using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoKillParticles : MonoBehaviour
{
    private ParticleSystem _particles;

    // Start is called before the first frame update
    void Start()
    {
        _particles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!_particles.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
