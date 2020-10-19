using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BombCrabAnim { Explode = 2};

public partial class BombCrab : Enemy
{
    private bool _exploding;
    private float _explodingTimer;
    [SerializeField]
    private ParticleSystem _explosionParticles;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.BombCrab;
        _speed = EnemyConfig.Instance.BombCrab.Base.Speed;
        _health = EnemyConfig.Instance.BombCrab.Base.MaxHealth;
        _maxHealth = EnemyConfig.Instance.BombCrab.Base.MaxHealth;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = EnemyConfig.Instance.BombCrab.Base.WanderRadius;
        _hostileRadius = EnemyConfig.Instance.BombCrab.Base.HostileRadius;
        _passiveRadius = EnemyConfig.Instance.BombCrab.Base.PassiveRadius;
        _maxRadius = EnemyConfig.Instance.BombCrab.Base.MaxRadius;
        _specialCooldown = new float[1] { 0.1f };
        _activeStates = new bool[3] { false, false, false };
        _animParm = new int[3] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("explode")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = EnemyConfig.Instance.BombCrab.Base.PushMult;
        _HostileAI = HostileBombCrab;
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
        base.Update();
        if(_dying && _exploding)
        {
            if(!DoActionQueue())
            {
                Destroy(transform.GetChild(0).gameObject);
                _exploding = false;
            }
        }
    }

    protected override void OnDeath()
    {
        if (_exploding)
        {
            ClearHitboxes();
            ClearTelegraphs();
            _dying = true;
            _isInvincible = true;
            _deathTimer = 0;
            //destroy hitboxes
            foreach (Hitbox hitbox in GetComponentsInChildren<Hitbox>())
            {
                Destroy(hitbox.gameObject);
            }
            _canvas.SetActive(false);
            transform.Find("StatusEffectIcons").gameObject.SetActive(false);
            //Destroy body of bomb crab
            Destroy(transform.GetChild(0).gameObject);
            _exploding = false;
        }
        else
        {
            ClearHitboxes();
            ClearTelegraphs();
            _dying = true;
            _isInvincible = true;
            _deathTimer = 0;
            //destroy hitboxes
            foreach (Hitbox hitbox in GetComponentsInChildren<Hitbox>())
            {
                Destroy(hitbox.gameObject);
            }
            _canvas.SetActive(false);
            if (_lootType != "")
            {
                GameObject lootboxClone = Instantiate(_lootboxPrefab);
                lootboxClone.transform.position = transform.position;
                lootboxClone.transform.position = new Vector3(lootboxClone.transform.position.x, -0.3f, lootboxClone.transform.position.z);
                Lootbox lootbox = lootboxClone.GetComponent<Lootbox>();
                lootbox.dropType = _lootType;
                lootbox.GenerateItems();
            }
            transform.Find("StatusEffectIcons").gameObject.SetActive(false);
            _exploding = true;
            _actionQueue.Enqueue(BombCrabChargeExplosion);
            _actionQueue.Enqueue(BombCrabExplode);
        }
    }
}
