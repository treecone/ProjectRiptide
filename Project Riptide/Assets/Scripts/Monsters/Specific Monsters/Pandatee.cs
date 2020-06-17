using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PandateeAnim { Situp = 2, Dive = 3};

public partial class Pandatee : Enemy
{
    protected bool _wentUnderwater = false;
    protected bool _isUnderwater = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.Pandatee;
        _speed = 0.7f;
        _health = 50;
        _maxHealth = 50;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = 45.0f;
        _hostileRadius = 10.0f;
        _passiveRadius = 20.0f;
        _maxRadius = 100.0f;
        _specialCooldown = new float[4] { 5.0f, 0.0f, 0.0f, 0.0f };
        _activeStates = new bool[4] { false, false, false, false };
        _animParm = new int[4] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("situp"),
                    Animator.StringToHash("dive")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = 1.0f;
        _HostileAI = HostilePandateeRunAway;
        _PassiveAI = PassivePandateeWander;

        //Setup health bar
        _healthBar.SetMaxHealth(_maxHealth);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnHostile()
    {
        ResetHostile();
        base.OnHostile();
    }

    protected override void OnPassive()
    {
        base.OnPassive();
        _passiveCooldown = 1.0f;
    }
}
