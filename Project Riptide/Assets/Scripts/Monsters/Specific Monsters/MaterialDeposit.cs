using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialDeposit : Enemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.MaterialDeposit;
        _speed = 1.0f;
        _health = 10;
        _maxHealth = 10;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = 30.0f;
        _hostileRadius = 0.0f;
        _passiveRadius = 10.0f;
        _maxRadius = 10.0f;
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 0;
        _pushMult = 0.0f;
        _HostileAI = PassiveDoNothing;
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

    protected override void OnDeath()
    {
        base.OnDeath();
        Destroy(transform.GetChild(1).gameObject);
        Destroy(transform.GetChild(0).gameObject);
    }
}
