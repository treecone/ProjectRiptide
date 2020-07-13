using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonkeyAnim { ScreechAttack = 2, ScreechAngry = 3}
public enum MonkeyAttackState { HandPush = 2, HandSwipe = 3, HandClap = 4, Protect = 5, Screech = 6, PushWave = 7, SlamWave = 8 }

public partial class MonkeyBoss : Enemy
{
    [SerializeField]
    private Physics _leftHand;
    [SerializeField]
    private Physics _rightHand;

    private Vector3 _leftHandStartPos;
    private Vector3 _rightHandStartPos;

    private const float RISE_HEIGHT = 5.0f;

    private bool _rising;
    private bool _rose;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.KoiBoss;
        _speed = 1.0f;
        _health = 300;
        _maxHealth = 300;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = 60.0f;
        _hostileRadius = 30.0f;
        _passiveRadius = 120.0f;
        _maxRadius = 240.0f;
        _specialCooldown = new float[5] { 5.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        _activeStates = new bool[3] { false, false, false };
        _animParm = new int[4] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("screechAttack"),
                    Animator.StringToHash("screechAngry")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = 0.1f;
        //_HostileAI = HostileKoiBoss;
        _PassiveAI = PassiveDoNothing;

        //Setup health bar
        _healthBar.SetMaxHealth(_maxHealth);
        _healthBar.UpdateHealth(_health);

        //Set up hitboxes
        foreach (Hitbox hitbox in GetComponentsInChildren<Hitbox>())
        {
            hitbox.OnTrigger += HitboxTriggered;
            hitbox.OnStay += OnObsticalCollision;
        }

        _leftHandStartPos = _leftHand.transform.localPosition;
        _rightHandStartPos = _rightHand.transform.localPosition;
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
