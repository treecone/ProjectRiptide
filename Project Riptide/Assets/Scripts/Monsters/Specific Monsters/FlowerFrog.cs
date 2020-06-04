using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FlowerFrog : Enemy
{
    [SerializeField]
    private LineRenderer _tounge;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.FlowerFrog;
        _speed = 1.0f;
        _health = 50;
        _maxHealth = 50;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = 60.0f;
        _hostileRadius = 30.0f;
        _passiveRadius = 60.0f;
        _maxRadius = 240.0f;
        _specialCooldown = new float[2] { 5.0f, 0.0f};
        _activeStates = new bool[2] { false, false };
        _animParm = new int[2] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = 0.1f;
        _HostileAI = HostileFlowerFrog;
        _PassiveAI = PassiveDoNothing;

        //Setup health bar
        _healthBar.SetMaxHealth(_maxHealth);

        //Setup tounge hitbox
        CreateHitbox(Vector3.zero, new Vector3(1, 1, 1), HitboxType.EnemyHitbox, 0);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
