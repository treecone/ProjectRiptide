using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CarpAnim { SwimSpeed = 2, Dive = 3, Shoot = 4, UAttack = 5 };
public enum KoiAttackState { TripleDash = 2, BubbleBlast = 4, UnderwaterAttack = 3, BubbleAttack = 3 }

public partial class KoiBoss : Enemy
{
    [SerializeField]
    private ParticleSystem _dashParticles;
    [SerializeField]
    private GameObject _bubbleBrothPrefab;
    [SerializeField]
    private float _waterLevel;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.KoiBoss;
        _speed = EnemyConfig.Instance.KoiBoss.Base.BaseSpeed;
        _health = EnemyConfig.Instance.KoiBoss.Base.MaxHealth;
        _maxHealth = EnemyConfig.Instance.KoiBoss.Base.MaxHealth;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = EnemyConfig.Instance.KoiBoss.Base.WanderRadius;
        _hostileRadius = EnemyConfig.Instance.KoiBoss.Base.HostileRadius;
        _passiveRadius = EnemyConfig.Instance.KoiBoss.Base.PassiveRadius;
        _maxRadius = EnemyConfig.Instance.KoiBoss.Base.MaxRadius;
        _specialCooldown = new float[5] { 5.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        _activeStates = new bool[3] { false, false, false };
        _animParm = new int[6] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("swimSpeed"),
                    Animator.StringToHash("dive"),
                    Animator.StringToHash("shoot"),
                    Animator.StringToHash("uAttack")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = EnemyConfig.Instance.KoiBoss.Base.PushMult;
        _HostileAI = HostileKoiBoss;
        _PassiveAI = PassiveWanderRadius;

        _dashParticles.Stop();

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
    }

    /// <summary>
    /// Spawns bubble broth at detect position of koi boss
    /// </summary>
    protected void SpawnBubbleBroth()
    {
        Instantiate(_bubbleBrothPrefab, new Vector3(_detectPosition.position.x, _waterLevel, _detectPosition.position.z), transform.rotation);
    }
}
