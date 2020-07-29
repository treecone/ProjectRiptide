using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Assets/Singletons/ShotDB")]
public class ShotDB : ScriptableObject
{
    private static ShotDB _instance;

    public static ShotDB Instance
    {
        get
        {
            if (!_instance)
                _instance = Resources.LoadAll<ShotDB>("ScriptableObjectInstances")[0];
            return _instance;
        }
    }

    private Dictionary<string, ShotData> _shotDict;

    [SerializeField]
    private ShotData defaultShot;
    [SerializeField]
    private List<ShotData> shotData;

    public GameObject currentShot;
    void OnEnable()
    {
        _instance = this;
        LoadShots();
    }

    public void LoadShots()
    {
        _shotDict = new Dictionary<string, ShotData>();
        foreach(ShotData s in shotData)
        {
            _shotDict[s.name] = s;
        }
        currentShot = defaultShot.shotPrefab;
    }

    public GameObject GetShotPrefab(string name)
    {
        if(_shotDict.ContainsKey(name))
        {
            return _shotDict[name].shotPrefab;
        } else
        {
            return defaultShot.shotPrefab;
        }
    }

    public void SetCurrentShot()
    {
        currentShot = GetShotPrefab(PlayerInventory.Instance.GetEquippedItemOfCategory(ItemCategory.Cannon).ShotType);
    }
}
