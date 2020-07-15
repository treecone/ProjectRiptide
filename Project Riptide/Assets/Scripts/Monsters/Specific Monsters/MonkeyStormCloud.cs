using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonkeyStormAttackState { TrackingStorm = 1, CircleStorm = 2}

public partial class MonkeyStormCloud : Enemy
{
    [SerializeField]
    private GameObject _stormCloud;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.MonkeyStorm;
        _speed = 1.0f;
        _health = 1;
        _maxHealth = 1;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = 60.0f;
        _hostileRadius = 60.0f;
        _passiveRadius = 60.0f;
        _maxRadius = 240.0f;
        _specialCooldown = new float[3] { 5.0f, 0.0f, 0.0f};
        _activeStates = new bool[1] { false };
        _animParm = new int[2] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = 0.0f;
        _HostileAI = HostileMonkeyStorm;
        _PassiveAI = PassiveDoNothing;

        //Setup health bar
        _healthBar.SetMaxHealth(_maxHealth);
        _healthBar.UpdateHealth(_health);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnHostile()
    {
        base.OnHostile();
        _canvas.SetActive(false);
    }
}
