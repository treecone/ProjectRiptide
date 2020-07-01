﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootbox : MonoBehaviour
{
    [SerializeField]
    private List<Mesh> _meshes;
    [SerializeField]
    private List<Material> _rarityMaterials;
    [SerializeField]
    public Inventory _inventory;

    public string dropType;

    public List<Item> items;

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Hitbox _hitbox;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _hitbox = GetComponentInChildren<Hitbox>();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        _hitbox.OnTrigger += DropItems;
        SetShape();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetShape()
    {
        _meshFilter.mesh = _meshes[Random.Range(0, _meshes.Count)];
    }

    private void SetRarity()
    {
        int maxRarity = 0;
        foreach(Item i in items)
        {
            if(i.Rarity > maxRarity)
            {
                maxRarity = i.Rarity;
            }
        }

        _meshRenderer.material = _rarityMaterials[maxRarity - 1];

    }
    public void GenerateItems()
    {
        items = DropData.instance.GetDrops(dropType);
        SetRarity();
    }

    private void DropItems(GameObject obj)
    {
        if (obj.tag == "Player")
        {
            for (int i = 0; i < items.Count; i++)
            {
                _inventory.AddItem(items[i].Name, items[i].Amount);
            }
            Destroy(gameObject);
        }
        
    }
}