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
    private GameObject _camera;

    private void Start()
    {
        _prefabs = new Dictionary<EnemyType, GameObject>();
        //Set up dictionary
        for(int i = 0; i < _spawnInfo.Count; i++)
        {
            _prefabs.Add(_spawnInfo[i].Type, _spawnInfo[i].Prefab);
        }
        _camera = GameObject.Find("Main Camera");
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
            float yPosition = transform.position.y;
            // Adjust spawn height.
            switch (type)
            {
                case EnemyType.ChickenFlock:
                {
                        yPosition += 1f;
                        break;
                }
                case EnemyType.Stingray:
                    {
                        yPosition += 1f;
                        break;
                    }
                case EnemyType.BombCrab:
                    {
                        yPosition += 1f;
                        break;
                    }
                case EnemyType.MonkeyBoss:
                    {
                        yPosition -= 6f;
                        break;
                    }
            }
            Vector3 lookPos = _camera.transform.position - this.transform.position;
            lookPos.y = 0;
            Instantiate(_prefabs[type], new Vector3(transform.position.x, yPosition, transform.position.z), Quaternion.LookRotation(lookPos));
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
