using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootbox : MonoBehaviour
{
    [SerializeField]
    private List<Mesh> _meshes;
    [SerializeField]
    private List<Material> _rarityMaterials;

    public Inventory inventory;

    public string dropType;

    public List<Item> items;

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Hitbox _hitbox;

    private int rarity;

    private TextDisplay _textDisplay;

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
        _textDisplay = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TextDisplay>();
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
        foreach (Item i in items)
        {
            if (i.Rarity > maxRarity)
            {
                maxRarity = i.Rarity;
            }
        }

        _meshRenderer.material = _rarityMaterials[maxRarity - 1];
        rarity = maxRarity;

    }
    public void GenerateItems()
    {
        items = DropManager.Instance.GetDrops(dropType);
        SetRarity();
    }

    private void DropItems(GameObject obj)
    {
        if (obj.tag == "Player")
        {
            for (int i = 0; i < items.Count; i++)
            {
                Upgrades upgrades = obj.GetComponent<Upgrades>();
                if (items[i].Name == "gold" || items[i].Name == "Gold")
                {
                    PlayerInventory.Instance.AddItem(items[i].Name, (int)(items[i].Amount * (1 + upgrades.masterUpgrade[StatusType.BonusGold])));
                    _textDisplay.AddText("+" + (int)(items[i].Amount * ( 1 + upgrades.masterUpgrade[StatusType.BonusGold])) + " " + items[i].Name, Color.yellow);
                }
                else
                {
                    PlayerInventory.Instance.AddItem(items[i].Name, items[i].Amount);
                    _textDisplay.AddText("+" + items[i].Amount + " " + items[i].Name, Color.black);
                }

            }
            inventory.UpdateInventoryVisuals();
            switch (rarity)
            {
                case 1:
                    SoundManager.instance.PlaySound("ItemCommon");
                    break;
                case 2:
                    SoundManager.instance.PlaySound("ItemUncommon");
                    break;
                case 3:
                    SoundManager.instance.PlaySound("ItemRare");
                    break;
                case 4:
                    SoundManager.instance.PlaySound("ItemLegendary");
                    break;
            }

            Destroy(gameObject);
        }

    }
}
