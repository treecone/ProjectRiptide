﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Assets/Recipe")]
public class Recipe : ScriptableObject
{
    public List<string> ingredients;
    public List<int> ingredientAmounts;

    public string result;
    public int resultAmount;
    
    public Recipe(List<string> ingredients, List<int> ingredientAmounts, string result, int resultAmount)
    {
        this.ingredients = ingredients;
        this.ingredientAmounts = ingredientAmounts;
        this.result = result;
        this.resultAmount = resultAmount;
    }
}
