using System.Collections;
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
    WeakFlameThrower = 44, MediumFlameThrower = 45, StrongFlameThrower = 46
}

public class ActiveAbilities : MonoBehaviour
{
    private const int SKILL_AMOUNT = 3;

    private ShipMovement _movementScript;
    private CannonFire _cannonFireScript;
    public CannonFire CannonFireScript => _cannonFireScript;
    [SerializeField]
    private InputManager _inputManager;

    [SerializeField]
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
        _skill[i] = GetActiveSkill(_skillType[i], i);
        if (_skill[i] != null)
        {
            _buttons[i].GetComponentInChildren<TMP_Text>().text = _skill[i].Name;
            _skill[i].OnCooldownEnd += ResetButton;
        }
        else
        {
            _buttons[i].GetComponentInChildren<TMP_Text>().text = "None";
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
                return new ActiveSkill("Small Dash", SmallDash, 5.0f, false, index);
            case SkillType.Dash:
                return new ActiveSkill("Dash", Dash, 5.0f, false, index);
            case SkillType.SmallManuverabilityBoost:
                return new StatusSkill("Manuverability Boost", this, 20.0f, false, index, StatusType.Maneuverability, 5.0f, 2.0f);
            case SkillType.MediumManuverabilityBoost:
                return new StatusSkill("Manuverability Boost", this, 20.0f, false, index, StatusType.Maneuverability, 6.0f, 2.0f);
            case SkillType.LargeManuverabilityBoost:
                return new StatusSkill("Manuverability Boost", this, 20.0f, false, index, StatusType.Maneuverability, 7.0f, 2.0f);
            case SkillType.BubbleField:
                return new BubbleFieldSkill("Bubble Field", this, 25.0f, false, index, 5.0f, 2.0f);
            case SkillType.StrongBubbleField:
                return new BubbleFieldSkill("Bubble Field", this, 25.0f, false, index, 10.0f, 3.0f);
            case SkillType.StopMovement:
                return new ActiveSkill("Stop Movement", StopMovement, 10.0f, false, index);
            case SkillType.StrongStopMovement:
                return new ActiveSkill("Stop Movement", StopMovement, 5.0f, false, index);
            case SkillType.SmallSpeedBoost:
                return new StatusSkill("Speed Boost", this, 15.0f, false, index, StatusType.MovementSpeed, 2.0f, 1.5f);
            case SkillType.MediumSpeedBoost:
                return new StatusSkill("Speed Boost", this, 15.0f, false, index, StatusType.MovementSpeed, 2.5f, 1.5f);
            case SkillType.LargeSpeedBoost:
                return new StatusSkill("Speed Boost", this, 15.0f, false, index, StatusType.MovementSpeed, 3.0f, 1.75f);
            case SkillType.SmallRegeneration:
                return new StatusSkill("Regeneration", this, 25.0f, false, index, StatusType.Regeneration, 3.0f, 10.0f / 3.0f);
            case SkillType.MediumRegeneration:
                return new StatusSkill("Regeneration", this, 25.0f, false, index, StatusType.Regeneration, 3.0f, 15.0f / 3.0f);
            case SkillType.LargeRegeneration:
                return new StatusSkill("Regeneration", this, 25.0f, false, index, StatusType.Regeneration, 3.0f, 20.0f / 3.0f);
            case SkillType.SmallDefenseBoost:
                return new ActiveSkill("Defense Boost", SmallDefenseBoost, 20.0f, false, index);
            case SkillType.LargeDefenseBoost:
                return new ActiveSkill("Defense Boost", LargeDefenseBoost, 20.0f, false, index);
            case SkillType.WeakSteelMine:
                return new SteelMineSkill("Mine", this, 25.0f, false, index, 10.0f);
            case SkillType.MediumSteelMine:
                return new SteelMineSkill("Mine", this, 25.0f, false, index, 15.0f);
            case SkillType.StrongSteelMine:
                return new SteelMineSkill("Mine", this, 25.0f, false, index, 20.0f);
            case SkillType.SmallCounter:
                return new CounterSkill("Counter", this, 15.0f, false, index, 5f);
            case SkillType.MediumCounter:
                return new CounterSkill("Counter", this, 15.0f, false, index, 8f);
            case SkillType.LargeCounter:
                return new CounterSkill("Counter", this, 15.0f, false, index, 10.0f);
            case SkillType.SmallRam:
                return new RamSkill("Ram", this, 20.0f, false, index, 20.0f, 20.0f);
            case SkillType.MediumRam:
                return new RamSkill("Ram", this, 20.0f, false, index, 25.0f, 20.0f);
            case SkillType.LargeRam:
                return new RamSkill("Ram", this, 20.0f, false, index, 30.0f, 25.0f);
            case SkillType.SmallSeaglassSpeed:
                return new StatusSkill("Speed Boost", this, 15.0f, false, index, StatusType.MovementSpeed, 3.0f, 2.0f);
            case SkillType.MediumSeaglassSpeed:
                return new StatusSkill("Speed Boost", this, 15.0f, false, index, StatusType.MovementSpeed, 4.0f, 2.0f);
            case SkillType.LargeSeaglassSpeed:
                return new StatusSkill("Speed Boost", this, 15.0f, false, index, StatusType.MovementSpeed, 5.0f, 2.0f);
            case SkillType.SmallInvulnerability:
                return new StatusSkill("Invulnerability", this, 20.0f, false, index, StatusType.Armor, 1.0f, 999999.0f);
            case SkillType.MediumInvulnerability:
                return new StatusSkill("Invulnerability", this, 20.0f, false, index, StatusType.Armor, 1.5f, 999999.0f);
            case SkillType.LargeInvulnerability:
                return new StatusSkill("Invulnerability", this, 20.0f, false, index, StatusType.Armor, 2.0f, 999999.0f);
            case SkillType.SpreadShot:
                return new SpecialShotSkill("Spread Shot", this, 15.0f, true, index, 4, 2, 0.2f, 20, 0);
            case SkillType.RapidShotFour:
                return new RapidShotSkill("Rapid Shot", this, 20.0f, true, index, 4, 0.3f);
            case SkillType.RapidShotEight:
                return new RapidShotSkill("Rapid Shot", this, 20.0f, true, index, 8, 0.15f);
            case SkillType.WeakBigShot:
                return new SpecialShotSkill("Big Shot", this, 15.0f, true, index, 0, 5.0f, 3, 0, 0);
            case SkillType.MediumBigShot:
                return new SpecialShotSkill("Big Shot", this, 15.0f, true, index, 0, 8.0f, 3, 0, 0);
            case SkillType.StrongBigShot:
                return new SpecialShotSkill("Big Shot", this, 15.0f, true, index, 0, 10.0f, 3, 0, 0);
            case SkillType.SmallFireworkCircle:
                return new ActiveSkill("Firework Burst", SmallFireworkCircle, 20.0f, false, index);
            case SkillType.LargeFireworkCircle:
                return new ActiveSkill("Firework Burst", LargeFireworkCircle, 20.0f, false, index);
            case SkillType.PoisonCloud:
                return new PoisonCloudSkill("Poison Cloud", this, 20.0f, true, index, 10.0f, 3.0f);
            case SkillType.StrongPoisonCloud:
                return new PoisonCloudSkill("Poison Cloud", this, 20.0f, true, index, 10.0f, 5.0f);
            case SkillType.StunShot:
                return new StunShotSkill("Stun Shot", this, 10.0f, true, index, 10, 4.0f);
            case SkillType.StrongStunShot:
                return new StunShotSkill("Stun Shot", this, 10.0f, true, index, 14, 6.0f);
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
        _movementScript.ApplyConstantMoveForce(transform.forward, 8.0f, 0.5f);
        if (time > 0.5f)
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

        _movementScript.ApplyConstantMoveForce(transform.forward, 10.0f, 0.5f);
        if(time > 0.5f)
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
        _playerStatusEffects.AddStatus(StatusType.Armor, 3.0f, 25.0f);
        _playerStatusEffects.AddStatus(StatusType.Hardiness, 3.0f, 0.5f);
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
        _playerStatusEffects.AddStatus(StatusType.Armor, 3.0f, 50.0f);
        _playerStatusEffects.AddStatus(StatusType.Hardiness, 3.0f, 1.0f);
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
            {StatusType.Damage, 8 },
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
            {StatusType.Damage, 12 },
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

    public RapidShotSkill(string name, ActiveAbilities activeAbilities, float cooldown, bool needsEnemy, int index, int maxShots, float shotDelay) : base(name, (OvertimeSkill)null, cooldown, needsEnemy, index)
    {
        _activeAbilities = activeAbilities;
        _movementScript = activeAbilities.GetComponent<ShipMovement>();
        _maxShots = maxShots;
        _shotDelay = shotDelay;
        _timeSkill = RapidShot;
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
            _activeAbilities.SpecialShot(enemy, 0, 4, 0.2f, 0, 0);
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

    private void ApplyPoison(GameObject other)
    {
        if(other.tag == "Enemy")
        {
            //APPLY POISON EFFECT TO ENEMY
            Debug.Log("Enemy would be poisoned");
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

    private void StunEnemy(GameObject other)
    {
        if (other.tag == "Enemy")
        {
            //APPLY STUN EFFECT TO ENEMY
            Debug.Log("Enemy would be stunned");
        }
    }
}
