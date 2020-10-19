using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Assets/Singletons/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    private static EnemyConfig _instance;

    public static EnemyConfig Instance
    {
        get
        {
            if (!_instance)
                _instance = Resources.LoadAll<EnemyConfig>("ScriptableObjectInstances")[0];
            return _instance;
        }
    }

    void OnEnable()
    {
        _instance = this;
    }

    [System.Serializable]
    public class BaseEnemyConfig
    {
        [SerializeField]
        protected float _maxHealth = 100;
        public float MaxHealth => _maxHealth;
        [SerializeField]
        protected float _speed = 1.0f;
        public float Speed => _speed;
        [SerializeField]
        protected float _wanderRadius = 60.0f;
        public float WanderRadius => _wanderRadius;
        [SerializeField]
        protected float _hostileRadius = 30.0f;
        public float HostileRadius => _hostileRadius;
        [SerializeField]
        protected float _passiveRadius = 90.0f;
        public float PassiveRadius => _passiveRadius;
        [SerializeField]
        protected float _maxRadius = 240.0f;
        public float MaxRadius => _maxRadius;
        [SerializeField]
        protected float _pushMultiplier = 1f;
        public float PushMult => _pushMultiplier;

    }

    [System.Serializable]
    public class KoiBossConfig
    {
        //Constant values for Koi boss AI
        [SerializeField]
        private BaseEnemyConfig _base;
        public BaseEnemyConfig Base => _base;

        //Koi dash attack
        [System.Serializable]
        public class KoiDashAttack
        {
            [SerializeField]
            private float _maxDistance = 13.0f; //Max distance to be able to use dash attack
            public float MaxDistance => _maxDistance;
            [SerializeField]
            private float _cooldown = 6.0f;
            public float Cooldown => _cooldown;
            [SerializeField]
            private float _chargeTime = 1.5f; //Time charging dash
            public float ChargeTime => _chargeTime;
            [SerializeField]
            private float _stallTime = 0.2f; //Time stalling after charging before using dash
            public float StallTime => _stallTime;
            [SerializeField]
            private float _damage = 20.0f;
            public float Damage => _damage;
            [SerializeField]
            private float _distance = 30.0f; //If this is changed, telegraph will need to be change manually
            public float Distance => _distance;
        }
        [SerializeField]
        private KoiDashAttack _dashAttack;
        public KoiDashAttack DashAttack => _dashAttack;

        //Koi bubble blast attack
        [System.Serializable]
        public class KoiBubbleBlast
        {
            [SerializeField]
            private float _maxDistance = 20.0f;
            public float MaxDistance => _maxDistance;
            [SerializeField]
            private float _cooldown = 8.0f;
            public float Cooldown => _cooldown;
            [SerializeField]
            private float _transitionDownTime = 1.0f; //Time it takes koi to go underwater before bubble blast
            public float TransitionDownTime => _transitionDownTime;
            [SerializeField]
            private float _transitionUpTime = 1.0f; //Time it takes koi to go back above water before bubble blast
            public float TransitionUpTime => _transitionUpTime;
            [SerializeField]
            private float _chargeTime = 1.0f; //Time charging bubble blast
            public float ChargeTime => _chargeTime;
            [SerializeField]
            private float _stallTime = 0.1f; //Time stalling after charging bubble blast
            public float StallTime => _stallTime;
            [SerializeField]
            private float _damage = 15.0f;
            public float Damage => _damage;
            [SerializeField]
            private float _rechargeTime = 0.5f; //Time charging subsequent shots after first one
            public float RechargeTime => _rechargeTime;
            [SerializeField]
            private float _restallTime = 0.1f; //Stall time of subsequent shots
            public float RestallTime => _restallTime;
        }
        [SerializeField]
        private KoiBubbleBlast _bubbleBlast;
        public KoiBubbleBlast BubbleBlast => _bubbleBlast;

        //Koi bubble attack
        [System.Serializable]
        public class KoiBubbleAttack
        {
            [SerializeField]
            private float _minDistance = 10.0f;
            public float MinDistance => _minDistance;
            [SerializeField]
            private float _cooldown = 3.0f;
            public float Cooldown => _cooldown;
            [SerializeField]
            private float _damage = 10.0f;
            public float Damage => _damage;
        }
        [SerializeField]
        private KoiBubbleAttack _bubbleAttack;
        public KoiBubbleAttack BubbleAttack => _bubbleAttack;

        //Koi underwater dash
        [System.Serializable]
        public class KoiUnderwaterDash
        {
            [SerializeField]
            private float _vulnerabilityTime = 3.0f;
            public float VulnerabilityTime => _vulnerabilityTime;
        }
        [SerializeField]
        private KoiUnderwaterDash _underwaterDash;
        public KoiUnderwaterDash UnderwaterDash => _underwaterDash;

        //Koi underwater bubble blast
        [System.Serializable]
        public class KoiUnderwaterBubbleBlast
        {
            [SerializeField]
            private float _vulnerabilityTime = 2.5f;
            public float VulnerabilityTime => _vulnerabilityTime;
        }
        [SerializeField]
        private KoiUnderwaterBubbleBlast _underwaterBubbleBlast;
        public KoiUnderwaterBubbleBlast UnderwaterBubbleBlast => _underwaterBubbleBlast;

        //Koi underater attack\
        [System.Serializable]
        public class KoiUnderwaterAttack
        {
            [SerializeField]
            private float _maxDistance = 15.0f;
            public float MaxDistance => _maxDistance;
            [SerializeField]
            private float _cooldown = 8.0f;
            public float Cooldown => _cooldown;
            [SerializeField]
            private float _followTime = 4.0f;
            public float FollowTime => _followTime;
            [SerializeField]
            private float _stallTime = 0.5f;
            public float StallTime => _stallTime;
            [SerializeField]
            private float _damage = 20.0f;
            public float Damage => _damage;
            [SerializeField]
            private float _followSpeed = 5.0f;
            public float FollowSpeed => _followSpeed;
            [SerializeField]
            private float _vulnerabilityTime = 3.0f;
            public float VulnerabilityTime => _vulnerabilityTime;
        }
        [SerializeField]
        private KoiUnderwaterAttack _underwaterAttack;
        public KoiUnderwaterAttack UnderwaterAttack => _underwaterAttack;
    }

    [System.Serializable]
    public class BombCrabConfig
    {
        [SerializeField]
        private BaseEnemyConfig _base;
        public BaseEnemyConfig Base => _base;

        [System.Serializable]
        public class BombCrabExplosion
        {
            [SerializeField]
            private float _maxRadius = 5.0f;
            public float MaxRadius => _maxRadius;
            [SerializeField]
            private float _damage = 20.0f;
            public float Damage => _damage;
            [SerializeField]
            private float _damageRadius = 2.5f;
            public float DamageRadius => _damageRadius;
            [SerializeField]
            private float _chargeTime = 1.5f;
            public float ChargeTime => _chargeTime;
            [SerializeField]
            private float _knockback = 1000.0f;
            public float Knockback => _knockback;
        }
        [SerializeField]
        private BombCrabExplosion _explosion;
        public BombCrabExplosion Explosion => _explosion;

    }

    [System.Serializable]
    public class RockCrabConfig
    {
        [SerializeField]
        private BaseEnemyConfig _base;
        public BaseEnemyConfig Base => _base;

        //Fling attack
        [System.Serializable]
        public class RockCrabFlingAttack
        {
            [SerializeField]
            private float _maxDistance = 20.0f; //Max distance to be able to use dash attack
            public float MaxDistance => _maxDistance;
            [SerializeField]
            private float _cooldown = 5.0f;
            public float Cooldown => _cooldown;
            [SerializeField]
            private float _chargeTime = 1.0f; //Time charging dash
            public float ChargeTime => _chargeTime;
            [SerializeField]
            private float _damage = 20.0f;
            public float Damage => _damage;
            [SerializeField]
            private float _distance = 15.0f; //If this is changed, telegraph will need to be change manually
            public float Distance => _distance;
            [SerializeField]
            private float _knockback = 1000.0f;
            public float Knockback => _knockback;
        }
        [SerializeField]
        private RockCrabFlingAttack _flingAttack;
        public RockCrabFlingAttack FlingAttack => _flingAttack;
    }

    [System.Serializable]
    public class FlowerFrogConfig
    {
        [SerializeField]
        private BaseEnemyConfig _base;
        public BaseEnemyConfig Base => _base;

        [System.Serializable]
        public class FlowerFrogToungeLatch
        {
            [SerializeField]
            private float _maxDistance = 20.0f; //Max distance to be able to use dash attack
            public float MaxDistance => _maxDistance;
            [SerializeField]
            private float _cooldown = 5.0f;
            public float Cooldown => _cooldown;
            [SerializeField]
            private float _chargeTime = 1.0f; //Time charging dash
            public float ChargeTime => _chargeTime;
            [SerializeField]
            private float _slowDownEffect = -0.4f;
            public float SlowDownEffect => _slowDownEffect;
            [SerializeField]
            private float _poisonDamage = 3.0f;
            public float PoisonDamage => _poisonDamage;
        }

        [SerializeField]
        private FlowerFrogToungeLatch _toungeLatch;
        public FlowerFrogToungeLatch ToungeLatch => _toungeLatch;


    }

    [System.Serializable]
    public class ClamBossConfig
    {
        [SerializeField]
        private BaseEnemyConfig _base;
        public BaseEnemyConfig Base => _base;

        [System.Serializable]
        public class ClamBossCircleAttack
        {
            [SerializeField]
            private float _maxDistance = 15.0f; //Max distance to be able to use dash attack
            public float MaxDistance => _maxDistance;
            [SerializeField]
            private float _cooldown = 3.0f;
            public float Cooldown => _cooldown;
            [SerializeField]
            private float _chargeTime = 2.0f; //Time charging dash
            public float ChargeTime => _chargeTime;
            [SerializeField]
            private float _damage = 5.0f;
            public float Damage => _damage;
            [SerializeField]
            private int _tentacleAmount = 10;
            public int TentacleAmount => _tentacleAmount;
            [SerializeField]
            private float _circleRadius = 7.0f;
            public float CircleRadius => _circleRadius;
            [SerializeField]
            private float _riseTime = 2.0f;
            public float RiseTime => _riseTime;
            [SerializeField]
            private float _trackTime = 1.0f;
            public float TrackTime => _trackTime;
            [SerializeField]
            private float _attackTime = 2.0f;
            public float AttackTime => _attackTime;

        }

        [SerializeField]
        private ClamBossCircleAttack _circleAttack;
        public ClamBossCircleAttack CircleAttack => _circleAttack;

        [System.Serializable]
        public class ClamBossTrackingAttack
        {
            [SerializeField]
            private float _maxDistance = 25.0f; //Max distance to be able to use dash attack
            public float MaxDistance => _maxDistance;
            [SerializeField]
            private float _cooldown = 5.0f;
            public float Cooldown => _cooldown;
            [SerializeField]
            private float _chargeTime = 1.0f; //Time charging dash
            public float ChargeTime => _chargeTime;
            [SerializeField]
            private float _damage = 5.0f;
            public float Damage => _damage;
            [SerializeField]
            private float _circleRadius = 5.0f;
            public float CircleRadius => _circleRadius;
            [SerializeField]
            private float _riseTime = 2.0f;
            public float RiseTime => _riseTime;
            [SerializeField]
            private float _trackTime = 1.0f;
            public float TrackTime => _trackTime;
            [SerializeField]
            private float _attackTime = 1.0f;
            public float AttackTime => _attackTime;
        }

        [SerializeField]
        private ClamBossTrackingAttack _trackingAttack;
        public ClamBossTrackingAttack TrackingAttack => _trackingAttack;

        [System.Serializable]
        public class ClamBossLineAttack
        {
            [SerializeField]
            private float _maxDistance = 20.0f; //Max distance to be able to use dash attack
            public float MaxDistance => _maxDistance;
            [SerializeField]
            private float _cooldown = 8.0f;
            public float Cooldown => _cooldown;
            [SerializeField]
            private float _chargeTime = 0.25f; //Time charging dash
            public float ChargeTime => _chargeTime;
            [SerializeField]
            private float _damage = 10.0f;
            public float Damage => _damage;
            [SerializeField]
            private float _riseTime = 1.0f;
            public float RiseTime => _riseTime;
            [SerializeField]
            private float _trackTime = 1.0f;
            public float TrackTime => _trackTime;
            [SerializeField]
            private float _attackTime = 1.0f;
            public float AttackTime => _attackTime;
        }

        [SerializeField]
        private ClamBossLineAttack _lineAttack;
        public ClamBossLineAttack LineAttack => _lineAttack;

        [System.Serializable]
        public class ClamBossBurstAttack
        {
            [SerializeField]
            private float _cooldown = 5.0f;
            public float Cooldown => _cooldown;
            [SerializeField]
            private float _duration = 2.0f; //Duration of each burst
            public float Duration => _duration;
            [SerializeField]
            private float _damage = 10.0f;
            public float Damage => _damage;
            [SerializeField]
            private float _attackRadius = 30.0f;
            public float AttackRadius => _attackRadius;
            [SerializeField]
            private int _tentacleAmount = 50;
            public int TentacleAmount => _tentacleAmount;
            [SerializeField]
            private float _riseTime = 1.0f;
            public float RiseTime => _riseTime;
            [SerializeField]
            private float _trackTime = 1.0f;
            public float TrackTime => _trackTime;
            [SerializeField]
            private float _attackTime = 1.0f;
            public float AttackTime => _attackTime;
        }

        [SerializeField]
        private ClamBossBurstAttack _burstAttack;
        public ClamBossBurstAttack BurstAttack => _burstAttack;

        [System.Serializable]
        public class ClamBossOpenAttack
        {
            [SerializeField]
            private float _maxDistance = 25.0f; //Max distance to be able to use dash attack
            public float MaxDistance => _maxDistance;
            [SerializeField]
            private float _cooldown = 15.0f;
            public float Cooldown => _cooldown;
            [SerializeField]
            private float _chargeTime = 2.0f; //Time charging dash
            public float ChargeTime => _chargeTime;
            [SerializeField]
            private float _waitTime = 2.0f; //Time charging dash
            public float WaitTime => _waitTime;

            [System.Serializable]
            public class ClamBossDragonAttack
            {
                [SerializeField]
                private float _chargeTime = 2.0f; //Time charging dash
                public float ChargeTime => _chargeTime;
                [SerializeField]
                private float _poisonDamage = 4.0f;
                public float PoisonDamage => _poisonDamage;
                [SerializeField]
                private float _circleRadius = 6.0f;
                public float CircleRadius => _circleRadius;
                [SerializeField]
                private int _smokeClouds = 10;
                public int SmokeClouds => _smokeClouds;
                [SerializeField]
                private float _distance = 35.0f;
                public float Distance => _distance;
                [SerializeField]
                private float _duration = 2.0f;
                public float Duration => _duration; 
            }

            [SerializeField]
            private ClamBossDragonAttack _dragonAttack;
            public ClamBossDragonAttack DragonAttack => _dragonAttack;

            [System.Serializable]
            public class ClamBossWaterSpoutAttack
            {
                [SerializeField]
                private float _chargeTime = 0.5f; //Time charging dash
                public float ChargeTime => _chargeTime;
                [SerializeField]
                private float _duration = 5.0f;
                public float Duration => _duration;
                [SerializeField]
                private float _stallTime = 0.5f; 
                public float StallTime => _stallTime;
                [SerializeField]
                private float _damagePerSecond = 15.0f;
                public float DamagePerSecond => _damagePerSecond;
            }

            [SerializeField]
            private ClamBossWaterSpoutAttack _waterSpoutAttack;
            public ClamBossWaterSpoutAttack WaterSpoutAttack => _waterSpoutAttack;

            [System.Serializable]
            public class ClamBossBirdAttack
            {
                [SerializeField]
                private float _duration = 4.0f; //Time charging dash
                public float Duration => _duration;
                [SerializeField]
                private float _damage = 10.0f;
                public float Damage => _damage;
                [SerializeField]
                private float _rockSpeed = 0.5f;
                public float RockSpeed => _rockSpeed;
                [SerializeField]
                private float _knockback = 300.0f;
                public float Knockback => _knockback;
            }

            [SerializeField]
            private ClamBossBirdAttack _birdAttack;
            public ClamBossBirdAttack BirdAttack => _birdAttack;

        }

        [SerializeField]
        private ClamBossOpenAttack _openAttack;
        public ClamBossOpenAttack OpenAttack => _openAttack;
    }

    [SerializeField]
    private KoiBossConfig _koiBoss;
    public KoiBossConfig KoiBoss => _koiBoss;

    [SerializeField]
    private RockCrabConfig _rockCrab;
    public RockCrabConfig RockCrab => _rockCrab;

    [SerializeField]
    private FlowerFrogConfig _flowerFrog;
    public FlowerFrogConfig FlowerFrog => _flowerFrog;

    [SerializeField]
    private ClamBossConfig _clamBoss;
    public ClamBossConfig ClamBoss => _clamBoss;

    [SerializeField]
    private BombCrabConfig _bombCrab;
    public BombCrabConfig BombCrab => _bombCrab;

}
