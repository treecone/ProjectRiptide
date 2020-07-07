using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public delegate bool Skill(Enemy enemy);
public delegate bool OvertimeSkill(Enemy enemy, ref float time);
public delegate void CallbackIndex(int i);
public enum SkillType { Dash = 0, BurstFire = 1, SpeedBoost = 2, TripleShot = 3, RamAttack = 4 }

public class ActiveAbilities : MonoBehaviour
{
    private const int SKILL_AMOUNT = 3;

    private ShipMovement _movementScript;
    private CannonFire _cannonFireScript;
    [SerializeField]
    private InputManager _inputManager;

    [SerializeField]
    private SkillType[] _skillType = new SkillType[SKILL_AMOUNT];

    [SerializeField]
    private Button[] _buttons = new Button[SKILL_AMOUNT];
    private Slider[] _sliders = new Slider[SKILL_AMOUNT];

    private ActiveSkill[] _skill = new ActiveSkill[SKILL_AMOUNT];

    private bool _rightEnemy;

    [SerializeField]
    private GameObject _hitbox;
    private static List<Hitbox> _hitboxes;

    private StatusEffects _playerStatusEffects;

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
            case SkillType.Dash:
                return new ActiveSkill("Dash", SmallDash, 5.0f, false, index);
            case SkillType.BurstFire:
                return new ActiveSkill("Burst Fire", BurstFire, 10.0f, true, index);
            case SkillType.SpeedBoost:
                return new ActiveSkill("Speed Boost", SmallSpeedBoost, 15.0f, false, index);
            case SkillType.TripleShot:
                return new TripleShotSkill("Triple Shot", this, 15.0f, true, index);
            case SkillType.RamAttack:
                return new RamSkill("Ram", this, 10.0f, false, index);
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
    /// Dash ability, player dashes forward
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool SmallDash(Enemy enemy, ref float time)
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
    /// Gives player a speed boost for 2 seconds
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool SmallSpeedBoost(Enemy enemy)
    {
        //Applies speed effect to player
        _playerStatusEffects.AddStatus(StatusType.ShipSpeed, 2.0f, 2.0f);
        return true;
    }

    /// <summary>
    /// Gives player a speed boost for 2.5 seconds
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool MediumSpeedBoost(Enemy enemy)
    {
        //Applies speed effect to player
        _playerStatusEffects.AddStatus(StatusType.ShipSpeed, 2.5f, 2.0f);
        return true;
    }

    /// <summary>
    /// Gives player a speed boost for 2.5 seconds
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool LargeSpeedBoost(Enemy enemy)
    {
        //Applies speed effect to player
        _playerStatusEffects.AddStatus(StatusType.ShipSpeed, 3.0f, 2.0f);
        return true;
    }

    /// <summary>
    /// Gives player regen for 10 health over 3 seconds
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool SmallRegenertaion(Enemy enemy)
    {
        //Applies speed effect to player
        _playerStatusEffects.AddStatus(StatusType.Regeneration, 3.0f, 10.0f / 3.0f);
        return true;
    }

    /// <summary>
    /// Gives player regen for 15 health over 3 seconds
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool MediumRegenertaion(Enemy enemy)
    {
        //Applies speed effect to player
        _playerStatusEffects.AddStatus(StatusType.Regeneration, 3.0f, 15.0f / 3.0f);
        return true;
    }

    /// <summary>
    /// Gives player regen for 20 health over 3 seconds
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool LargeRegenertaion(Enemy enemy)
    {
        //Applies speed effect to player
        _playerStatusEffects.AddStatus(StatusType.Regeneration, 3.0f, 20.0f / 3.0f);
        return true;
    }

    /// <summary>
    /// Boosts player manuverability for 5 seconds
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool SmallManuverabilityBoost(Enemy enemy)
    {
        //Applies speed effect to player
        _playerStatusEffects.AddStatus(StatusType.Maneuverability, 5.0f, 2.0f);
        return true;
    }

    /// <summary>
    /// Boosts player manuverability for 5 seconds
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool MediumManuverabilityBoost(Enemy enemy)
    {
        //Applies speed effect to player
        _playerStatusEffects.AddStatus(StatusType.Maneuverability, 6.0f, 2.0f);
        return true;
    }

    /// <summary>
    /// Boosts player manuverability for 7 seconds
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool LargeManuverabilityBoost(Enemy enemy)
    {
        //Applies speed effect to player
        _playerStatusEffects.AddStatus(StatusType.Maneuverability, 7.0f, 2.2f);
        return true;
    }

    /// <summary>
    /// Fires a powerful spread shot at the closest enemy
    /// </summary>
    /// <param name="enemy">Enemy to fire at</param>
    /// <returns>If skill was successful</returns>
    private bool BurstFire(Enemy enemy)
    {
        //Enemy is needed for skill to activate
        if(enemy == null)
        {
            return false;
        }

        //Shoot burst shot
        SpecialShot(enemy, 2, 3, 0.5f, 0, 0);

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
        MasterUpgrade upgrade = new MasterUpgrade("triple_shot", shotValues);

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

public class TripleShotSkill : ActiveSkill
{
    private ShipMovement _movementScript;
    private ActiveAbilities _activeAbilities;
    private int _shotCount;

    public TripleShotSkill(string name, ActiveAbilities activeAbilities, float cooldown, bool needsEnemy, int index) : base(name, (OvertimeSkill)null, cooldown, needsEnemy, index)
    {
        _activeAbilities = activeAbilities;
        _movementScript = activeAbilities.GetComponent<ShipMovement>();
        _timeSkill = TripleFire;
    }

    /// <summary>
    /// Shoots three shots in a row towards the closest enemy
    /// </summary>
    /// <param name="enemy">Enemy to fire at</param>
    /// <param name="time">Current time</param>
    /// <returns>If skill was successful</returns>
    private bool TripleFire(Enemy enemy, ref float time)
    {
        //Enemy is needed for skill to activate
        if (enemy == null)
        {
            return true;
        }

        if (time > 0.3f)
        {
            time = 0;
        }

        if (time == 0)
        {
            _activeAbilities.SpecialShot(enemy, 0, 2, 0.4f, 0, 0);
            _shotCount += 1;
        }

        if (_shotCount == 3)
        {
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

    public RamSkill(string name, ActiveAbilities activeAbilities, float cooldown, bool needsEnemy, int index) : base(name, (OvertimeSkill)null, cooldown, needsEnemy, index)
    {
        _activeAbilities = activeAbilities;
        _movementScript = activeAbilities.GetComponent<ShipMovement>();
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
            _ramHitbox = _activeAbilities.CreateHitbox(new Vector3(0, 0, 2), new Vector3(1, 1, 1), HitboxType.PlayerHitbox, 20, Vector2.zero, 0);
            _ramHitbox.OnTrigger += RamHit;
        }

        if (_ramHitbox != null)
        {
            _movementScript.ApplyConstantMoveForce(_movementScript.transform.forward, 20.0f, 1.0f);
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
