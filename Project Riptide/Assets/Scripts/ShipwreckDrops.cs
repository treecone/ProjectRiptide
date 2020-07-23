using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipwreckDrops : MonoBehaviour
{
    [SerializeField]
    private GameObject _lootboxPrefab;

    [SerializeField]
    private float _radius;

    [SerializeField]
    private int _count;
    
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < _count; i++)
        {
            Vector3 offset = transform.position + Random.onUnitSphere * _radius;
            offset = new Vector3(offset.x, 0, offset.z);
            GameObject drop = Instantiate(_lootboxPrefab, offset, Random.rotation, transform);
            Lootbox lootbox = drop.GetComponent<Lootbox>();
            lootbox.inventory = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Inventory>();
            lootbox.dropType = "shipwreck";
            lootbox.GenerateItems();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
