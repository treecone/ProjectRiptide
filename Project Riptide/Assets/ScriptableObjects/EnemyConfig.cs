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
    public class KoiBossConfig
    {
        //Constant values for Koi boss AI
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

    [SerializeField]
    private KoiBossConfig _koiBoss;
    public KoiBossConfig KoiBoss => _koiBoss;

}
