using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Item 
{

    public int id; //These are public, and thus not secure...
    public string name;
    public string description;
    public bool stackable;
    public int rarity;
    public int value;
    public string slug;
    public Sprite sprite;
    public int amount;
    public int maxAmount;

    public Item(int id, string itemName, string description, bool stackable, int rarity, int value, string slug, Sprite sprite, int amount, int maxAmount)
    {
        this.id = id;
        this.name = itemName;
        this.description = description;
        this.stackable = stackable;
        this.rarity = rarity;
        this.value = value;
        this.slug = slug;
        this.sprite = sprite;
        this.amount = amount;
        this.maxAmount = maxAmount;
    }
}

