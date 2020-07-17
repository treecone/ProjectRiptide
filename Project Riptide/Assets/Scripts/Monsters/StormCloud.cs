using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormCloud : MonoBehaviour
{
    [SerializeField]
    private GameObject _hitboxPrefab;
    private Hitbox _hitbox;

    private const float HITBOX_ACTIVATE_TIME = 2.0f;

    private float _lifeSpan;
    private float _currLifeSpan;
    private float _damage;
    private float _launchStrength;
    private Vector3 _moveDir;
    private float _moveDistance;
    private GameObject _telegraph;

    // Update is called once per frame
    void Update()
    {
        //Create hitbox when lightning strikes
        if(_currLifeSpan >= HITBOX_ACTIVATE_TIME && _hitbox == null)
        {
            _hitbox = Instantiate(_hitboxPrefab, transform.position, transform.rotation, transform).GetComponent<Hitbox>();
            _hitbox.SetHitbox(gameObject, Vector3.zero, new Vector3(2.5f, 15.0f, 2.5f), HitboxType.EnemyHitbox, _damage, Vector2.zero, _launchStrength);
            transform.parent = null;
            //Turn off telegraph
            if (_telegraph != null)
            {
                _telegraph.GetComponentInChildren<ParticleSystem>().Stop();
            }
        }
        //Move storm after lighting is created
        if(_currLifeSpan >= HITBOX_ACTIVATE_TIME && _currLifeSpan < _lifeSpan)
        {
            if(_moveDir != Vector3.zero)
            {
                transform.position += _moveDir * _moveDistance / (_lifeSpan - HITBOX_ACTIVATE_TIME) * Time.deltaTime;
            }
        }
        //Stop storm after life span is over
        if(_currLifeSpan >= _lifeSpan)
        {
            foreach(ParticleSystem particles in GetComponentsInChildren<ParticleSystem>())
            {
                particles.Stop();
            }
            Destroy(_hitbox.gameObject);
        }

        _currLifeSpan += Time.deltaTime;
    }

    /// <summary>
    /// Set storms values
    /// </summary>
    /// <param name="lifeSpan">Life span inlcuding windup</param>
    /// <param name="damage">Damage dealt</param>
    /// <param name="launchStrength">Strength of knock back</param>
    /// <param name="moveDir">Direction of movement</param>
    /// <param name="moveDistance">Distance moved over life span</param>
    public void SetStorm(float lifeSpan, float damage, float launchStrength, Vector3 moveDir, float moveDistance, GameObject telegraph)
    {
        _lifeSpan = lifeSpan;
        _damage = damage;
        _launchStrength = launchStrength;
        _moveDir = moveDir;
        _moveDistance = moveDistance;
        _telegraph = telegraph;
    }
}
