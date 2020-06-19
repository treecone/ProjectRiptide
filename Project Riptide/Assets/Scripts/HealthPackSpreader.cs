using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPackSpreader : MonoBehaviour
{

    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private GameObject _healthPackPrefab;

    [SerializeField]
    private float _minRadius;

    [SerializeField]
    private float _maxRadius;

    [SerializeField]
    private int _numHealthPacks;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _numHealthPacks; i++)
        {
            GameObject newObj = Instantiate(_healthPackPrefab, Random.onUnitSphere * Random.Range(_minRadius, _maxRadius), Random.rotation, transform);
            newObj.transform.position -= new Vector3(0, newObj.transform.position.y, 0);
            newObj.GetComponent<Mine>().SetData(_player, -12.5f, StatusType.Regeneration, 1.25f, 10);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
