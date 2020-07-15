using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleWave : Physics
{
    protected float _scaleDist;
    protected float _yMax;
    protected Vector3 _gravity;
    protected float _lifeTime;
    protected float _currLife;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _gravity = ApplyArcForce(transform.forward, 0, _yMax, _lifeTime);
    }

    // Update is called once per frame
    protected override void Update()
    {
        ApplyForce(_gravity);
        transform.localScale += new Vector3(1,0,1) * (_scaleDist / _lifeTime * Time.deltaTime);
        _currLife += Time.deltaTime;
        if (_currLife >= _lifeTime)
        {
            Destroy(gameObject);
        }

        base.Update();
    }

    /// <summary>
    /// Sets up wave to be sent out
    /// </summary>
    /// <param name="distance">Distance traveled in life time</param>
    /// <param name="yMax">Y distance traveled</param>
    /// <param name="lifeTime">Lifetime of wave</param>
    /// <param name="damage">Damage dealt by the wave</param>
    /// <param name="launchStrength">Strength of wave's knockback</param>
    public void StartWave(float scaleDist, float yMax, float lifeTime, float damage, float launchStrength)
    {
        _scaleDist = scaleDist;
        _yMax = yMax;
        _lifeTime = lifeTime;
        Hitbox hitbox = GetComponentInChildren<Hitbox>();
        hitbox.Damage = damage;
        hitbox.LaunchStrength = launchStrength;
    }
}
