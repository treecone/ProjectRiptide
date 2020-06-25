using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootbox : MonoBehaviour
{
    [SerializeField]
    private List<Mesh> _meshes;
    [SerializeField]
    private List<Material> _rarityMaterials;

    [SerializeField]
    private string _dropType;

    public List<Item> items;

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Hitbox _hitbox;
    // Start is called before the first frame update
    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _hitbox = GetComponentInChildren<Hitbox>();
        _hitbox.OnTrigger += DropItems;
        SetShape();
        transform.rotation = Random.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            GenerateItems();
        }
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
    private void GenerateItems()
    {
        items = DropData.instance.GetDrops(_dropType);
        SetRarity();
    }

    private void DropItems(GameObject obj)
    {

    }
}
