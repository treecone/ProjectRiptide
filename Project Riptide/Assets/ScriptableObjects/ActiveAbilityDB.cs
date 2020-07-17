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
}
