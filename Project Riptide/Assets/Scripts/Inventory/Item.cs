﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item 
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
    public List<Upgrade> upgrades;
    public bool equipped;

    public Item(int id, string itemName, string description, bool stackable, int rarity, int value, string slug, Sprite sprite, int amount, int maxAmount, List<Upgrade> upgrades)
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
        this.upgrades = upgrades;
        this.equipped = true;
    }
}

