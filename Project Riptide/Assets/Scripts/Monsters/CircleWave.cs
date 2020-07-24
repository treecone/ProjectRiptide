using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleWave : MonoBehaviour
{
    protected float _dist;
    protected float _lifeTime;
    protected float _currLife;
    protected ParticleSystem particles;
    protected Hitbox _hitbox;
    private bool _dying;

    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        _hitbox = GetComponentInChildren<Hitbox>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_currLife < _lifeTime)
        {
            particles.Stop();
            particles.Clear();
            ParticleSystem.ShapeModule shape = particles.shape;
            shape.radius += _dist / _lifeTime * Time.deltaTime;
            ParticleSystem.EmissionModule emission = particles.emission;
            emission.rateOverTime = 30 * shape.radius;
            particles.Play();

            _hitbox.transform.localScale = new Vector3(shape.radius * 2, 10, shape.radius * 2);
            _currLife += Time.deltaTime;
        }

        if (_currLife >= _lifeTime && !_dying)
        {
            GetComponentInChildren<ParticleSystem>().Stop();
            Destroy(_hitbox.gameObject);
            _dying = true;
        }
    }

    /// <summary>
    /// Sets up wave to be sent out
    /// </summary>
    /// <param name="dist">Distance traveled in life time</param>
    /// <param name="lifeTime">Lifetime of wave</param>
    /// <param name="damage">Damage dealt by the wave</param>
    /// <param name="launchStrength">Strength of wave's knockback</param>
    public void StartWave(float dist, float lifeTime, float damage, float launchStrength)
    {
        _dist = dist;
        _lifeTime = lifeTime;
        Hitbox hitbox = GetComponentInChildren<Hitbox>();
        hitbox.Damage = damage;
        hitbox.LaunchStrength = launchStrength;
    }
}
