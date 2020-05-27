﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Caden Messenger, Mira Antolovich
/// 4/6/2019
/// </summary>
public enum EnemyState { Passive, Hostile }
public delegate void AI();
public delegate bool MonsterAction(ref float time);
public delegate Vector3 GetVector();
public delegate void GiveVector(Vector3 vec);
public enum EnemyType { FirstEnemy = 0, KoiBoss = 1, DefensiveEnemy = 2, PassiveEnemy = 3, CrabRock = 4}
public enum Anim { Die = 0};
public enum CarpAnim { SwimSpeed = 1, Dive = 2, Shoot = 3, UAttack = 4, Velocity = 5};


public partial class Enemy : Physics
{
    //public fields
    [SerializeField]
    private EnemyType _enemyType;
    [SerializeField]
    private GameObject _projectile;
    [SerializeField]
    private GameObject _healthBarObject;
    [SerializeField]
    private GameObject _hitbox;
    [SerializeField]
    private Camera _camera;

    private HealthBar _healthBar;

    //fields
    private float _health;
    private float _maxHealth;

    [HideInInspector]
    public EnemyState state;

    //player's distance from enemy
    private float _playerDistance;
    //monsters distance from start position
    private float _enemyDistance;
    //Radius to trigger hostile AI
    private float _hostileRadius;
    //Radius to trigger passive AI
    private float _passiveRadius;
    //Fields for AI
    private float _speed;
    private Vector3 _destination;
    private Quaternion _lookRotation;
    private double _timeBetween;
    private double _timeCurrent;
    private Vector3 _startPos;
    private Vector3 _gravity;
    private float _wanderRadius;
    private float _maxRadius;
    private float _passiveCooldown;
    private float[] _specialCooldown;
    private bool[] _activeStates;
    private bool _playerCollision;
    private bool _obsticalCollision;
    private bool _isRaming;
    private bool _inKnockback = false;
    private float _initalPos;
    private float _currTime = 0.0f;
    private int _ramingDamage;
    private AI _HostileAI;
    private AI _PassiveAI;
    private List<GameObject> _hitboxes;
    private List<GameObject> _hurtboxes;
    private Queue<MonsterAction> _actionQueue;
    private GetVector _PlayerPosition;
    private GiveVector _SendKnockback;

    //Animation
    private Animator _animator;
    private int[] _animParm;

    //Death
    private bool _dying = false;
    private int _deathAnim;

    //Fields for collision detection
    [SerializeField]
    private float _lengthMult;
    [SerializeField]
    private float _widthMult;
    [SerializeField]
    private float _heightMult;

    private float _halfView = 55.0f;
    private float _viewRange = 20.0f;
    private Vector3 _widthVector;

    //Smooth rotation stuff
    private float _rotationalVeloctiy = 0.5f;

    public float health => _health;

    public Vector2 startingChunk;

    // Start is called before the first frame update
    protected override void Start()
    {
        state = EnemyState.Passive;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerDistance = Vector3.Distance(transform.position, player.transform.position);
        _healthBar = GetComponent<HealthBar>();
        _healthBarObject.SetActive(false);
        _hitboxes = new List<GameObject>();
        _actionQueue = new Queue<MonsterAction>();
        _PlayerPosition = player.GetComponent<ShipMovement>().GetPosition;
        _SendKnockback = player.GetComponent<ShipMovement>().TakeKnockback;
        foreach (Hitbox hitbox in GetComponentsInChildren<Hitbox>())
        {
            hitbox.OnTrigger += HitboxTriggered;
            hitbox.OnStay += OnObsticalCollision;
        }
        LoadEnemy(_enemyType);
        _camera = Camera.main.GetComponent<Camera>();
        _animator = GetComponentInChildren<Animator>();

        _widthVector = new Vector3(_widthMult, 0, 0);

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!_dying)
        {
            //updates player position
            _playerDistance = Vector3.Distance(transform.position, _PlayerPosition());
            _enemyDistance = Vector3.Distance(_startPos, transform.position);

            //checks for states
            switch (state)
            {
                case EnemyState.Passive:
                    _PassiveAI();
                    //check for hostile behavior trigger event stuff -> if you get close enough, or shoot it
                    //also make sure enemy is not in a passive cooldown
                    if (_playerDistance < _hostileRadius && _passiveCooldown <= 0)
                    {
                        _healthBarObject.SetActive(true);
                        state = EnemyState.Hostile;
                    }
                    break;
                case EnemyState.Hostile:
                    _HostileAI();
                    //check for passive behavior trigger, if you get far enough away
                    if (_playerDistance >= _passiveRadius)
                    {
                        _healthBarObject.SetActive(false);
                        state = EnemyState.Passive;
                    }
                    break;
            }

            if (Input.GetKeyDown(KeyCode.Space))
                TakeDamage(10);

            //Make health bar face player
            _healthBarObject.transform.rotation = new Quaternion(_camera.transform.rotation.x, _camera.transform.rotation.y, _camera.transform.rotation.z, _camera.transform.rotation.w);

            if (_passiveCooldown > 0)
                _passiveCooldown -= Time.deltaTime;

            SetHealthBarPosition();

            _playerCollision = false;
            _obsticalCollision = false;

            base.Update();
        }
        else
        {
            if(!_animator.IsInTransition(0) && !_animator.GetCurrentAnimatorStateInfo(0).IsTag("death"))
            {
                DestroyEnemy();
            }
        }
    }

    /// <summary>
    /// Loads an enemy of the specified type
    /// </summary>
    /// <param name="type">Type of enemy</param>
    private void LoadEnemy(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.FirstEnemy:
                _speed = 1.0f;
                _health = 20;
                _maxHealth = 20;
                _timeBetween = 5.0;
                _timeCurrent = _timeBetween;
                _startPos = transform.position;
                _wanderRadius = 30.0f;
                _hostileRadius = 10.0f;
                _passiveRadius = 50.0f;
                _specialCooldown = new float[1] { 5.0f };
                _activeStates = new bool[1] { false };
                _playerCollision = false;
                _isRaming = false;
                _ramingDamage = 15;
                _HostileAI = HostileFollowAndDash;
                _PassiveAI = PassiveWanderRadius;
                break;
            case EnemyType.KoiBoss:
                _speed = 1.0f;
                _health = 200;
                _maxHealth = 200;
                _timeBetween = 5.0;
                _timeCurrent = _timeBetween;
                _startPos = transform.position;
                _wanderRadius = 45.0f;
                _hostileRadius = 30.0f;
                _passiveRadius = 120.0f;
                _maxRadius = 240.0f;
                _specialCooldown = new float[5] { 5.0f, 0.0f, 0.0f, 0.0f, 0.0f };
                _activeStates = new bool[3] { false, false, false };
                _animParm = new int[6] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("swimSpeed"),
                    Animator.StringToHash("dive"),
                    Animator.StringToHash("shoot"),
                    Animator.StringToHash("uAttack"),
                    Animator.StringToHash("velocity")};
                _playerCollision = false;
                _isRaming = false;
                _ramingDamage = 20;
                _HostileAI = KoiBossHostile;
                _PassiveAI = PassiveWanderRadius;
                break;
            case EnemyType.DefensiveEnemy:
                _speed = 1.0f;
                _health = 30;
                _maxHealth = 30;
                _timeBetween = 5.0;
                _timeCurrent = _timeBetween;
                _startPos = transform.position;
                _wanderRadius = 30.0f;
                _hostileRadius = 0.0f;
                _passiveRadius = 50.0f;
                _maxRadius = 120.0f;
                _specialCooldown = new float[1] { 5.0f };
                _activeStates = new bool[1] { false };
                _playerCollision = false;
                _isRaming = false;
                _ramingDamage = 15;
                _HostileAI = HostileFollowAndDash;
                _PassiveAI = PassiveWanderRadius;
                break;
            case EnemyType.PassiveEnemy:
                _speed = 1.2f;
                _health = 20;
                _maxHealth = 20;
                _timeBetween = 5.0;
                _timeCurrent = _timeBetween;
                _startPos = transform.position;
                _wanderRadius = 30.0f;
                _hostileRadius = 10.0f;
                _passiveRadius = 30.0f;
                _maxRadius = 120.0f;
                _specialCooldown = new float[1] { 5.0f };
                _activeStates = new bool[1] { false };
                _playerCollision = false;
                _isRaming = false;
                _ramingDamage = 5;
                _HostileAI = HostileRunAway;
                _PassiveAI = PassiveWanderRadius;
                break;
            case EnemyType.CrabRock:
                _speed = 0.8f;
                _health = 50;
                _maxHealth = 50;
                _timeBetween = 5.0;
                _timeCurrent = _timeBetween;
                _startPos = transform.position;
                _wanderRadius = 45.0f;
                _hostileRadius = 10.0f;
                _passiveRadius = 130.0f;
                _maxRadius = 240.0f;
                _specialCooldown = new float[1] { 5.0f };
                _activeStates = new bool[1] { false};
                _playerCollision = false;
                _isRaming = false;
                _ramingDamage = 20;
                _HostileAI = HostileRockCrab;
                _PassiveAI = PassiveDoNothing;
                break;
        }

        //Setup health bar
        _healthBar.SetMaxHealth(_maxHealth);
    }

    /// <summary>
    /// Monster takes damage, if health is 0 they die
    /// </summary>
    /// <param name="damage">Amount of damage taken</param>
    public void TakeDamage(float damage)
    {
        _health -= damage;
        _healthBar.UpdateHealth(_health);
        if (state == EnemyState.Passive)
        {
            _healthBarObject.SetActive(true);
            state = EnemyState.Hostile;
        }
        if (_health <= 0)
        {
            _health = 0;
            if(_animator != null)
            {
                _animator.SetTrigger(_animParm[(int)Anim.Die]);
                _deathAnim = Animator.StringToHash("death");
            }
            _dying = true;
        }
    }

    /// <summary>
    /// Destorys the enemy and drops loot
    /// </summary>
    public void DestroyEnemy()
    {
        GameObject lootable = Instantiate(Resources.Load("Inventory/Lootable"), new Vector3(transform.position.x + Random.Range(-2.0f, 2.0f), transform.position.y, transform.position.z + Random.Range(-2.0f, 2.0f)), Quaternion.identity) as GameObject;
        lootable.GetComponent<Lootable>().itemStored = GameObject.FindWithTag("GameManager").GetComponent<ItemDatabase>().FindItem("Carp Scale");
        lootable.GetComponent<Lootable>().lightColor = GameObject.FindWithTag("GameManager").GetComponent<ItemDatabase>().rarityColors[lootable.GetComponent<Lootable>().itemStored.Rarity];
        //Kill monster
        Destroy(gameObject);
    }

    /// <summary>
    /// Resets enemy's hostile AI values
    /// </summary>
    public void ResetHostile()
    {
        //reset states
        for (int i = 0; i > _activeStates.Length; i++)
        {
            _activeStates[i] = false;
        }
        //reset cooldowns
        for (int i = 0; i > _specialCooldown.Length; i++)
        {
            _specialCooldown[i] = 0.0f;
        }
        _isRaming = false;
        _inKnockback = false;
        _actionQueue.Clear();
        ClearHitboxes();
        _currTime = 0;
    }

    /// <summary>
    /// Called when a hitbox is triggered
    /// </summary>
    /// <param name="collision">GameObject that triggered hitbox</param>
    public void HitboxTriggered(GameObject collision)
    {
        if (collision.tag == "Obstical")
        {
            _obsticalCollision = true;
            Debug.Log("Obstical Collision");
        }
        if (collision.tag == "Player")
            _playerCollision = true;

    }

    /// <summary>
    /// Creates a hitbox as a child of the enemy
    /// </summary>
    /// <param name="position">Position relative to enemy</param>
    /// <param name="scale">Size of hitbox</param>
    /// <param name="type">Type of hitbox</param>
    /// <param name="damage">Damage dealt by hitbox</param>
    /// <returns></returns>
    public GameObject CreateHitbox(Vector3 position, Vector3 scale, HitboxType type, float damage)
    {
        GameObject temp = Instantiate(_hitbox, transform);
        temp.GetComponent<Hitbox>().SetHitbox(gameObject, position, scale, type, damage);
        temp.GetComponent<Hitbox>().OnTrigger += HitboxTriggered;
        return temp;
    }

    /// <summary>
    /// Creates a hitbox as a child of the enemy
    /// </summary>
    /// <param name="position">Position relative to enemy</param>
    /// <param name="scale">Size of hitbox</param>
    /// <param name="type">Type of hitbox</param>
    /// <param name="damage">Damage dealt by hitbox</param>
    /// <param name="launchAngle">Angle that hitbox will launch player</param>
    /// <param name="launchStrength">Strength at which player will be launched</param>
    /// <returns></returns>
    public GameObject CreateHitbox(Vector3 position, Vector3 scale, HitboxType type, float damage, Vector2 launchAngle, float launchStrength)
    {
        GameObject temp = Instantiate(_hitbox, transform);
        temp.GetComponent<Hitbox>().SetHitbox(gameObject, position, scale, type, damage, launchAngle, launchStrength);
        temp.GetComponent<Hitbox>().OnTrigger += HitboxTriggered;
        return temp;
    }

    /// <summary>
    /// Spawns an enemy projectile
    /// </summary>
    /// <param name="position">Position relative to enemy</param>
    /// <param name="speed">Speed of projectile</param>
    /// <param name="damage">Damage projectile inflicts</param>
    /// <param name="maxLifeSpan">Max life span of projectile</param>
    /// <param name="movementPattern">Movement pattern of projectile</param>
    private void SpawnProjectile(Vector3 position, float speed, int damage, float maxLifeSpan, MovementPattern movementPattern)
    {
        GameObject.Instantiate(_projectile,
            transform.position + transform.TransformVector(position),
            new Quaternion())
            .GetComponent<EnemyProjectile>().LoadProjectile(
            transform.TransformVector(position),
            0.75f,
            damage,
            maxLifeSpan,
            movementPattern);
    }

    /// <summary>
    /// Spawns an enemy projectile
    /// </summary>
    /// <param name="position">Position relative to enemy</param>
    /// <param name="speed">Speed of projectile</param>
    /// <param name="damage">Damage projectile inflicts</param>
    /// <param name="maxLifeSpan">Max life span of projectile</param>
    /// <param name="movementPattern">Movement pattern of projectile</param>
    /// <param name="launchAngle">Angle that hitbox will launch player</param>
    /// <param name="launchStrength">Strength at which player will be launched</param>
    private void SpawnProjectile(Vector3 position, float speed, int damage, float maxLifeSpan, MovementPattern movementPattern, Vector2 launchAngle, float launchStrength)
    {
        GameObject.Instantiate(_projectile,
            transform.position + transform.TransformVector(position),
            new Quaternion())
            .GetComponent<EnemyProjectile>().LoadProjectile(
            transform.TransformVector(position),
            0.75f,
            damage,
            maxLifeSpan,
            movementPattern,
            launchAngle,
            launchStrength);
    }

    private void ClearHitboxes()
    {
        for (int i = 0; i < _hitboxes.Count; i++)
        {
            GameObject.Destroy(_hitboxes[i]);
        }
        _hitboxes.Clear();
    }

    /// <summary>
    /// Returns enemy to inital position on Y axis
    /// </summary>
    private void ReturnToInitalPosition()
    {

        _position = new Vector3(transform.position.x, _initalPos, transform.position.z);
    }

    /// <summary>
    /// Applys a force to move the enemy in an arc
    /// </summary>
    /// <param name="dir">Direction of movement</param>
    /// <param name="dist">Horizontal distance covered</param>
    /// <param name="time">Time that the arc takes place</param>
    /// <param name="gravity">Gravity being applied each frame</param>
    private void ApplyArcForce(Vector3 dir, float dist, float time, Vector3 gravity)
    {
        float xForce = _mass * (dist / (time * Time.deltaTime));
        float yForce = (-gravity.y * time) / (2 * Time.deltaTime);
        Vector3 netForce = dir * xForce;
        netForce += yForce * Vector3.up;
        ApplyForce(netForce);
    }

    /// <summary>
    /// Applys a force to move the enemy in an arc
    /// </summary>
    /// <param name="dir">Direction of movement</param>
    /// <param name="dist">Horizontal distance covered</param>
    /// <param name="yMax">Maximum vertical distance</param>
    /// <param name="time">Time that the arc takes place</param>
    /// <returns>Gravity to be applied each frame</returns>
    private Vector3 ApplyArcForce(Vector3 dir, float dist, float yMax, float time)
    {
        float xForce = _mass * (dist / (time * Time.deltaTime));
        float gravity = (-8 * _mass * yMax) / (time * time);
        float yForce = (-gravity * time) / (2 * Time.deltaTime);
        Vector3 netForce = dir * xForce;
        netForce += yForce * Vector3.up;
        ApplyForce(netForce);
        return Vector3.up * gravity;
    }

    /// <summary>
    /// Applies a force to move in a direction at a specified speed
    /// Applied only once
    /// </summary>
    /// <param name="dir">Direction of movment</param>
    /// <param name="dist">Distance moved over time frame</param>
    /// <param name="time">Time frame to move dstance</param>
    private void ApplyMoveForce(Vector3 dir, float dist, float time)
    {
        float moveForce = _mass * (dist / (time * Time.deltaTime));
        Vector3 netForce = dir * moveForce;
        ApplyForce(netForce);
    }

    /// <summary>
    /// Applies a force to move in a direction at a specified speed
    /// Needs to be applied each frame
    /// </summary>
    /// <param name="dir">Direction of movement</param>
    /// <param name="dist">Distance moved over time frame</param>
    /// <param name="time">Time frame to move distance</param>
    private void ApplyConstantMoveForce(Vector3 dir, float dist, float time)
    {
        float moveForce = (2 * _mass * dist) / (time * time);
        Vector3 netForce = dir * moveForce;
        ApplyForce(netForce);
    }

    /// <summary>
    /// Sets position of health bar above enemy
    /// </summary>
    private void SetHealthBarPosition()
    {
        _healthBarObject.transform.position = new Vector3(transform.position.x, _heightMult + 1.5f * transform.localScale.y, transform.position.z);
    }

    /// <summary>
    /// Checks if there is an obstical in the enemy's path
    /// </summary>
    /// <returns>If enemy's path is interuptted</returns>
    public bool CheckObstacle()
    {
        RaycastHit hit = new RaycastHit();
        Vector3 detectPosition = transform.GetChild(transform.childCount - 1).position;
        for (int i = 0; i <= _halfView; i += 4)
        {
            Debug.DrawRay(detectPosition, Quaternion.AngleAxis(i, Vector3.up) * transform.forward * _viewRange, Color.red);
            Debug.DrawRay(detectPosition, Quaternion.AngleAxis(-i, Vector3.up) * transform.forward * _viewRange, Color.red);
            if (UnityEngine.Physics.Raycast(detectPosition, Quaternion.AngleAxis(i, Vector3.up) * transform.forward, out hit, _viewRange))
            {
                return true;
            }
            if (UnityEngine.Physics.Raycast(detectPosition, Quaternion.AngleAxis(-i, Vector3.up) * transform.forward, out hit, _viewRange))
            {
                return true;
            }
        }
        /*if(Physics.SphereCast(detectPosition + transform.TransformDirection(widthVector), widthMult, transform.forward, out hit, viewRange * 1.5f))
        {
            return true;
        }*/

        return false;
    }

    /// <summary>
    /// Checks if there is an obstical in the enemy's path
    /// </summary>
    /// <returns>If enemy's path is interuptted</returns>
    public bool CheckObstacle(Vector3 target)
    {
        RaycastHit hit = new RaycastHit();
        Vector3 detectPosition = transform.GetChild(transform.childCount - 1).position;
        Vector3 targetDir = target - transform.position;
        targetDir.Normalize();
        /*
        for (int i = 0; i <= _halfView; i += 4)
        {
            Debug.DrawRay(detectPosition, Quaternion.AngleAxis(i, Vector3.up) * targetDir * _viewRange, Color.red);
            Debug.DrawRay(detectPosition, Quaternion.AngleAxis(-i, Vector3.up) * targetDir * _viewRange, Color.red);
            if (UnityEngine.Physics.Raycast(detectPosition, Quaternion.AngleAxis(i, Vector3.up) * targetDir, out hit, _viewRange))
            {
                return true;
            }
            if (UnityEngine.Physics.Raycast(detectPosition, Quaternion.AngleAxis(-i, Vector3.up) * targetDir, out hit, _viewRange))
            {
                return true;
            }
        }*/
        if(UnityEngine.Physics.SphereCast(detectPosition, _widthMult * 2, transform.forward, out hit, _viewRange * 1.5f))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Find direction to avoid obstacle
    /// </summary>
    /// <returns>Direction to avoid obstacle</returns>
    public Vector3 AvoidObstacle()
    {
        //Debug.Log("Avoiding Obstacle");
        Vector3 dir = Vector3.zero;
        bool found = false;

        Vector3 detectPosition = transform.GetChild(transform.childCount - 1).position;
        RaycastHit hit;

        //Check 90 degrees for a path to avoid obstacle
        for (int i = 0; i <= 90; i += 4)
        {
            //Check right side for path
            if (!UnityEngine.Physics.SphereCast(detectPosition, _widthMult, Quaternion.AngleAxis(i, Vector3.up) * transform.forward, out hit, _viewRange * 1.5f))
            {
                //Set direction if path is found
                 dir = Quaternion.AngleAxis(i, Vector3.up) * transform.forward;
                Debug.DrawLine(transform.position, transform.position + dir * _viewRange * 1.5f, Color.yellow);
                 found = true;
            }
            //Check left side for path
            if (!UnityEngine.Physics.SphereCast(detectPosition, _widthMult, Quaternion.AngleAxis(-i, Vector3.up) * transform.forward, out hit, _viewRange * 1.5f))
            {
                //Set direction if path is found
                dir = Quaternion.AngleAxis(-i, Vector3.up) * transform.forward;
                Debug.DrawLine(transform.position, transform.position + dir * _viewRange * 1.5f, Color.yellow);
                found = true;
            }
            if (found)
                return dir;
        }

        return Quaternion.AngleAxis(90, Vector3.up) * transform.forward;
    }

    /// <summary>
    /// Find direction to avoid obstacle
    /// </summary>
    /// <returns>Direction to avoid obstacle</returns>
    public Vector3 AvoidObstacle(Vector3 target)
    {
        //Debug.Log("Avoiding Obstacle");
        Vector3 dir = Vector3.zero;
        bool found = false;

        Vector3 detectPosition = transform.GetChild(transform.childCount - 1).position;
        Vector3 targetDir = target - transform.position;
        targetDir.Normalize();
        RaycastHit hit;

        //Check 90 degrees for a path to avoid obstacle
        for (int i = 0; i <= 90; i += 4)
        {
            //Check right side for path
            if (!UnityEngine.Physics.SphereCast(detectPosition, _widthMult, Quaternion.AngleAxis(i, Vector3.up) * targetDir, out hit, _viewRange * 1.5f))
            {
                //Set direction if path is found
                dir = Quaternion.AngleAxis(i, Vector3.up) * targetDir;
                Debug.DrawLine(transform.position, transform.position + dir * _viewRange * 1.5f, Color.yellow);
                found = true;
            }
            //Check left side for path
            if (!UnityEngine.Physics.SphereCast(detectPosition, _widthMult, Quaternion.AngleAxis(-i, Vector3.up) * targetDir, out hit, _viewRange * 1.5f))
            {
                //Set direction if path is found
                dir = Quaternion.AngleAxis(-i, Vector3.up) * targetDir;
                Debug.DrawLine(transform.position, transform.position + dir * _viewRange * 1.5f, Color.yellow);
                found = true;
            }
            if (found)
                return dir;
        }

        return Quaternion.AngleAxis(90, Vector3.up) * transform.forward;
    }

    /// <summary>
    /// Called when inside an obstacle
    /// Move enemy after the obstacle
    /// </summary>
    /// <param name="obstical">GameObject colliding with</param>
    public void OnObsticalCollision(GameObject obstical)
    {
        if (obstical.tag == "Obstical")
        {
            StopMotion();
            Vector3 backForce = transform.position - obstical.transform.position;
            backForce = new Vector3(backForce.x, 0, backForce.z);
            backForce.Normalize();
            backForce *= 200.0f;
            ApplyForce(backForce);
        }
    }

    /// <summary>
    /// Take knockback from an outside source
    /// </summary>
    /// <param name="knockback">Knockback force</param>
    public void TakeKnockback(Vector3 knockback)
    {
        ApplyForce(knockback);
    }

    /// <summary>
    /// Rotates monster smoothly towards desired rotation
    /// </summary>
    /// <param name="desiredRotation">Desired Rotation</param>
    /// <param name="rotationalAcceleration">Rotational Acceleration</param>
    /// <param name="minRotationalVelocity">Minimum Rotational Velocity</param>
    /// <param name="maxRotationalVelocity">Maximum Rotational Velocity</param>
    public void SetSmoothRotation(Quaternion desiredRotation, float rotationalAcceleration, float minRotationalVelocity, float maxRotationalVelocity)
    {
        //Rotate based on target location
        if (_rotation != desiredRotation)
        {
            //If rotation is close to desired location, slow down rotation
            if (Quaternion.Angle(_rotation, desiredRotation) < 45.0f)
            {
                _rotationalVeloctiy += _rotationalVeloctiy * -0.80f * Time.deltaTime;
                //Make sure rotation stay's above minium value
                if (_rotationalVeloctiy < minRotationalVelocity)
                    _rotationalVeloctiy = minRotationalVelocity;
            }
            //Else speed up rotation
            else
            {
                _rotationalVeloctiy += rotationalAcceleration * Time.deltaTime;
                //Make sure rotation stay's below maximum value
                if (_rotationalVeloctiy > maxRotationalVelocity)
                    _rotationalVeloctiy = maxRotationalVelocity;
            }

            //Update rotation
            _rotation = Quaternion.RotateTowards(_rotation, desiredRotation, _rotationalVeloctiy);
        }
        //Reset velocity when not rotating
        else
            _rotationalVeloctiy = minRotationalVelocity;
    }
}