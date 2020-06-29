using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FrogAnim { Attack = 2, Close = 3};
public enum FlowerFrogAttackState { Latched = 1 }

public partial class FlowerFrog : Enemy
{
    [SerializeField]
    protected LineRenderer _tounge;

    protected const float LATCH_DAMAGE_CAP = 15;
    protected const float MAX_DRAG_DIST = 15.0f;
    protected const float MAX_LATCH_DIST = 25.0f;
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
        _hostileRadius = 15.0f;
        _passiveRadius = 60.0f;
        _maxRadius = 240.0f;
        _specialCooldown = new float[2] { 0.0f, 0.0f};
        _activeStates = new bool[2] { false, false };
        _animParm = new int[4] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("attack"),
                    Animator.StringToHash("close")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = 2.0f;
        _HostileAI = HostileFlowerFrog;
        _PassiveAI = PassiveReturnToRadius;

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
        _tounge.transform.rotation = Quaternion.identity;
        base.Update();
        if(IsInvincible)
        {
            _tounge.SetPosition(1, Vector3.zero);
        }
    }

    /// <summary>
    /// On passive reset tounge position
    /// </summary>
    protected override void OnPassive()
    {
        _tounge.SetPosition(1, Vector3.zero);
        base.OnPassive();
    }
}
