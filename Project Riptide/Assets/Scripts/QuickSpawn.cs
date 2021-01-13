using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject _spawnButtons;
    [SerializeField]
    private Button _hideButton;
    [SerializeField]
    private List<SpawnInfo> _spawnInfo;
    private Dictionary<EnemyType, GameObject> _prefabs;
    private bool _hidden = false;

    private void Start()
    {
        _prefabs = new Dictionary<EnemyType, GameObject>();
        //Set up dictionary
        for(int i = 0; i < _spawnInfo.Count; i++)
        {
            _prefabs.Add(_spawnInfo[i].Type, _spawnInfo[i].Prefab);
        }
    }

    /// <summary>
    /// Toggles whether or not spawn buttons are displayed
    /// </summary>
    public void ToggleSpawnButtons()
    {
        if (_hidden)
        {
            _spawnButtons.SetActive(true);
            _hideButton.GetComponentInChildren<Text>().text = "Hide Spawn Buttons";
            _hidden = false;
        }
        else
        {
            _spawnButtons.SetActive(false);
            _hideButton.GetComponentInChildren<Text>().text = "Unhide Spawn Buttons";
            _hidden = true;
        }
    }

    /// <summary>
    /// Spawn an enemy of given type
    /// </summary>
    /// <param name="type">Type of enemy</param>
    public void SpawnEnemy(int typeid)
    {
        EnemyType type = (EnemyType)typeid;
        //EnemyType type = EnemyType.KoiBoss;
        if(_prefabs.ContainsKey(type))
        {
            Instantiate(_prefabs[type], transform.position, transform.rotation);
        }
        else
        {
            Debug.LogWarning("Enemy type " + type + " could not be spawned");
        }
    }
}

[System.Serializable]
public class SpawnInfo
{
    [SerializeField]
    private EnemyType _type;
    public EnemyType Type => _type;
    [SerializeField]
    private GameObject _prefab;
    public GameObject Prefab => _prefab;
}
