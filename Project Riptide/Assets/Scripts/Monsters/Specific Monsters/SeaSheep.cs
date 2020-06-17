using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SeaSheep : Enemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.SeaSheep;
        _speed = 0.7f;
        _health = 20;
        _maxHealth = 20;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = 45.0f;
        _hostileRadius = 10.0f;
        _passiveRadius = 20.0f;
        _maxRadius = 100.0f;
        _specialCooldown = new float[1] { 5.0f };
        _activeStates = new bool[1] { false };
        _animParm = new int[3] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("jump")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = 10.0f;
        _HostileAI = HostileRunAway;
        _PassiveAI = PassiveWanderRadius;

        //Setup health bar
        _healthBar.SetMaxHealth(_maxHealth);
        _healthBar.UpdateHealth(_health);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
