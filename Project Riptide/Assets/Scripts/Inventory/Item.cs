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
    private List<Upgrade> upgrades;     //?????? we keep this here or no
    private bool equipped;              //?????? we keep this here or no
    #endregion

    #region Get/Set
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Rarity { get; set; }
    public int Value { get; set; }
    public string Slug { get; set; }
    public Sprite Icon { get; set; }
    public int Amount { get; set; }
    public int MaxAmount { get; set; }
    //KEEPING THESE OR NAH?
    public List<Upgrade> Upgrades { get; set; }
    public bool Equipped { get; set; }
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

