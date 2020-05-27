using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    #region Fields
    [SerializeField]
    private int id;                     //id for items (use this to compare)
    private string name;                //name
    private string description;         //tooltip for description
    private int rarity;                 //rarity color
    private int value;                  //gold value
    private string slug;                //lowercase names with no spaces
    private Sprite icon;                //sprite of item
    private int amount;                 //current amount of item
    private int maxAmount;              //max amount of items
    private List<Upgrade> upgrades;
    private bool equipped;
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

    #endregion

    public Item(int id, string itemName, string description, int rarity, int value, string slug, Sprite icon, int amount, int maxAmount, List<Upgrade> upgrades)
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
    }
}

