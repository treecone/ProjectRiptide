using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StingrayAttackState { BoltAttack = 1, CrossZap = 2};

public partial class Stingray : Enemy
{
    [SerializeField]
    private GameObject _electricParticles;

    private bool _crossZapping;
    public bool CrossZapping => _crossZapping;

    private Enemy _zapBuddy;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.Stingray;
        _speed = 1.0f;
        _health = 100;
        _maxHealth = 100;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = 60.0f;
        _hostileRadius = 30.0f;
        _passiveRadius = 120.0f;
        _maxRadius = 240.0f;
        _specialCooldown = new float[5] { 5.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        _activeStates = new bool[3] { false, false, false };
        _animParm = new int[2] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = 0.5f;
        //_HostileAI = HostileKoiBoss;
        _PassiveAI = PassiveWanderRadius;

        //Setup health bar
        _healthBar.SetMaxHealth(_maxHealth);
        _healthBar.UpdateHealth(_health);

        //Set up hitboxes
        foreach (Hitbox hitbox in GetComponentsInChildren<Hitbox>())
        {
            hitbox.OnTrigger += HitboxTriggered;
            hitbox.OnStay += OnObsticalCollision;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }
        base.Update();
    }
}
