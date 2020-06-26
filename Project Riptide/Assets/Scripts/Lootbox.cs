using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootbox : MonoBehaviour
{
    [SerializeField]
    private List<Mesh> _meshes;
    [SerializeField]
    private List<Material> _rarityMaterials;
    
    public string dropType;

    public List<Item> items;

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Hitbox _hitbox;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _hitbox = GetComponent<Hitbox>();
        _hitbox.OnTrigger += DropItems;
    }
    // Start is called before the first frame update
    void Start()
    {
        
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

    }
}
