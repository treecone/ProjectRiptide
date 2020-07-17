using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardWave : MonoBehaviour
{
    protected float _distance;
    protected float _lifeTime;
    protected float _currLife;
    private bool _dying;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(_currLife < _lifeTime)
        {
            transform.position += transform.forward * (_distance / _lifeTime * Time.deltaTime);
            _currLife += Time.deltaTime;
        }

        if(_currLife >= _lifeTime && !_dying)
        {
            Destroy(GetComponentInChildren<Hitbox>().gameObject);
            GetComponentInChildren<ParticleSystem>().Stop();
        }
    }

    /// <summary>
    /// Sets up wave to be sent out
    /// </summary>
    /// <param name="distance">Distance traveled in life time</param>
    /// <param name="lifeTime">Lifetime of wave</param>
    /// <param name="damage">Damage dealt by the wave</param>
    /// <param name="launchStrength">Strength of wave's knockback</param>
    public void StartWave(float distance, float lifeTime, float damage, float launchStrength)
    {
        _distance = distance;
        _lifeTime = lifeTime;
        Hitbox hitbox = GetComponentInChildren<Hitbox>();
        hitbox.Damage = damage;
        hitbox.LaunchStrength = launchStrength;
    }
}
