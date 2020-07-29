using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCategory { Ship = 0, Sails = 1, Hull = 2, Cannon = 3, Trinket = 4, Material = 5}
[System.Serializable]
public class Item
{
    #region Fields
    [SerializeField]
    private int id;                     //id for items (use this to compare)
    [SerializeField]
    private string name;                //name
    private string description;         //tooltip for description
    private int rarity;                 //rarity color
    [SerializeField]
    private int value;                  //gold value
    private string slug;                //lowercase names with no spaces
    private Sprite icon;                //sprite of item
    [SerializeField]
    private int amount;                 //current amount of item
    private int maxAmount;              //max amount of items
    private List<Upgrade> upgrades;
    private bool equipped;
    private ItemCategory category;
    private int equipmentRank;
    private string activeText;
    private string passiveText;
    private string shotType;
    #endregion

    #region Properties
    public int Id
    {
        get
        {
            return id;
        }
    }
    public string Name
    {
        get
        {
            return name;
        }
    }

    public string Description
    {
        get
        {
            return description;
        }
    }

    public int Rarity
    {
        get
        {
            return rarity;
        }
    }

    public int Value
    {
        get
        {
            return value;
        }
    }

    public string Slug
    {
        get
        {
            return slug;
        }
    }

    public Sprite Icon
    {
        get
        {
            return icon;
        }
    }

    public int Amount
    {
        get
        {
            return amount;
        }
        set
        {
            amount = value;
        }
    }

    public int MaxAmount
    {
        get
        {
            return maxAmount;
        }
    }

    public List<Upgrade> Upgrades
    {
        get
        {
            return upgrades;
        }
    }

    public bool Equipped
    {
        get
        {
            return equipped;
        }
        set
        {
            equipped = value;
        }
    }

    public ItemCategory Category
    {
        get
        {
            return category;
        }
    }

    public int EquipmentRank
    {
        get
        {
            return equipmentRank;
        }
    }

    public string PassiveText
    {
        get
        {
            return passiveText;
        }
    }

    public string ActiveText
    {
        get
        {
            return activeText;
        }
    }

    public string ShotType
    {
        get
        {
            return shotType;
        }
    }
    #endregion

    public Item(int id, string itemName, string description, int rarity, int value, string slug, Sprite icon, int amount, int maxAmount, List<Upgrade> upgrades, ItemCategory category)
    {
        this.id = id;
        this.name = itemName;
        this.description = description;
        this.rarity = rarity;
        this.value = value;
        this.slug = slug;
        this.icon = icon;
        this.amount = amount;
        this.maxAmount = maxAmount;
        this.upgrades = upgrades;
        this.equipped = true;
        this.category = category;
    }

    public Item(Item other)
    {
        this.id = other.id;
        this.name = other.name;
        this.description = other.description;
        this.rarity = other.rarity;
        this.value = other.value;
        this.slug = other.slug;
        this.icon = other.icon;
        this.amount = other.amount;
        this.maxAmount = other.maxAmount;
        this.upgrades = new List<Upgrade>();
        foreach(Upgrade u in other.upgrades)
        {
            this.upgrades.Add(u);
        }
        this.equipped = other.equipped;
        this.category = other.category;
    }
}

