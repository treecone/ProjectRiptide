using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineCircle : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private GameObject _minePrefab;

    [SerializeField]
    private float radius;

    [SerializeField]
    private int numMines;

    [SerializeField]
    private float damage;
    [SerializeField]
    private StatusType statusType;
    [SerializeField]
    private float statusDuration;
    [SerializeField]
    private float statusLevel;

    // Start is called before the first frame update
    void Start()
    {
        for(float i = 0; i < Mathf.PI * 2; i += Mathf.PI * 2 / numMines)
        {
            GameObject mine = Instantiate(_minePrefab, new Vector3(Mathf.Sin(i) * radius, transform.position.y, Mathf.Cos(i) * radius), Random.rotation, transform);
            mine.GetComponent<Mine>().SetData(_player, damage, statusType, statusDuration, statusLevel);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
