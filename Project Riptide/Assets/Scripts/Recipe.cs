using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    public List<string> ingredients;
    public List<int> ingredientAmounts;

    public string result;
    public int resultAmount;

    public List<Upgrade> upgrades;
    public Recipe(List<string> ingredients, List<int> ingredientAmounts, string result, int resultAmount, List<Upgrade> upgrades)
    {
        this.ingredients = ingredients;
        this.ingredientAmounts = ingredientAmounts;
        this.result = result;
        this.resultAmount = resultAmount;
        this.upgrades = upgrades;
    }
}
