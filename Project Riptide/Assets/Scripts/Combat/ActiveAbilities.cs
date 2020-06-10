using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public delegate bool Skill(Enemy enemy);
public delegate void CallbackIndex(int i);
public enum SkillType { Dash = 0, BurstFire = 1 }

public class ActiveAbilities : MonoBehaviour
{
    private const int SKILL_AMOUNT = 2;

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

    // Start is called before the first frame update
    void Start()
    {
        _movementScript = GetComponent<ShipMovement>();
        _cannonFireScript = GetComponent<CannonFire>();
        //Set up skill based on inital values
        for(int i = 0; i < SKILL_AMOUNT; i++)
        {
            SetActiveSkill(i, _skillType[i]);
            _sliders[i] = _buttons[i].GetComponentInChildren<Slider>();
            _sliders[i].gameObject.SetActive(false);
        }
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
                Enemy rightEnemy = _inputManager.CheckEnemy(transform.right);
                Enemy leftEnemy = _inputManager.CheckEnemy(-transform.right);
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
                return new ActiveSkill("Dash", Dash, 5.0f, false, index);
            case SkillType.BurstFire:
                return new ActiveSkill("Burst Fire", BurstFire, 10.0f, true, index);
        }
        return null;
    }

    #region Skills
    /// <summary>
    /// Dash ability, player dashes forward
    /// </summary>
    /// <param name="enemy">Targeted enemy, uncessary for this skill</param>
    /// <returns>If skill was successful</returns>
    private bool Dash(Enemy enemy)
    {
        //Applies force in direction of player velocity
        Vector3 netForce = _movementScript.GetVelocity().normalized;
        netForce *= 500.0f;
        _movementScript.ApplyForce(netForce);
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

        Dictionary<string, float> shotValues = new Dictionary<string, float>()
        {
            {"count", 2 },
            {"damage", 3 },
            {"shotSize", 0.5f}
        };
        MasterUpgrade burstUpgrade = new MasterUpgrade("burst_fire", shotValues);

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

        _cannonFireScript.Fire(transform.right, new Vector3(diff.x, 0, diff.z), offset, burstUpgrade);
        return true;
    }
    #endregion
}

public class ActiveSkill
{
    private Skill _skill;
    private float _maxCooldown;
    private float _currCooldown;
    private bool _needsEnemy;
    private string _name;
    private int _index;

    public float MaxCooldown => _maxCooldown;
    public float CurrCooldown => _currCooldown;
    public bool NeedsEnemy => _needsEnemy;
    public string Name => _name;

    public event CallbackIndex OnCooldownEnd;

    public ActiveSkill(string name, Skill skill, float cooldown, bool needsEnemy, int index)
    {
        _skill = skill;
        _maxCooldown = cooldown;
        _currCooldown = 0;
        _needsEnemy = needsEnemy;
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
        if(_currCooldown > 0)
        {
            _currCooldown -= time;
            if(_currCooldown <= 0)
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
            //Only set cooldown if skill is successful
            if (_skill(enemy))
            {
                //Set cooldown
                _currCooldown = _maxCooldown;
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
            if (_skill(null))
            {
                //Set cooldown
                _currCooldown = _maxCooldown;
            }
        }
    }
}
