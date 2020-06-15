using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    private List<StatusEffect> _activeStatusEffects;
    private Upgrade _totalUpgrade;
    private PlayerHealth _playerHealth;
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
        _activeStatusEffects = new List<StatusEffect>();
        if(isPlayer)
        {
            _playerHealth = GetComponent<PlayerHealth>();
        }else
        {

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
                if(status.Type == "fire" || status.Type == "poison")
                {
                    if(isPlayer)
                    {
                        _playerHealth.TakeDamage(status.Level * Time.deltaTime);
                    } else
                    {

                    }
                }
            } else
            {
                _activeStatusEffects.RemoveAt(i);
                i--;
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            ActiveStatusEffects.Add(new StatusEffect("fire", 5, 5));
        }
    }
}

public class StatusEffect
{
    private string _type;
    private float _duration;
    private float _level;
    private Color _color;

    private float _currentDuration;

    public string Type
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
    public bool Active
    {
        get
        {
            return _currentDuration < _duration;
        }
    }
    public StatusEffect(string type, float duration, float level)
    {
        _type = type;
        _duration = duration;
        _level = level;

        _currentDuration = 0;
    }

    public void Update()
    {
        _currentDuration += Time.deltaTime;
    }
}
