using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Assets/Singletons/ActiveAbilityDB")]
public class ActiveAbilityDB : ScriptableObject
{
    private static ActiveAbilityDB _instance;

    public static ActiveAbilityDB Instance
    {
        get
        {
            if (!_instance)
                _instance = Resources.LoadAll<ActiveAbilityDB>("ScriptableObjectInstances")[0];
            return _instance;
        }
    }

    [SerializeField]
    private bool _resetOnEnable;
    public void OnEnable()
    {
        if(_resetOnEnable)
        {
            FillDefault();
        }
    }
    public List<string> skillNames;

    private void FillDefault()
    {
        skillNames = new List<string>();
        for(int i = 0; i < Enum.GetNames(typeof(SkillType)).Length; i++)
        {
            skillNames.Add(((SkillType)i).ToString());
        }
    }
    [System.Serializable]
    public class SmallDashConfig
    {
        [SerializeField]
        private float _cooldown = 8.0f;
        public float Cooldown => _cooldown;
        [SerializeField]
        private float _distance = 8.0f;
        public float Distance => _distance;
        [SerializeField]
        private float _time = 0.5f;
        public float Time => _time;
    }
    [System.Serializable]
    public class DashConfig
    {
        [SerializeField]
        private float _cooldown = 7.0f;
        public float Cooldown => _cooldown;
        [SerializeField]
        private float _distance = 10.0f;
        public float Distance => _distance;
        [SerializeField]
        private float _time = 0.5f;
        public float Time => _time;
    }
    [System.Serializable]
    public class StatusSkillConfig
    {
        [SerializeField]
        private float _cooldown = 20.0f;
        public float Cooldown => _cooldown;
        [SerializeField]
        private float _duration = 5.0f;
        public float Duration => _duration;
        [SerializeField]
        private float _level = 2f;
        public float Level => _level;
    }
    [System.Serializable]
    public class BubbleFieldConfig
    {
        [SerializeField]
        private float _cooldown = 25.0f;
        public float Cooldown => _cooldown;
        [SerializeField]
        private float _fieldDuration = 5.0f;
        public float FieldDuration => _fieldDuration;
        [SerializeField]
        private float _effectDuration = 5.0f;
        public float EffectDuration => _effectDuration;
    }

    [System.Serializable]
    public class StopMovementConfig
    {
        [SerializeField]
        private float _cooldown = 10.0f;
        public float Cooldown => _cooldown;
    }
    [System.Serializable]
    public class DefenseBoostConfig
    {
        [SerializeField]
        private float _cooldown = 20.0f;
        public float Cooldown => _cooldown;
        [SerializeField]
        private float _duration = 5.0f;
        public float Duration => _duration;
        [SerializeField]
        private float _armorLevel = 2f;
        public float ArmorLevel => _armorLevel;
        [SerializeField]
        private float _hardinessLevel = 2f;
        public float HardinessLevel => _hardinessLevel;
    }

    [System.Serializable]
    public class DamageSkillConfig
    {
        [SerializeField]
        private float _cooldown = 20.0f;
        public float Cooldown => _cooldown;
        [SerializeField]
        private float _damage = 5.0f;
        public float Damage => _damage;
    }

    [System.Serializable]
    public class RamConfig
    {
        [SerializeField]
        private float _cooldown = 20.0f;
        public float Cooldown => _cooldown;
        [SerializeField]
        private float _damage = 5.0f;
        public float Damage => _damage;
        [SerializeField]
        private float _distance = 20.0f;
        public float Distance => _distance;
    }

    [System.Serializable]
    public class StunShotConfig
    {
        [SerializeField]
        private float _cooldown = 20.0f;
        public float Cooldown => _cooldown;
        [SerializeField]
        private float _damage = 5.0f;
        public float Damage => _damage;
        [SerializeField]
        private float _stunDuration = 5.0f;
        public float StunDuration => _stunDuration;
    }

    [Header("Config Values")]
    [SerializeField]
    private SmallDashConfig _smallDash;
    public SmallDashConfig SmallDash => _smallDash;
    [SerializeField]
    private DashConfig _dash;
    public DashConfig Dash => _dash;
    [SerializeField]
    private StatusSkillConfig _smallManuverabilityBoost;
    public StatusSkillConfig SmallManuverabilityBoost => _smallManuverabilityBoost;
    [SerializeField]
    private StatusSkillConfig _mediumManuverabilityBoost;
    public StatusSkillConfig MediumManuverabilityBoost => _mediumManuverabilityBoost;
    [SerializeField]
    private StatusSkillConfig _largeManuverabilityBoost;
    public StatusSkillConfig LargeManuverabilityBoost => _largeManuverabilityBoost;
    [SerializeField]
    private BubbleFieldConfig _bubbleField;
    public BubbleFieldConfig BubbleField => _bubbleField;
    [SerializeField]
    private BubbleFieldConfig _strongBubbleField;
    public BubbleFieldConfig StrongBubbleField => _strongBubbleField;
    [SerializeField]
    private StopMovementConfig _stopMovement;
    public StopMovementConfig StopMovement => _stopMovement;
    [SerializeField]
    private StopMovementConfig _strongStopMovement;
    public StopMovementConfig StrongStopMovement => _strongStopMovement;
    [SerializeField]
    private StatusSkillConfig _smallSpeedBoost;
    public StatusSkillConfig SmallSpeedBoost => _smallSpeedBoost;
    [SerializeField]
    private StatusSkillConfig _mediumSpeedBoost;
    public StatusSkillConfig MediumSpeedBoost => _mediumSpeedBoost;
    [SerializeField]
    private StatusSkillConfig _largeSpeedBoost;
    public StatusSkillConfig LargeSpeedBoost => _largeSpeedBoost;
    [SerializeField]
    private StatusSkillConfig _smallRegeneration;
    public StatusSkillConfig SmallRegeneration => _smallRegeneration;
    [SerializeField]
    private StatusSkillConfig _mediumRegeneration;
    public StatusSkillConfig MediumRegeneration => _mediumRegeneration;
    [SerializeField]
    private StatusSkillConfig _largeRegeneration;
    public StatusSkillConfig LargeRegeneration => _largeRegeneration;
    [SerializeField]
    private DefenseBoostConfig _smallDefenseBoost;
    public DefenseBoostConfig SmallDefenseBoost => _smallDefenseBoost;
    [SerializeField]
    private DefenseBoostConfig _largeDefenseBoost;
    public DefenseBoostConfig LargeDefenseBoost => _largeDefenseBoost;
    [SerializeField]
    private DamageSkillConfig _weakSteelMine;
    public DamageSkillConfig WeakSteelMine => _weakSteelMine;
    [SerializeField]
    private DamageSkillConfig _mediumSteelMine;
    public DamageSkillConfig MediumSteelMine => _mediumSteelMine;
    [SerializeField]
    private DamageSkillConfig _strongSteelMine;
    public DamageSkillConfig StrongSteelMine => _strongSteelMine;
    [SerializeField]
    private DamageSkillConfig _smallCounter;
    public DamageSkillConfig SmallCounter => _smallCounter;
    [SerializeField]
    private DamageSkillConfig _mediumCounter;
    public DamageSkillConfig MediumCounter => _mediumCounter;
    [SerializeField]
    private DamageSkillConfig _largeCounter;
    public DamageSkillConfig LargeCounter => _largeCounter;
    [SerializeField]
    private RamConfig _smallRam;
    public RamConfig SmallRam => _smallRam;
    [SerializeField]
    private RamConfig _mediumRam;
    public RamConfig MediumRam => _mediumRam;
    [SerializeField]
    private RamConfig _largeRam;
    public RamConfig LargeRam => _largeRam;
    [SerializeField]
    private StatusSkillConfig _smallSeaglassSpeed;
    public StatusSkillConfig SmallSeaglassSpeed => _smallSeaglassSpeed;
    [SerializeField]
    private StatusSkillConfig _mediumSeaglassSpeed;
    public StatusSkillConfig MediumSeaglassSpeed => _mediumSeaglassSpeed;
    [SerializeField]
    private StatusSkillConfig _largeSeaglassSpeed;
    public StatusSkillConfig LargeSeaglassSpeed => _largeSeaglassSpeed;
    [SerializeField]
    private StatusSkillConfig _smallInvulnerability;
    public StatusSkillConfig SmallInvulnerability => _smallInvulnerability;
    [SerializeField]
    private StatusSkillConfig _mediumInvulnerability;
    public StatusSkillConfig MediumInvulnerability => _mediumInvulnerability;
    [SerializeField]
    private StatusSkillConfig _largeInvulnerability;
    public StatusSkillConfig LargeInvulnerability => _largeInvulnerability;
    [SerializeField]
    private DamageSkillConfig _spreadShot;
    public DamageSkillConfig SpreadShot => _spreadShot;
    [SerializeField]
    private DamageSkillConfig _rapidShotFour;
    public DamageSkillConfig RapidShotFour => _rapidShotFour;
    [SerializeField]
    private DamageSkillConfig _rapidShotEight;
    public DamageSkillConfig RapidShotEight => _rapidShotEight;
    [SerializeField]
    private DamageSkillConfig _weakBigShot;
    public DamageSkillConfig WeakBigShot => _weakBigShot;
    [SerializeField]
    private DamageSkillConfig _mediumBigShot;
    public DamageSkillConfig MediumBigShot => _mediumBigShot;
    [SerializeField]
    private DamageSkillConfig _strongBigShot;
    public DamageSkillConfig StrongBigShot => _strongBigShot;
    [SerializeField]
    private DamageSkillConfig _smallFireworkCircle;
    public DamageSkillConfig SmallFireworkCircle => _smallFireworkCircle;
    [SerializeField]
    private DamageSkillConfig _largeFireworkCircle;
    public DamageSkillConfig LargeFireworkCircle => _largeFireworkCircle;
    [SerializeField]
    private StatusSkillConfig _poisonCloud;
    public StatusSkillConfig PoisonCloud => _poisonCloud;
    [SerializeField]
    private StatusSkillConfig _strongPoisonCloud;
    public StatusSkillConfig StrongPoisonCloud => _strongPoisonCloud;
    [SerializeField]
    private StunShotConfig _stunShot;
    public StunShotConfig StunShot => _stunShot;
    [SerializeField]
    private StunShotConfig _strongStunShot;
    public StunShotConfig StrongStunShot => _strongStunShot;
    [SerializeField]
    private DamageSkillConfig _weakFlamethrower;
    public DamageSkillConfig WeakFlamethrower => _weakFlamethrower;
    [SerializeField]
    private DamageSkillConfig _mediumFlamethrower;
    public DamageSkillConfig MediumFlamethrower => _mediumFlamethrower;
    [SerializeField]
    private DamageSkillConfig _strongFlamethrower;
    public DamageSkillConfig StrongFlamethrower => _strongFlamethrower;
    [SerializeField]
    private StatusSkillConfig _weakDefenseBoost;
    public StatusSkillConfig WeakDefenseBoost => _weakDefenseBoost;
    [SerializeField]
    private DamageSkillConfig _veryWeakBigShot;
    public DamageSkillConfig VeryWeakBigShot => _veryWeakBigShot;

}
