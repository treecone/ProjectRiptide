using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class RockCrab : Enemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.RockCrab;
        _speed = 0.8f;
        _health = 50;
        _maxHealth = 50;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = 45.0f;
        _hostileRadius = 10.0f;
        _passiveRadius = 50.0f;
        _maxRadius = 240.0f;
        _specialCooldown = new float[1] { 5.0f };
        _activeStates = new bool[3] { false, false, false };
        _animParm = new int[3] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("jump")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _HostileAI = HostileRockCrab;
        _PassiveAI = PassiveRockCrab;

        //Setup health bar
        _healthBar.SetMaxHealth(_maxHealth);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
