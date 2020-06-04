using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FlowerFrog : Enemy
{
    [SerializeField]
    protected LineRenderer _tounge;

    protected const float LATCH_DAMAGE_CAP = 15;
    protected float _latchStartHealth = 0;

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
        _wanderRadius = 30.0f;
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
        _pushMult = 2.0f;
        _HostileAI = HostileFlowerFrog;
        _PassiveAI = PassiveReturnToRadius;

        //Setup health bar
        _healthBar.SetMaxHealth(_maxHealth);
    }

    // Update is called once per frame
    protected override void Update()
    {
        _tounge.transform.rotation = Quaternion.identity;
        base.Update();
        if(IsDying)
        {
            _tounge.SetPosition(1, Vector3.zero);
        }
    }
}
