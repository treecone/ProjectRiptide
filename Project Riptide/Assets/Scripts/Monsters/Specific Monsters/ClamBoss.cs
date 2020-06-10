﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClamOpenState { Dragon = 0, Bird = 1, WaterSpout = 2}; 

public partial class ClamBoss : Enemy
{
    protected ClamOpenState _openState;
    protected float _lineOffset;
    protected float _speedScale;

    [SerializeField]
    protected GameObject _tentaclePrefab;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.ClamBoss;
        _speed = 1.0f;
        _health = 250;
        _maxHealth = 250;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = 60.0f;
        _hostileRadius = 30.0f;
        _passiveRadius = 120.0f;
        _maxRadius = 240.0f;
        _specialCooldown = new float[7] { 1.0f, 0.0f, 0.0f, 5.0f, 0.0f, 0.0f, 0.0f };
        _activeStates = new bool[3] { false, false, false };
        _animParm = new int[2] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = 0.1f;
        _HostileAI = HostileClamBoss;
        _PassiveAI = PassiveDoNothing;

        //Setup health bar
        _healthBar.SetMaxHealth(_maxHealth);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }
        //Calculate speed scale for attacks
        _speedScale = 0.5f + _health / _maxHealth * 0.5f;
        base.Update();
    }
}
