using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementPattern { Forward }

public class EnemyProjectile : MonoBehaviour
{
    //Fields
    [SerializeField]
    private GameObject _hitbox;

    private Vector3 _velocity;
    private int _damage;
    private float _speed;
    private float _currLifeSpan;
    private float _maxLifeSpan;
    private Vector2 _launchAngle;
    private float _launchStrength;
    private MovementPattern _movementPattern;
    private GameObject _projHitbox;
    private Vector3 _hitboxSize = Vector3.zero;

    public GameObject AttachedHitbox
    {
        get { return _projHitbox; }
        set { _projHitbox = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(_hitboxSize == Vector3.zero)
        {
            _hitboxSize = new Vector3(1, 1, 1);
        }
        _projHitbox = Instantiate(_hitbox, transform);
        _projHitbox.GetComponent<Hitbox>().SetHitbox(gameObject, Vector3.zero, _hitboxSize, HitboxType.EnemyHitbox, _damage, _launchAngle, _launchStrength);
        _projHitbox.GetComponent<Hitbox>().OnTrigger += DestroyProj;
        _currLifeSpan = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Increment lifespan
        _currLifeSpan += Time.deltaTime;

        if(_currLifeSpan >= _maxLifeSpan)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            //Move projectile based on its movement pattern
            switch(_movementPattern)
            {
                case MovementPattern.Forward:
                    MoveProjectileForward();
                    break;
            }
        }
    }

    /// <summary>
    /// Loads projecitle
    /// </summary>
    /// <param name="velocity">Direction of projectile</param>
    /// <param name="speed">Speed of projectile</param>
    /// <param name="damage">Damage projectile inflicts</param>
    /// <param name="maxLifeSpan">Max life span of projectile</param>
    public void LoadProjectile(Vector3 velocity, float speed, int damage, float maxLifeSpan, MovementPattern movementPattern)
    {
        this._velocity = velocity;
        this._speed = speed;
        this._damage = damage;
        this._maxLifeSpan = maxLifeSpan;
        this._movementPattern = movementPattern;
    }

    /// <summary>
    /// Loads projecitle
    /// </summary>
    /// <param name="velocity">Direction of projectile</param>
    /// <param name="speed">Speed of projectile</param>
    /// <param name="damage">Damage projectile inflicts</param>
    /// <param name="maxLifeSpan">Max life span of projectile</param>
    /// <param name="launchAngle">Knockback angle for player</param>
    /// <param name="launchStrength">Knockback strength</param>
    public void LoadProjectile(Vector3 velocity, float speed, int damage, float maxLifeSpan, MovementPattern movementPattern, Vector2 launchAngle, float launchStrength)
    {
        _velocity = velocity;
        _speed = speed;
        _damage = damage;
        _maxLifeSpan = maxLifeSpan;
        _movementPattern = movementPattern;
        _launchAngle = launchAngle;
        _launchStrength = launchStrength;
    }

    /// <summary>
    /// Loads projecitle
    /// </summary>
    /// <param name="velocity">Direction of projectile</param>
    /// <param name="speed">Speed of projectile</param>
    /// <param name="damage">Damage projectile inflicts</param>
    /// <param name="maxLifeSpan">Max life span of projectile</param>
    /// <param name="launchAngle">Knockback angle for player</param>
    /// <param name="launchStrength">Knockback strength</param>
    public void LoadProjectile(Vector3 velocity, float speed, int damage, float maxLifeSpan, MovementPattern movementPattern, Vector2 launchAngle, float launchStrength, Vector3 hitboxSize)
    {
        _velocity = velocity;
        _speed = speed;
        _damage = damage;
        _maxLifeSpan = maxLifeSpan;
        _movementPattern = movementPattern;
        _launchAngle = launchAngle;
        _launchStrength = launchStrength;
        _hitboxSize = hitboxSize;
    }

    /// <summary>
    /// Moves projectile in a straight line
    /// </summary>
    private void MoveProjectileForward()
    {
        transform.Translate(_velocity.normalized * _speed * 60 * Time.deltaTime, Space.World);
    }

    //Destroy projectile upon hitbox activation
    void DestroyProj(GameObject hit)
    {
        if(hit.tag == "Obstical")
            Destroy(gameObject);
        if (hit.tag == "Player")
            Destroy(gameObject);
    }
}
