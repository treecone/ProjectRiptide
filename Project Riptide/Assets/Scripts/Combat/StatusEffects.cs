using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    [SerializeField]
    private List<StatusEffect> _activeStatusEffects;
    private PlayerHealth _playerHealth;
    private Enemy _enemyHealth;
    private Upgrades _upgrades;
    private StatusIcons _statusIcons;
    //TODO: add enemy health interaction


    [SerializeField]
    private bool isPlayer;
    
    public List<StatusEffect> ActiveStatusEffects
    {
        get
        {
            return _activeStatusEffects;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _statusIcons = GetComponentInChildren<StatusIcons>();
        _activeStatusEffects = new List<StatusEffect>();
        _upgrades = GetComponent<Upgrades>();
        if (isPlayer)
        {
            _playerHealth = GetComponent<PlayerHealth>();
        }else
        {
            _enemyHealth = GetComponent<Enemy>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < _activeStatusEffects.Count; i++)
        {
            StatusEffect status = _activeStatusEffects[i];
            status.Update();
            if(status.Active)
            {
                if(status.Type == StatusType.Fire|| status.Type == StatusType.Poison)
                {
                    if(isPlayer)
                    {
                        _playerHealth.TakeDamage(status.Level * Time.deltaTime);
                    } else
                    {
                        _enemyHealth.TakeDamage(status.Level * Time.deltaTime);
                    }
                }
            } else
            {
                _activeStatusEffects.RemoveAt(i);
                if(_upgrades != null)
                {
                    _upgrades.Recalculate();
                }
                
                _statusIcons.RearrangeStatuses(ActiveStatusEffects);
                i--;
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            AddStatus(StatusType.Fire, 5, 5);
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddStatus(StatusType.Regeneration, 5, 5);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddStatus(StatusType.MaxHealth, 5, 30);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AddStatus(StatusType.Poison, 5, 5);
        }
    }

    public void AddStatus(StatusType type, float duration, float level)
    {
        StatusEffect s = new StatusEffect(type, duration, level);
        ActiveStatusEffects.Add(s);
        _statusIcons.RearrangeStatuses(ActiveStatusEffects);
        if(_upgrades != null)
        {
            _upgrades.Recalculate();
        }
    }

    public void AddStatus(StatusType type, string tag, float duration, float level)
    {
        StatusEffect s = new StatusEffect(type, tag, duration, level);
        ActiveStatusEffects.Add(s);
        _statusIcons.RearrangeStatuses(ActiveStatusEffects);
        if (_upgrades != null)
        {
            _upgrades.Recalculate();
        }
    }

    public void RemoveStatus(string tag)
    {
        for (int i = 0; i < _activeStatusEffects.Count; i++)
        {
            StatusEffect status = _activeStatusEffects[i];
            if (status.Tag == tag)
            {
                _activeStatusEffects.RemoveAt(i);
                if (_upgrades != null)
                {
                    _upgrades.Recalculate();
                }

                _statusIcons.RearrangeStatuses(ActiveStatusEffects);
                i--;
            }
        }
    }

    public void ClearStatuses()
    {
        ActiveStatusEffects.Clear();
        _statusIcons.RearrangeStatuses(ActiveStatusEffects);
        if (_upgrades != null)
        {
            _upgrades.Recalculate();
        }
    }

    /// <summary>
    /// Checks to see if a certain status is active
    /// </summary>
    /// <param name="tag">Tag of status to check</param>
    /// <returns>If status was found</returns>
    public bool CheckStatus(string tag)
    {
        for (int i = 0; i < _activeStatusEffects.Count; i++)
        {
            StatusEffect status = _activeStatusEffects[i];
            if (status.Tag == tag)
            {
                return true;
            }
        }
        return false;
    }
}

[System.Serializable]
public class StatusEffect
{
    private StatusType _type;
    private float _duration;
    private float _level;
    private string _tag;

    private float _currentDuration;

    public StatusType Type
    {
        get
        {
            return _type;
        }
    }

    public float Level
    {
        get
        {
            return _level;
        }
    }

    public float Duration
    {
        get
        {
            return _duration;
        }
    }
    public bool Active
    {
        get
        {
            return _currentDuration < _duration;
        }
    }

    public string Tag
    {
        get
        {
            return _tag;
        }
    }
    public StatusEffect(StatusType type, float duration, float level)
    {
        _type = type;
        _tag = "noTag";
        _duration = duration;
        _level = level;

        _currentDuration = 0;
    }

    public StatusEffect(StatusType type, string tag, float duration, float level)
    {
        _type = type;
        _tag = tag;
        _duration = duration;
        _level = level;

        _currentDuration = 0;
    }

    public void Update()
    {
        _currentDuration += Time.deltaTime;
    }
}

public class StatusEffectComparer : IComparer<StatusEffect>
{
    public int Compare(StatusEffect x, StatusEffect y)
    {
        if(x.Type.CompareTo(y.Type) != 0)
        {
            return x.Type.CompareTo(y.Type);
        } else if (x.Level.CompareTo(y.Level) != 0){
            return x.Level.CompareTo(y.Level);
        } else if(x.Duration.CompareTo(y.Duration) != 0)
        {
            return x.Duration.CompareTo(y.Duration);
        } else
        {
            return 0;
        }
    }
}
