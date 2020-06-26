using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LootSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _lootboxPrefab;

    [SerializeField]
    private string _dropType;
    void Start()
    {
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            DropLootbox();
        }
    }
    public void DropLootbox ()
    {
        GameObject drop = Instantiate(_lootboxPrefab);
        drop.transform.position = transform.position;
        drop.transform.rotation = Random.rotation;
        Lootbox lootbox = drop.GetComponent<Lootbox>();
        lootbox.dropType = _dropType;
        lootbox.GenerateItems();
    }
}
