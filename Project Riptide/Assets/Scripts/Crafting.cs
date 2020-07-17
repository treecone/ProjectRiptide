using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using System;

public class Crafting : MonoBehaviour
{
    private string _defaultPath;
    private string _jsonString;
    private JsonData _recipeData;

    
    [SerializeField]
    private List<Recipe> _recipes;

    void Awake()
    {
        _recipes = new List<Recipe>();
        _defaultPath = Application.dataPath + "/Resources/Crafting/Recipes.json";
        _jsonString = File.ReadAllText(_defaultPath);
        Debug.Log(_jsonString);
        _recipeData = JsonMapper.ToObject(_jsonString);
        for(int i = 0; i < _recipeData.Count; i++)
        {
            List<string> ingredients = new List<string>();
            foreach(JsonData j in _recipeData[i]["ingredients"])
            {
                ingredients.Add(j.ToString());
            }
            List<int> ingredientAmounts = new List<int>();
            foreach(JsonData j in _recipeData[i]["ingredientAmounts"])
            {
                ingredientAmounts.Add(int.Parse(j.ToString()));
            }

            string result = _recipeData[i]["result"].ToString();
            int resultAmount = int.Parse(_recipeData[i]["resultAmount"].ToString());
            _recipes.Add(new Recipe(ingredients, ingredientAmounts, result, resultAmount));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Crafts an item, removing its ingredients from the inventory
    /// and returning the item crafted
    /// </summary>
    /// <param name="recipe">The recipe to craft</param>
    /// <returns>The item crafted, or null if the item cannot be crafted</returns>
    Item Craft(Recipe recipe)
    {
        if(CanCraft(recipe))
        {
            for (int i = 0; i < recipe.ingredients.Count; i++)
            {
                PlayerInventory.Instance.RemoveItem(recipe.ingredients[i], recipe.ingredientAmounts[i]);
            }
            PlayerInventory.Instance.AddItem(recipe.result, recipe.resultAmount);
        }
        Debug.LogWarning("Not enough items in inventory to craft " + recipe.result);
        return null;
    }

    /// <summary>
    /// Determines if a recipe can be crafted
    /// </summary>
    /// <param name="recipe">The recipe to be crafted</param>
    /// <returns>A boolean indicating whether the item can be crafted</returns>
    bool CanCraft(Recipe recipe)
    {
        for(int i = 0; i < recipe.ingredients.Count; i++)
        {
            if(PlayerInventory.Instance.CountOf(recipe.ingredients[i]) < recipe.ingredientAmounts[i])
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Returns all recipes that can be crafted, regardless of whether you have the reuslt or not
    /// </summary>
    List<Recipe> ValidRecipes()
    {
        List<Recipe> validRecipes = new List<Recipe>();
        foreach(Recipe r in _recipes)
        {
            if(CanCraft(r))
            {
                validRecipes.Add(r);
            }
        }
        return validRecipes;
    }

    /// <summary>
    /// Returns all recipes that are not already crafted, i.e. equipment that you already have
    /// </summary>
    List<Recipe> UncraftedRecipes()
    {
        List<Recipe> validRecipes = new List<Recipe>();
        foreach (Recipe r in _recipes)
        {
            if (ItemDB.Instance.FindItemNoClone(r.result).Category == ItemCategory.Material || !PlayerInventory.Instance.HasEquipment(r.result))
            {
                validRecipes.Add(r);
            }
        }
        return validRecipes;
    }

    /// <summary>
    /// Combines Valid and Uncrafted Recipes, finding recipes that are both not already crafted AND you can craft
    /// </summary>
    List<Recipe> ValidUncraftedRecipes()
    {
        List<Recipe> validRecipes = new List<Recipe>();
        foreach (Recipe r in _recipes)
        {
            if ((ItemDB.Instance.FindItemNoClone(r.result).Category == ItemCategory.Material || !PlayerInventory.Instance.HasEquipment(r.result)) && CanCraft(r))
            {
                validRecipes.Add(r);
            }
        }
        return validRecipes;
    }
}
