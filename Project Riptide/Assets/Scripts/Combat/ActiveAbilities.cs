﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public delegate bool Skill(Enemy enemy);
public delegate bool OvertimeSkill(Enemy enemy, ref float time);
public delegate void CallbackIndex(int i);
public enum SkillType { SmallDash = 0, Dash = 1,
    SmallManuverabilityBoost = 2, MediumManuverabilityBoost = 3, LargeManuverabilityBoost = 4,
    BubbleField = 5, StrongBubbleField = 6, StopMovement = 7, StrongStopMovement = 8, SmallSpeedBoost = 9,
    MediumSpeedBoost = 10, LargeSpeedBoost = 11, SmallRegeneration = 12, MediumRegeneration = 13, LargeRegeneration = 14,
    SmallDefenseBoost = 15, LargeDefenseBoost = 16, WeakSteelMine = 17, MediumSteelMine = 18, StrongSteelMine = 19,
    SmallCounter = 20, MediumCounter = 21, LargeCounter = 22, SmallRam = 23, MediumRam = 24, LargeRam = 25, SmallSeaglassSpeed = 26,
    MediumSeaglassSpeed = 27, LargeSeaglassSpeed = 28, SmallInvulnerability = 29, MediumInvulnerability = 30, LargeInvulnerability = 31,
    SpreadShot = 32, RapidShotFour = 33, RapidShotEight = 34, WeakBigShot = 35, MediumBigShot = 36, StrongBigShot = 37,
    SmallFireworkCircle = 38, LargeFireworkCircle = 39, PoisonCloud = 40, StrongPoisonCloud = 41, StunShot = 42, StrongStunShot = 43,
    WeakFlameThrower = 44, MediumFlameThrower = 45, StrongFlameThrower = 46, WeakDefenseBoost = 47, VeryWeakBigShot = 48
}

public class ActiveAbilities : MonoBehaviour
{
    private const int SKILL_AMOUNT = 3;

    private ShipMovement _movementScript;
    private CannonFire _cannonFireScript;
    public CannonFire CannonFireScript => _cannonFireScript;
    [SerializeField]
    private InputManager _inputManager;

    //[SerializeField]
    private SkillType[] _skillType = new SkillType[SKILL_AMOUNT];

    [SerializeField]
    private Button[] _buttons = new Button[SKILL_AMOUNT];
    private Slider[] _sliders = new Slider[SKILL_AMOUNT];

    private ActiveSkill[] _skill = new ActiveSkill[SKILL_AMOUNT];

    private bool _rightEnemy;
    public bool RightEnemy
    {
        get { return _rightEnemy; }
        set { _rightEnemy = value; }
    }

    [SerializeField]
    private GameObject _hitbox;
    public GameObject HitboxPrefab => _hitbox;
    private List<Hitbox> _hitboxes;

    private StatusEffects _playerStatusEffects;
    public StatusEffects PlayerStatusEffects => _playerStatusEffects;

    // Start is called before the first frame update
    void Start()
    {
        _movementScript = GetComponent<ShipMovement>();
        _cannonFireScript = GetComponent<CannonFire>();
        _playerStatusEffects = GetComponent<StatusEffects>();
        //Set up skill based on inital values
        for (int i = 0; i < SKILL_AMOUNT; i++)
        {
            if(_skill[i] == null)
                SetActiveSkill(i, _skillType[i]);
            _sliders[i] = _buttons[i].GetComponentInChildren<Slider>();
            _sliders[i].gameObject.SetActive(false);
        }
        _hitboxes = new List<Hitbox>();
    }

    // Update is called once per frame
    void Update()
    {
        //Do cooldown if there is any
        for (int i = 0; i < SKILL_AMOUNT; i++)
        {
            if(_skill[i].InCooldown)
            {
                _skill[i].DoCooldown(Time.deltaTime);
                _sliders[i].value = _skill[i].CurrCooldown / _skill[i].MaxCooldown;
            }

            if(_skill[i].Active)
            {
                _skill[i].DoOvertimeActive();
            }
        }

        //Check if buttons should be enabled or disabled
        if (_buttons[0].gameObject.activeSelf && !_inputManager.InCombatMode)
        {
            for(int i = 0; i < SKILL_AMOUNT; i++)
            {
                _buttons[i].gameObject.SetActive(false);
            }
        }
        if (!_buttons[0].gameObject.activeSelf && _inputManager.InCombatMode)
        {
            for (int i = 0; i < SKILL_AMOUNT; i++)
            {
                _buttons[i].gameObject.SetActive(true);
            }
        }

        if(_inputManager.InCombatMode)
        {
            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                _buttons[0].onClick?.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _buttons[1].onClick?.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _buttons[2].onClick?.Invoke();
            }
        }
    }

    /// <summary>
    /// Activates skill if possible
    /// </summary>
    /// <param name="i">Index of skill to activate</param>
    public void ActivateSkill(int i)
    {
        if (_skill[i] != null && !_skill[i].InCooldown)
        {
            if (_skill[i].NeedsEnemy)
            {
                Enemy rightEnemy = _inputManager.CheckTargetEnemy(transform.right);
                Enemy leftEnemy = _inputManager.CheckTargetEnemy(-transform.right);
                if (rightEnemy != null && !rightEnemy.IsInvincible && (leftEnemy == null || leftEnemy.IsInvincible || _inputManager.EnemyCompare(rightEnemy, leftEnemy)))
                {
                    _rightEnemy = true;
                    _skill[i].Activate(rightEnemy);
                }
                else if (leftEnemy != null && !leftEnemy.IsInvincible)
                {
                    _rightEnemy = false;
                    _skill[i].Activate(leftEnemy);
                }
            }
            else
            {
                _skill[i].Activate();
            }

            //Check if skill activated successfully
            if (_skill[i].InCooldown)
            {
                //Change button to darken
                ColorBlock colors = _buttons[i].colors;
                colors.normalColor = new Color32(150, 150, 150, 255);
                colors.highlightedColor = new Color32(150, 150, 150, 255);
                _buttons[i].colors = colors;
                _sliders[i].gameObject.SetActive(true);
                _sliders[i].value = 0;
            }
        }
    }

    /// <summary>
    /// Resets button's color after cooldown ends
    /// </summary>
    /// <param name="i">Index of button</param>
    private void ResetButton(int i)
    {
        //Change button to normal
        ColorBlock colors = _buttons[i].colors;
        colors.normalColor = new Color32(255, 255, 255, 255);
        colors.highlightedColor = new Color32(255, 255, 255, 255);
        _buttons[i].colors = colors;
        _sliders[i].gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets active skill to specified slot
    /// </summary>
    /// <param name="i">Index of skill</param>
    /// <param name="type">Type of skill</param>
    public void SetActiveSkill(int i, SkillType type)
    {
        _skill[i] = GetActiveSkill(type, i);
        if (_skill[i] != null)
        {
            _buttons[i].GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("ActiveAbilities/Buttons/"+_skill[i].Name);
            _skill[i].OnCooldownEnd += ResetButton;
        }
        else
        {
            _buttons[i].GetComponentInChildren<Image>().sprite = null;
            Debug.LogError("No active skill of that type found.");
        }
        _buttons[i].gameObject.SetActive(false);
    }

    /// <summary>
    /// Gets active skill based on type
    /// </summary>
    /// <param name="type">Type of skill</param>
    /// <returns>Active skill</returns>
    private ActiveSkill GetActiveSkill(SkillType type, int index)
    {
        switch(type)
        {
            case SkillType.SmallDash:
                return new ActiveSkill("ShortDash", SmallDash, ActiveAbilityDB.Instance.SmallDash.Cooldown, false, index);
            case SkillType.Dash:
                return new ActiveSkill("Dash", Dash, ActiveAbilityDB.Instance.Dash.Cooldown, false, index);
            case SkillType.SmallManuverabilityBoost:
                return new StatusSkill("AgilityBoost", this, ActiveAbilityDB.Instance.SmallManuverabilityBoost.Cooldown, false, index, StatusType.Turning, ActiveAbilityDB.Instance.SmallManuverabilityBoost.Duration, ActiveAbilityDB.Instance.SmallManuverabilityBoost.Level);
            case SkillType.MediumManuverabilityBoost:
                return new StatusSkill("AgilityBoost", this, ActiveAbilityDB.Instance.MediumManuverabilityBoost.Cooldown, false, index, StatusType.Turning, ActiveAbilityDB.Instance.MediumManuverabilityBoost.Duration, ActiveAbilityDB.Instance.MediumManuverabilityBoost.Level);
            case SkillType.LargeManuverabilityBoost:
                return new StatusSkill("AgilityBoost", this, ActiveAbilityDB.Instance.LargeManuverabilityBoost.Cooldown, false, index, StatusType.Turning, ActiveAbilityDB.Instance.LargeManuverabilityBoost.Duration, ActiveAbilityDB.Instance.LargeManuverabilityBoost.Level);
            case SkillType.BubbleField:
                return new BubbleFieldSkill("BubbleField", this, ActiveAbilityDB.Instance.BubbleField.Cooldown, false, index, ActiveAbilityDB.Instance.BubbleField.FieldDuration, ActiveAbilityDB.Instance.BubbleField.EffectDuration);
            case SkillType.StrongBubbleField:
                return new BubbleFieldSkill("BubbleField", this, ActiveAbilityDB.Instance.StrongBubbleField.Cooldown, false, index, ActiveAbilityDB.Instance.StrongBubbleField.FieldDuration, ActiveAbilityDB.Instance.StrongBubbleField.EffectDuration);
            case SkillType.StopMovement:
                return new ActiveSkill("Stop", StopMovement, ActiveAbilityDB.Instance.StopMovement.Cooldown, false, index);
            case SkillType.StrongStopMovement:
                return new ActiveSkill("Stop", StopMovement, ActiveAbilityDB.Instance.StrongStopMovement.Cooldown, false, index);
            case SkillType.SmallSpeedBoost:
                return new StatusSkill("SpeedBoost", this, ActiveAbilityDB.Instance.SmallSpeedBoost.Cooldown, false, index, StatusType.Speed, ActiveAbilityDB.Instance.SmallSpeedBoost.Duration, ActiveAbilityDB.Instance.SmallSpeedBoost.Level);
            case SkillType.MediumSpeedBoost:
                return new StatusSkill("SpeedBoost", this, ActiveAbilityDB.Instance.MediumSpeedBoost.Cooldown, false, index, StatusType.Speed, ActiveAbilityDB.Instance.MediumSpeedBoost.Duration, ActiveAbilityDB.Instance.MediumSpeedBoost.Level);
            case SkillType.LargeSpeedBoost:
                return new StatusSkill("SpeedBoost", this, ActiveAbilityDB.Instance.LargeSpeedBoost.Cooldown, false, index, StatusType.Speed, ActiveAbilityDB.Instance.LargeSpeedBoost.Duration, ActiveAbilityDB.Instance.LargeSpeedBoost.Level);
            case SkillType.SmallRegeneration:
                return new StatusSkill("Regeneration", this, ActiveAbilityDB.Instance.SmallRegeneration.Cooldown, false, index, StatusType.Regeneration, ActiveAbilityDB.Instance.SmallRegeneration.Duration, ActiveAbilityDB.Instance.SmallRegeneration.Level);
            case SkillType.MediumRegeneration:
                return new StatusSkill("Regeneration", this, ActiveAbilityDB.Instance.MediumRegeneration.Cooldown, false, index, StatusType.Regeneration, ActiveAbilityDB.Instance.MediumRegeneration.Duration, ActiveAbilityDB.Instance.MediumRegeneration.Level);
            case SkillType.LargeRegeneration:
                return new StatusSkill("Regeneration", this, ActiveAbilityDB.Instance.LargeRegeneration.Cooldown, false, index, StatusType.Regeneration, ActiveAbilityDB.Instance.LargeRegeneration.Duration, ActiveAbilityDB.Instance.LargeRegeneration.Level);
            case SkillType.SmallDefenseBoost:
                return new ActiveSkill("DefenseBoost", SmallDefenseBoost, ActiveAbilityDB.Instance.SmallDefenseBoost.Cooldown, false, index);
            case SkillType.LargeDefenseBoost:
                return new ActiveSkill("DefenseBoost", LargeDefenseBoost, ActiveAbilityDB.Instance.LargeDefenseBoost.Cooldown, false, index);
            case SkillType.WeakSteelMine:
                return new SteelMineSkill("TripleMine", this, ActiveAbilityDB.Instance.WeakSteelMine.Cooldown, false, index, ActiveAbilityDB.Instance.WeakSteelMine.Damage);
            case SkillType.MediumSteelMine:
                return new SteelMineSkill("TripleMine", this, ActiveAbilityDB.Instance.MediumSteelMine.Cooldown, false, index, ActiveAbilityDB.Instance.MediumSteelMine.Damage);
            case SkillType.StrongSteelMine:
                return new SteelMineSkill("TripleMine", this, ActiveAbilityDB.Instance.StrongSteelMine.Cooldown, false, index, ActiveAbilityDB.Instance.StrongSteelMine.Damage);
            case SkillType.SmallCounter:
                return new CounterSkill("Counter", this, ActiveAbilityDB.Instance.SmallCounter.Cooldown, false, index, ActiveAbilityDB.Instance.SmallCounter.Damage);
            case SkillType.MediumCounter:
                return new CounterSkill("Counter", this, ActiveAbilityDB.Instance.MediumCounter.Cooldown, false, index, ActiveAbilityDB.Instance.MediumCounter.Damage);
            case SkillType.LargeCounter:
                return new CounterSkill("Counter", this, ActiveAbilityDB.Instance.LargeCounter.Cooldown, false, index, ActiveAbilityDB.Instance.LargeCounter.Damage);
            case SkillType.SmallRam:
                return new RamSkill("Ram", this, ActiveAbilityDB.Instance.SmallRam.Cooldown, false, index, ActiveAbilityDB.Instance.SmallRam.Damage, ActiveAbilityDB.Instance.SmallRam.Distance);
            case SkillType.MediumRam:
                return new RamSkill("Ram", this, ActiveAbilityDB.Instance.MediumRam.Cooldown, false, index, ActiveAbilityDB.Instance.MediumRam.Damage, ActiveAbilityDB.Instance.MediumRam.Distance);
            case SkillType.LargeRam:
                return new RamSkill("Ram", this, ActiveAbilityDB.Instance.LargeRam.Cooldown, false, index, ActiveAbilityDB.Instance.LargeRam.Damage, ActiveAbilityDB.Instance.LargeRam.Distance);
            case SkillType.SmallSeaglassSpeed:
                return new StatusSkill("SpeedBoost", this, ActiveAbilityDB.Instance.SmallSeaglassSpeed.Cooldown, false, index, StatusType.Speed, ActiveAbilityDB.Instance.SmallSeaglassSpeed.Duration, ActiveAbilityDB.Instance.SmallSeaglassSpeed.Level);
            case SkillType.MediumSeaglassSpeed:
                return new StatusSkill("SpeedBoost", this, ActiveAbilityDB.Instance.MediumSeaglassSpeed.Cooldown, false, index, StatusType.Speed, ActiveAbilityDB.Instance.MediumSeaglassSpeed.Duration, ActiveAbilityDB.Instance.MediumSeaglassSpeed.Level);
            case SkillType.LargeSeaglassSpeed:
                return new StatusSkill("SpeedBoost", this, ActiveAbilityDB.Instance.LargeSeaglassSpeed.Cooldown, false, index, StatusType.Speed, ActiveAbilityDB.Instance.LargeSeaglassSpeed.Duration, ActiveAbilityDB.Instance.LargeSeaglassSpeed.Level);
            case SkillType.SmallInvulnerability:
                return new StatusSkill("Invincible", this, ActiveAbilityDB.Instance.SmallInvulnerability.Cooldown, false, index, StatusType.Armor, ActiveAbilityDB.Instance.SmallInvulnerability.Duration, ActiveAbilityDB.Instance.SmallInvulnerability.Level);
            case SkillType.MediumInvulnerability:
                return new StatusSkill("Invincible", this, ActiveAbilityDB.Instance.MediumInvulnerability.Cooldown, false, index, StatusType.Armor, ActiveAbilityDB.Instance.MediumInvulnerability.Duration, ActiveAbilityDB.Instance.MediumInvulnerability.Level);
            case SkillType.LargeInvulnerability:
                return new StatusSkill("Invincible", this, ActiveAbilityDB.Instance.LargeInvulnerability.Cooldown, false, index, StatusType.Armor, ActiveAbilityDB.Instance.LargeInvulnerability.Duration, ActiveAbilityDB.Instance.LargeInvulnerability.Level);
            case SkillType.SpreadShot:
                return new SpecialShotSkill("SpreadShot", this, ActiveAbilityDB.Instance.SpreadShot.Cooldown, true, index, 4, ActiveAbilityDB.Instance.SpreadShot.Damage, 0.2f, 20, 0);
            case SkillType.RapidShotFour:
                return new RapidShotSkill("RapidShot", this, ActiveAbilityDB.Instance.RapidShotFour.Cooldown, true, index, 4, 0.3f, ActiveAbilityDB.Instance.RapidShotFour.Damage);
            case SkillType.RapidShotEight:
                return new RapidShotSkill("RapidShot", this, ActiveAbilityDB.Instance.RapidShotEight.Cooldown, true, index, 8, 0.15f, ActiveAbilityDB.Instance.RapidShotEight.Damage);
            case SkillType.WeakBigShot:
                return new SpecialShotSkill("BigShot", this, ActiveAbilityDB.Instance.WeakBigShot.Cooldown, true, index, 0, ActiveAbilityDB.Instance.WeakBigShot.Damage, 2, 0, 0);
            case SkillType.MediumBigShot:
                return new SpecialShotSkill("BigShot", this, ActiveAbilityDB.Instance.MediumBigShot.Cooldown, true, index, 0, ActiveAbilityDB.Instance.MediumBigShot.Damage, 2, 0, 0);
            case SkillType.StrongBigShot:
                return new SpecialShotSkill("BigShot", this, ActiveAbilityDB.Instance.StrongBigShot.Cooldown, true, index, 0, ActiveAbilityDB.Instance.StrongBigShot.Damage, 2, 0, 0);
            case SkillType.SmallFireworkCircle:
                return new ActiveSkill("FireworkBlast", SmallFireworkCircle, ActiveAbilityDB.Instance.SmallFireworkCircle.Cooldown, false, index);
            case SkillType.LargeFireworkCircle:
                return new ActiveSkill("FireworkBlast", LargeFireworkCircle, ActiveAbilityDB.Instance.LargeFireworkCircle.Cooldown, false, index);
            case SkillType.PoisonCloud:
                return new PoisonCloudSkill("PoisonCloud", this, ActiveAbilityDB.Instance.PoisonCloud.Cooldown, true, index, ActiveAbilityDB.Instance.PoisonCloud.Level, ActiveAbilityDB.Instance.PoisonCloud.Duration);
            case SkillType.StrongPoisonCloud:
                return new PoisonCloudSkill("PoisonCloud", this, ActiveAbilityDB.Instance.StrongPoisonCloud.Cooldown, true, index, ActiveAbilityDB.Instance.StrongPoisonCloud.Level, ActiveAbilityDB.Instance.StrongPoisonCloud.Duration);
            case SkillType.StunShot:
                return new StunShotSkill("StunShot", this, ActiveAbilityDB.Instance.StunShot.Cooldown, true, index, ActiveAbilityDB.Instance.StunShot.Damage, ActiveAbilityDB.Instance.StunShot.StunDuration);
            case SkillType.StrongStunShot:
                return new StunShotSkill("StunShot", this, ActiveAbilityDB.Instance.StrongStunShot.Cooldown, true, index, ActiveAbilityDB.Instance.StrongStunShot.Damage, ActiveAbilityDB.Instance.StrongStunShot.StunDuration);
            case SkillType.WeakFlameThrower:
                return new FlamethrowerSkill("Flamethrower", this, ActiveAbilityDB.Instance.WeakFlamethrower.Cooldown, false, index, ActiveAbilityDB.Instance.WeakFlamethrower.Damage);
            case SkillType.MediumFlameThrower:
                return new FlamethrowerSkill("Flamethrower", this, ActiveAbilityDB.Instance.MediumFlamethrower.Cooldown, false, index, ActiveAbilityDB.Instance.MediumFlamethrower.Damage);
            case SkillType.StrongFlameThrower:
                return new FlamethrowerSkill("Flamethrower", this, ActiveAbilityDB.Instance.StrongFlamethrower.Cooldown, false, index, ActiveAbilityDB.Instance.StrongFlamethrower.Damage);
            case SkillType.WeakDefenseBoost:
                return new StatusSkill("DefenseBoost", this, ActiveAbilityDB.Instance.WeakDefenseBoost.Cooldown, false, index, StatusType.Armor, ActiveAbilityDB.Instance.WeakDefenseBoost.Duration, ActiveAbilityDB.Instance.WeakDefenseBoost.Level);
            case SkillType.VeryWeakBigShot:
                return new SpecialShotSkill("BigShot", this, ActiveAbilityDB.Instance.VeryWeakBigShot.Cooldown, true, index, 0, ActiveAbilityDB.Instance.VeryWeakBigShot.Damage, 1, 0, 0);
        }
        return null;
    }

    /// <summary>
    /// Clears all hitboxes off player
    /// </summary>
    public void ClearHitboxes()
    {
        for (int i = 0; i < _hitboxes.Count; i++)
        {
            GameObject.Destroy(_hitboxes[i].gameObject);
        }
        _hitboxes.Clear();
    }

    /// <summary>
    /// Creates a hitbox as a child of the enemy
    /// </summary>
    /// <param name="position">Position relative to enemy</param>
    /// <param name="scale">Size of hitbox</param>
    /// <param name="type">Type of hitbox</param>
    /// <param name="damage">Damage dealt by hitbox</param>
    /// <param name="launchAngle">Angle that hitbox will launch player</param>
    /// <param name="launchStrength">Strength at which player will be launched</param>
    /// <returns></returns>
    public Hitbox CreateHitbox(Vector3 position, Vector3 scale, HitboxType type, float damage, Vector2 launchAngle, float launchStrength)
    {
        Hitbox temp = Instantiate(_hitbox, transform).GetComponent<Hitbox>();
        temp.SetHitbox(gameObject, position, scale, type, damage, launchAngle, launchStrength);
        return temp;
    }

    #region Skills
    /// <summary>
    /// Small Dash ability, player dashes forward
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool SmallDash(Enemy enemy, ref float time)
    {
        //Applies force forward for player
        _movementScript.ApplyConstantMoveForce(transform.forward, ActiveAbilityDB.Instance.SmallDash.Distance, ActiveAbilityDB.Instance.SmallDash.Time);
        if (time > ActiveAbilityDB.Instance.SmallDash.Time)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Dash ability, player dashes forward
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool Dash(Enemy enemy, ref float time)
    {
        //Applies force in direction of player velocity

        _movementScript.ApplyConstantMoveForce(transform.forward, ActiveAbilityDB.Instance.Dash.Distance, ActiveAbilityDB.Instance.Dash.Time);
        if(time > ActiveAbilityDB.Instance.Dash.Time)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Stops player's movement
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool StopMovement(Enemy enemy)
    {
        _inputManager.ResetMovement();
        return true;
    }


    /// <summary>
    /// Boosts player defense for 3 seconds
    /// Reduce damage taken by 20% for 3 seconds
    /// Reduce Knockback by 33% for 3 seconds
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool SmallDefenseBoost(Enemy enemy)
    {
        //Applies speed effect to player
        _playerStatusEffects.AddStatus(StatusType.Armor, ActiveAbilityDB.Instance.SmallDefenseBoost.Duration, ActiveAbilityDB.Instance.SmallDefenseBoost.ArmorLevel);
        _playerStatusEffects.AddStatus(StatusType.Hardiness, ActiveAbilityDB.Instance.SmallDefenseBoost.Duration, ActiveAbilityDB.Instance.SmallDefenseBoost.HardinessLevel);
        return true;
    }

    /// <summary>
    /// Boosts player defense for 3 seconds
    /// Reduce damage taken by 33% for 3 seconds
    /// Reduce Knockback by 50% for 3 seconds
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool LargeDefenseBoost(Enemy enemy)
    {
        //Applies speed effect to player
        _playerStatusEffects.AddStatus(StatusType.Armor, ActiveAbilityDB.Instance.LargeDefenseBoost.Duration, ActiveAbilityDB.Instance.LargeDefenseBoost.ArmorLevel);
        _playerStatusEffects.AddStatus(StatusType.Hardiness, ActiveAbilityDB.Instance.LargeDefenseBoost.Duration, ActiveAbilityDB.Instance.LargeDefenseBoost.HardinessLevel);
        return true;
    }

    /// <summary>
    /// Fire a 360ish spread shot
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool SmallFireworkCircle(Enemy enemy)
    {
        Dictionary<StatusType, float> shotValues = new Dictionary<StatusType, float>()
        {
            {StatusType.Count, 12 },
            {StatusType.Damage, ActiveAbilityDB.Instance.SmallFireworkCircle.Damage },
            {StatusType.ShotSize, 0},
            {StatusType.SpreadAngle, 130},
            {StatusType.VerticalRatio, 0 }
        };
        MasterUpgrade upgrade = new MasterUpgrade("firework_shot", shotValues);

        _cannonFireScript.Fire(transform.right, transform.right, 0, upgrade);
        _cannonFireScript.Fire(transform.right, -transform.right, 0, upgrade);
        return true;
    }

    /// <summary>
    /// Fire a 360ish spread shot
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool LargeFireworkCircle(Enemy enemy)
    {
        Dictionary<StatusType, float> shotValues = new Dictionary<StatusType, float>()
        {
            {StatusType.Count, 12 },
            {StatusType.Damage, ActiveAbilityDB.Instance.LargeFireworkCircle.Damage },
            {StatusType.ShotSize, 0},
            {StatusType.SpreadAngle, 130},
            {StatusType.VerticalRatio, 0 }
        };
        MasterUpgrade upgrade = new MasterUpgrade("firework_shot", shotValues);

        _cannonFireScript.Fire(transform.right, transform.right, 0, upgrade);
        _cannonFireScript.Fire(transform.right, -transform.right, 0, upgrade);
        return true;
    }

    public void SpecialShot(Enemy enemy, int count, float damage, float shotSize, float spreadAngle, float verticleRatio)
    {
        Dictionary<StatusType, float> shotValues = new Dictionary<StatusType, float>()
        {
            {StatusType.Count, count },
            {StatusType.Damage, damage },
            {StatusType.ShotSize, shotSize},
            {StatusType.SpreadAngle, spreadAngle},
            {StatusType.VerticalRatio, verticleRatio }
        };
        MasterUpgrade upgrade = new MasterUpgrade("special_shot", shotValues);

        Vector3 diff = (enemy.transform.position - transform.position).normalized;

        //Calculate offset for firing
        float offset = 0;
        if (_rightEnemy)
        {
            offset = 15.0f;
        }
        else
        {
            offset = -15.0f;
        }

        _cannonFireScript.Fire(transform.right, new Vector3(diff.x, 0, diff.z), offset, upgrade);
    }

    #endregion
}

public class ActiveSkill
{
    protected Skill _skill;
    protected OvertimeSkill _timeSkill;
    protected float _maxCooldown;
    protected float _currCooldown;
    protected bool _needsEnemy;
    protected string _name;
    protected int _index;

    public float MaxCooldown => _maxCooldown;
    public float CurrCooldown => _currCooldown;
    public bool NeedsEnemy => _needsEnemy;
    public bool Active => _active;
    public string Name => _name;

    public event CallbackIndex OnCooldownEnd;

    //Over time active skill variables
    protected bool _instant;
    protected bool _active;
    protected float _currActiveTime;
    protected Enemy _targetEnemy;

    public ActiveSkill(string name, Skill skill, float cooldown, bool needsEnemy, int index)
    {
        _skill = skill;
        _maxCooldown = cooldown;
        _currCooldown = 0;
        _needsEnemy = needsEnemy;
        _instant = true;
        _name = name;
        _index = index;
    }

    public ActiveSkill(string name, OvertimeSkill skill, float cooldown, bool needsEnemy, int index)
    {
        _timeSkill = skill;
        _maxCooldown = cooldown;
        _currCooldown = 0;
        _needsEnemy = needsEnemy;
        _instant = false;
        _currActiveTime = 0;
        _name = name;
        _index = index;
    }

    /// <summary>
    /// Returns if the skill is in cooldown
    /// </summary>
    public bool InCooldown
    {
        get { return _currCooldown > 0; }
    }

    /// <summary>
    /// Decreases the skills cooldown
    /// </summary>
    /// <param name="time">Time to decrement skill</param>
    public void DoCooldown(float time)
    {
        if (_currCooldown > 0)
        {
            _currCooldown -= time;
            if (_currCooldown <= 0)
            {
                OnCooldownEnd?.Invoke(_index);
                _currCooldown = 0;
            }
        }
    }

    /// <summary>
    /// Activates active skill
    /// </summary>
    /// <param name="enemy">Enemy player is targeting</param>
    public void Activate(Enemy enemy)
    {
        //If skill is not in cooldown, activate skill
        if (!InCooldown)
        {
            //If the skill is instant
            if (_instant)
            {
                //Only set cooldown if skill is successful
                if (_skill(enemy))
                {
                    //Set cooldown
                    _currCooldown = _maxCooldown;
                }
            }
            //If skill is overtime
            else
            {
                if (enemy != null)
                {
                    _active = true;
                    _targetEnemy = enemy;
                    _currActiveTime = 0;
                    _currCooldown = _maxCooldown;
                }
            }
        }
    }

    /// <summary>
    /// Activates active skill
    /// </summary>
    public void Activate()
    {
        //If skill is not in cooldown, activate skill
        if (!InCooldown)
        {
            //If skill is instant
            if (_instant)
            {
                if (_skill(null))
                {
                    //Set cooldown
                    _currCooldown = _maxCooldown;
                }
            }
            //If skill is overtime
            else
            {
                _active = true;
                _targetEnemy = null;
                _currActiveTime = 0;
                _currCooldown = _maxCooldown;
            }
        }
    }

    /// <summary>
    /// Executes an overtime active skill
    /// </summary>
    public void DoOvertimeActive()
    {
        //Do active skill until time is up
        if (_timeSkill(_targetEnemy, ref _currActiveTime))
        {
            //Turn off overtime active skill
            _active = false;
            _currActiveTime = 0;
        }
        else
        {
            _currActiveTime += Time.deltaTime;
        }
    }
}

public class RapidShotSkill : ActiveSkill
{
    private ShipMovement _movementScript;
    private ActiveAbilities _activeAbilities;
    private int _shotCount;
    private int _maxShots;
    private float _shotDelay;
    private float _damage;

    public RapidShotSkill(string name, ActiveAbilities activeAbilities, float cooldown, bool needsEnemy, int index, int maxShots, float shotDelay, float damage) : base(name, (OvertimeSkill)null, cooldown, needsEnemy, index)
    {
        _activeAbilities = activeAbilities;
        _movementScript = activeAbilities.GetComponent<ShipMovement>();
        _maxShots = maxShots;
        _shotDelay = shotDelay;
        _timeSkill = RapidShot;
        _damage = damage;
    }

    /// <summary>
    /// Shoots three shots in a row towards the closest enemy
    /// </summary>
    /// <param name="enemy">Enemy to fire at</param>
    /// <param name="time">Current time</param>
    /// <returns>If skill was successful</returns>
    private bool RapidShot(Enemy enemy, ref float time)
    {
        //Enemy is needed for skill to activate
        if (enemy == null)
        {
            return true;
        }

        if (time > _shotDelay)
        {
            time = 0;
        }

        if (time == 0)
        {
            _activeAbilities.SpecialShot(enemy, 0, _damage, 0.2f, 0, 0);
            _shotCount += 1;
        }

        if (_shotCount == _maxShots)
        {
            _shotCount = 0;
            return true;
        }

        return false;
    }
}

public class RamSkill : ActiveSkill
{
    private ShipMovement _movementScript;
    private ActiveAbilities _activeAbilities;
    private Hitbox _ramHitbox;
    private float _ramDamage;
    private float _ramDistance;

    public RamSkill(string name, ActiveAbilities activeAbilities, float cooldown, bool needsEnemy, int index, float ramDamage, float ramDistance) : base(name, (OvertimeSkill)null, cooldown, needsEnemy, index)
    {
        _activeAbilities = activeAbilities;
        _movementScript = activeAbilities.GetComponent<ShipMovement>();
        _ramDamage = ramDamage;
        _ramDistance = ramDistance;
        _timeSkill = RamAttack;
    }

    /// <summary>
    /// Player rams forward to attack enemy
    /// </summary>
    /// <param name="enemy">Enemy, not needed for this attack</param>
    /// <param name="currTime">Current time</param>
    /// <param name="fullTime">Full time</param>
    /// <returns>If attack is finished</returns>
    public bool RamAttack(Enemy enemy, ref float time)
    {
        if (time == 0)
        {
            //Create Hitbox for raming
            _ramHitbox = _activeAbilities.CreateHitbox(new Vector3(0, 0, 2), new Vector3(1, 1, 1), HitboxType.PlayerHitbox, _ramDamage, Vector2.zero, 0);
            _ramHitbox.OnTrigger += RamHit;
        }

        if (_ramHitbox != null)
        {
            _movementScript.ApplyConstantMoveForce(_movementScript.transform.forward, _ramDistance, 1.0f);
        }

        if (time > 1.0f)
        {
            if (_ramHitbox != null)
            {
                GameObject.Destroy(_ramHitbox.gameObject);
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// Called when player hits an object while ramming
    /// </summary>
    /// <param name="other"></param>
    private void RamHit(GameObject other)
    {
        if (other.tag == "Enemy" || other.tag == "Obstical")
        {
            _movementScript.StopMotion();
            _movementScript.TakeKnockback(_movementScript.transform.forward * -100f);
            GameObject.Destroy(_ramHitbox.gameObject);
        }
    }
}

public class BubbleFieldSkill : ActiveSkill
{
    private ShipMovement _movementScript;
    private ActiveAbilities _activeAbilities;
    private GameObject _bubbleBroth;
    private float _timeActive;
    private float _debuffDuration;

    public BubbleFieldSkill(string name, ActiveAbilities activeAbilities, float cooldown, bool needsEnemy, int index, float timeActive, float debuffDur) : base(name, (Skill)null, cooldown, needsEnemy, index)
    {
        _activeAbilities = activeAbilities;
        _movementScript = activeAbilities.GetComponent<ShipMovement>();
        _timeActive = timeActive;
        _debuffDuration = debuffDur;
        _bubbleBroth = (GameObject)Resources.Load("ActiveAbilities/PlayerBubbleBroth");
        _skill = BubbleField;
    }

    /// <summary>
    /// Player rams forward to attack enemy
    /// </summary>
    /// <param name="enemy">Enemy, not needed for this attack</param>
    /// <returns>If attack is finished</returns>
    public bool BubbleField(Enemy enemy)
    {
        GameObject bubbles = GameObject.Instantiate(_bubbleBroth, _movementScript.Position, _movementScript.Rotation);
        bubbles.GetComponent<PlayerBubbeBroth>().SetBubbleBroth(_timeActive, _debuffDuration);
        return true;
    }
}

public class SteelMineSkill : ActiveSkill
{
    private ShipMovement _movementScript;
    private ActiveAbilities _activeAbilities;
    private GameObject _steelMine;
    private int _mineCount;
    private float _mineDamage;
    private const float MAX_MINE_DIST = 10.0f;
    private GameObject _prevMine;

    public SteelMineSkill(string name, ActiveAbilities activeAbilities, float cooldown, bool needsEnemy, int index, float mineDamage) : base(name, (OvertimeSkill)null, cooldown, needsEnemy, index)
    {
        _activeAbilities = activeAbilities;
        _movementScript = activeAbilities.GetComponent<ShipMovement>();
        _mineDamage = mineDamage;
        _mineCount = 0;
        _steelMine = (GameObject)Resources.Load("ActiveAbilities/SteelMine");
        _timeSkill = SteelMine;
    }

    /// <summary>
    /// Place three mines behind the player overtime
    /// </summary>
    /// <param name="enemy">Enemy, not needed for this attack</param>
    /// <returns>If attack is finished</returns>
    public bool SteelMine(Enemy enemy, ref float time)
    {
        if ((time > 0.5f && _prevMine == null) || (time > 0.5f && Vector3.SqrMagnitude(_prevMine.transform.position - _movementScript.transform.position) > MAX_MINE_DIST * MAX_MINE_DIST))
        {
            time = 0;
        }

        if (time == 0)
        {
            GameObject mine = GameObject.Instantiate(_steelMine, _movementScript.Position, _movementScript.Rotation);
            mine.GetComponent<SteelMine>().SetMine(_mineDamage);
            _prevMine = mine;
            _mineCount += 1;
        }

        if(_mineCount == 3)
        {
            _mineCount = 0;
            return true;
        }

        return false;
    }
}

public class CounterSkill : ActiveSkill
{
    private ShipMovement _movementScript;
    private ActiveAbilities _activeAbilities;
    private float _counterDamage;
    private Hitbox _counterHitbox;
    private const float COUNTER_TIME = 1.0f;

    public CounterSkill(string name, ActiveAbilities activeAbilities, float cooldown, bool needsEnemy, int index, float counterDamage) : base(name, (OvertimeSkill)null, cooldown, needsEnemy, index)
    {
        _activeAbilities = activeAbilities;
        _movementScript = activeAbilities.GetComponent<ShipMovement>();
        _counterDamage = counterDamage;
        _timeSkill = Counter;
    }

    /// <summary>
    /// Counter enemy attacks taken in a short time period
    /// </summary>
    /// <param name="enemy">Enemy counter will be </param>
    /// <returns>If attack is finished</returns>
    public bool Counter(Enemy enemy, ref float time)
    {
        if(time == 0)
        {
            _counterHitbox = _activeAbilities.CreateHitbox(new Vector3(0,0.5f,0), new Vector3(3, 2, 7), HitboxType.PlayerHurtbox, 0, Vector2.zero, 0);
            _counterHitbox.OnTrigger += DealCounter;
            _activeAbilities.PlayerStatusEffects.AddStatus(StatusType.Armor, "CounterArmor", COUNTER_TIME, 999999.0f);
        }

        if(time > COUNTER_TIME)
        {
            if (_counterHitbox != null)
            {
                GameObject.Destroy(_counterHitbox.gameObject);
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// Deals counter to owner of hitbox
    /// </summary>
    /// <param name="other">Hitbox that triggered</param>
    public void DealCounter(GameObject other)
    {
        if(other.tag == "Enemy")
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if(enemy != null)
            {
                if(Vector3.Dot(_movementScript.transform.forward, enemy.transform.forward) > 0)
                {
                    _activeAbilities.RightEnemy = true;
                }
                else
                {
                    _activeAbilities.RightEnemy = false;
                }

                _activeAbilities.SpecialShot(enemy, 0, _counterDamage, 1.5f, 0, 0);
                _activeAbilities.PlayerStatusEffects.RemoveStatus("CounterArmor");
                _activeAbilities.PlayerStatusEffects.AddStatus(StatusType.Armor, 0.1f, 999999.0f);
                GameObject.Destroy(_counterHitbox.gameObject);
            }
        }
    }
}

public class StatusSkill : ActiveSkill
{
    private ShipMovement _movementScript;
    private ActiveAbilities _activeAbilities;
    private StatusType _type;
    private float _duration;
    private float _level;

    public StatusSkill(string name, ActiveAbilities activeAbilities, float cooldown, bool needsEnemy, int index, StatusType type, float duration, float level) : base(name, (Skill)null, cooldown, needsEnemy, index)
    {
        _activeAbilities = activeAbilities;
        _movementScript = activeAbilities.GetComponent<ShipMovement>();
        _type = type;
        _duration = duration;
        _level = level;
        _skill = AddStatus;
    }

    /// <summary>
    /// Inflicts player with specified status effect
    /// </summary>
    /// <param name="enemy">Enemy, not needed for this skill</param>
    /// <returns>If attack is finished</returns>
    public bool AddStatus(Enemy enemy)
    {
        _activeAbilities.PlayerStatusEffects.AddStatus(_type, _duration, _level);

        return true;
    }
}

public class SpecialShotSkill : ActiveSkill
{
    private ShipMovement _movementScript;
    private ActiveAbilities _activeAbilities;
    private int _count;
    private float _damage;
    private float _size;
    private float _spreadAngle;
    private float _verticleRatio;

    public SpecialShotSkill(string name, ActiveAbilities activeAbilities, float cooldown, bool needsEnemy, int index, int count, float damage, float size, float spreadAngle, float verticleRatio) : base(name, (Skill)null, cooldown, needsEnemy, index)
    {
        _activeAbilities = activeAbilities;
        _movementScript = activeAbilities.GetComponent<ShipMovement>();
        _count = count;
        _damage = damage;
        _size = size;
        _spreadAngle = spreadAngle;
        _verticleRatio = verticleRatio;
        _skill = SpecialShot;
    }

    /// <summary>
    /// Inflicts player with specified status effect
    /// </summary>
    /// <param name="enemy">Enemy, not needed for this skill</param>
    /// <returns>If attack is finished</returns>
    public bool SpecialShot(Enemy enemy)
    {
        //Make sure enemy is not null
        if(enemy == null)
        {
            return false;
        }

        _activeAbilities.SpecialShot(enemy, _count, _damage, _size, _spreadAngle, _verticleRatio);

        return true;
    }
}

public class PoisonCloudSkill : ActiveSkill
{
    private ShipMovement _movementScript;
    private ActiveAbilities _activeAbilities;
    private GameObject _poisonCloudPrefab;
    private GameObject _poisonCloud;
    private Vector3 _movementDirection;
    private float _damagePerSecond;
    private float _poisonDuration;

    public PoisonCloudSkill(string name, ActiveAbilities activeAbilities, float cooldown, bool needsEnemy, int index, float damagePerSecond, float poisonDuration) : base(name, (OvertimeSkill)null, cooldown, needsEnemy, index)
    {
        _activeAbilities = activeAbilities;
        _movementScript = activeAbilities.GetComponent<ShipMovement>();
        _poisonCloudPrefab = (GameObject)Resources.Load("ActiveAbilities/PoisonCloud");
        _damagePerSecond = damagePerSecond;
        _poisonDuration = poisonDuration;
        _timeSkill = PoisonCloud;
    }

    /// <summary>
    /// Inflicts player with specified status effect
    /// </summary>
    /// <param name="enemy">Enemy, not needed for this skill</param>
    /// <returns>If attack is finished</returns>
    public bool PoisonCloud(Enemy enemy, ref float time)
    {
        //Make sure enemy is not null
        if (time == 0)
        {
            _poisonCloud = GameObject.Instantiate(_poisonCloudPrefab, _movementScript.transform.position + _movementScript.transform.right, _movementScript.Rotation);
            Hitbox hitbox = GameObject.Instantiate(_activeAbilities.HitboxPrefab, _poisonCloud.transform.position, _poisonCloud.transform.rotation, _poisonCloud.transform).GetComponent<Hitbox>();
            hitbox.SetHitbox(_movementScript.gameObject, Vector3.zero, new Vector3(1, 1, 1), HitboxType.PlayerHitbox, 0);
            hitbox.OnTrigger += ApplyPoison;
            _movementDirection = enemy.transform.position - _movementScript.transform.position;
            _movementDirection = new Vector3(_movementDirection.x, 0, _movementDirection.z);
            _movementDirection.Normalize();
        }

        _poisonCloud.transform.position += _movementDirection * 25 * Time.deltaTime;

        if(time > 1.0f)
        {
            _poisonCloud.GetComponentInChildren<ParticleSystem>().Stop();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Apply poison effect to enemy
    /// </summary>
    /// <param name="other"></param>
    private void ApplyPoison(GameObject other)
    {
        if (other.tag == "Enemy")
        {
            StatusEffects status = other.GetComponent<StatusEffects>();

            if (status != null && status.CheckStatus("PoisonCloud"))
            {
                status.AddStatus(StatusType.Poison, "PoisonCloud", _poisonDuration, _damagePerSecond);
            }
        }
    }
}

public class StunShotSkill : ActiveSkill
{
    private ShipMovement _movementScript;
    private ActiveAbilities _activeAbilities;
    private float _damage;
    private float _stunTime;

    public StunShotSkill(string name, ActiveAbilities activeAbilities, float cooldown, bool needsEnemy, int index, float damage, float stunTime) : base(name, (Skill)null, cooldown, needsEnemy, index)
    {
        _activeAbilities = activeAbilities;
        _movementScript = activeAbilities.GetComponent<ShipMovement>();
        _damage = damage;
        _stunTime = stunTime;
        _skill = StunShot;
    }

    /// <summary>
    /// Inflicts player with specified status effect
    /// </summary>
    /// <param name="enemy">Enemy, not needed for this skill</param>
    /// <returns>If attack is finished</returns>
    public bool StunShot(Enemy enemy)
    {
        if(enemy == null)
        {
            return false;
        }

        Dictionary<StatusType, float> shotValues = new Dictionary<StatusType, float>()
        {
            {StatusType.Count, 0 },
            {StatusType.Damage, _damage },
            {StatusType.ShotSize, 2.0f},
            {StatusType.SpreadAngle, 0},
            {StatusType.VerticalRatio, 0 }
        };
        MasterUpgrade upgrade = new MasterUpgrade("special_shot", shotValues);

        Vector3 diff = (enemy.transform.position - _movementScript.transform.position).normalized;

        //Calculate offset for firing
        float offset = 0;
        if (_activeAbilities.RightEnemy)
        {
            offset = 15.0f;
        }
        else
        {
            offset = -15.0f;
        }

        _activeAbilities.CannonFireScript.Fire(_movementScript.transform.right, new Vector3(diff.x, 0, diff.z), offset, upgrade, StunEnemy);

        return true;
    }

    /// <summary>
    /// Stun enemy
    /// </summary>
    /// <param name="other"></param>
    private void StunEnemy(GameObject other)
    {
        if (other.tag == "Enemy")
        {
            StatusEffects status = other.GetComponent<StatusEffects>();
            if (status != null)
            {
                other.GetComponent<StatusEffects>().AddStatus(StatusType.Stun, _stunTime, 1.0f);
            }
        }
    }
}

public class FlamethrowerSkill : ActiveSkill
{
    private ShipMovement _movementScript;
    private ActiveAbilities _activeAbilities;
    private GameObject _flamethrowerPrefab;
    private GameObject _rightFlame;
    private GameObject _leftFlame;
    private Hitbox _leftHitbox;
    private Hitbox _rightHitbox;
    private List<StatusEffects> _enemyStatus;
    private float _damagePerSecond;
    private float _poisonDuration;

    public FlamethrowerSkill(string name, ActiveAbilities activeAbilities, float cooldown, bool needsEnemy, int index, float damagePerSecond) : base(name, (OvertimeSkill)null, cooldown, needsEnemy, index)
    {
        _activeAbilities = activeAbilities;
        _movementScript = activeAbilities.GetComponent<ShipMovement>();
        _flamethrowerPrefab = (GameObject)Resources.Load("ActiveAbilities/Flamethrower");
        _damagePerSecond = damagePerSecond;
        _enemyStatus = new List<StatusEffects>();
        _timeSkill = Flamethrower;
    }

    /// <summary>
    /// Inflicts player with specified status effect
    /// </summary>
    /// <param name="enemy">Enemy, not needed for this skill</param>
    /// <returns>If attack is finished</returns>
    public bool Flamethrower(Enemy enemy, ref float time)
    {
        //Make sure enemy is not null
        if (time == 0)
        {
            _rightFlame = GameObject.Instantiate(_flamethrowerPrefab, _movementScript.transform.position + _movementScript.transform.right, Quaternion.LookRotation(_movementScript.transform.right), _movementScript.transform);
            _leftFlame = GameObject.Instantiate(_flamethrowerPrefab, _movementScript.transform.position - _movementScript.transform.right, Quaternion.LookRotation(-_movementScript.transform.right), _movementScript.transform);

            //Create right hitbox
            _rightHitbox = GameObject.Instantiate(_activeAbilities.HitboxPrefab, _rightFlame.transform.position, _rightFlame.transform.rotation, _rightFlame.transform).GetComponent<Hitbox>();
            _rightHitbox.SetHitbox(_movementScript.gameObject, new Vector3(0,0,10), new Vector3(3, 1, 20), HitboxType.PlayerHitbox, 0);
            _rightHitbox.OnTrigger += ApplyFire;
            _rightHitbox.OnExit += RemoveFireExit;
            _rightHitbox.OnDestruction += RemoveFire;

            //Create left hitbox
            _leftHitbox = GameObject.Instantiate(_activeAbilities.HitboxPrefab, _leftFlame.transform.position, _leftFlame.transform.rotation, _leftFlame.transform).GetComponent<Hitbox>();
            _leftHitbox.SetHitbox(_movementScript.gameObject, new Vector3(0, 0, 10), new Vector3(3, 1, 20), HitboxType.PlayerHitbox, 0);
            _leftHitbox.OnTrigger += ApplyFire;
            _leftHitbox.OnExit += RemoveFireExit;
            _leftHitbox.OnDestruction += RemoveFire;
        }

        if (time > 5.0f)
        {
            _rightFlame.GetComponentInChildren<ParticleSystem>().Stop();
            _leftFlame.GetComponentInChildren<ParticleSystem>().Stop();
            GameObject.Destroy(_leftHitbox.gameObject);
            GameObject.Destroy(_rightHitbox.gameObject);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Apply fire effect to enemy
    /// </summary>
    /// <param name="other"></param>
    private void ApplyFire(GameObject other)
    {
        if (other.tag == "Enemy")
        {
            StatusEffects status = other.GetComponent<StatusEffects>();
            if (status != null && !status.CheckStatus("FlamethrowerFire"))
            {
                status.AddStatus(StatusType.Fire, "FlamethrowerFire", 5.0f, _damagePerSecond);
                _enemyStatus.Add(status);
            }
        }
    }

    /// <summary>
    /// Removes fire effect when enemy leaves
    /// </summary>
    /// <param name="other"></param>
    private void RemoveFireExit(GameObject other)
    {
        if (other.tag == "Hitbox" && other.transform.parent.tag == "Enemy")
        {
            StatusEffects status = other.GetComponent<Hitbox>().AttachedObject.GetComponent<StatusEffects>();
            if (status != null)
            {
                other.GetComponent<Hitbox>().AttachedObject.GetComponent<StatusEffects>().RemoveStatus("FlamethrowerFire");
                _enemyStatus.Remove(status);
            }
        }
    }

    /// <summary>
    /// Removes fire effect when fire ends
    /// </summary>
    private void RemoveFire()
    {
        if (_enemyStatus.Count > 0)
        {
            for (int i = 0; i < _enemyStatus.Count; i++)
            {
                if (_enemyStatus[i] != null)
                {
                    _enemyStatus[i].RemoveStatus("FlamethrowerFire");
                }
            }
        }
    }
}
