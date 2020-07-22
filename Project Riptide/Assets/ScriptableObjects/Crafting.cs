using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using System;

[CreateAssetMenu(menuName = "Custom Assets/Singletons/Crafting")]
public class Crafting : ScriptableObject
{
    public static Crafting _instance;

    public static Crafting Instance
    {
        get
        {
            if(_instance)
            {
                _instance = Resources.LoadAll<Crafting>("ScriptableObjectInstances")[0];
            }
            return _instance;

        }
    }
    private void OnEnable()
    {
        _instance = this;
        if(_reloadRecipesOnLoad)
        {
            LoadRecipes();
            _reloadRecipesOnLoad = false;
        }
    }

    [SerializeField]
    private bool _reloadRecipesOnLoad;
    [SerializeField]
    private List<Recipe> _recipes;
    private void LoadRecipes()
    {
        _recipes = new List<Recipe>();
        Recipe[] allRecipes = Resources.LoadAll<Recipe>("ScriptableObjectInstances");
        for (int i = 0; i < allRecipes.Length; i++)
        {
            _recipes.Add(allRecipes[i]);
        }
    }

    /// <summary>
    /// Crafts an item, removing its ingredients from the inventory
    /// and returning the item crafted
    /// </summary>
    /// <param name="recipe">The recipe to craft</param>
    /// <returns>The item crafted, or null if the item cannot be crafted</returns>
    public Item Craft(Recipe recipe)
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
    public bool CanCraft(Recipe recipe)
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

    public List<Recipe> Recipes()
    {
        return _recipes;
    }
    /// <summary>
    /// Returns all recipes that can be crafted, regardless of whether you have the reuslt or not
    /// </summary>
    public List<Recipe> ValidRecipes()
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
    public List<Recipe> UncraftedRecipes()
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
    public List<Recipe> ValidUncraftedRecipes()
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

    public bool IsUncrafted(Recipe recipe)
    {
        return ItemDB.Instance.FindItemNoClone(recipe.result).Category == ItemCategory.Material ||
            !PlayerInventory.Instance.HasEquipment(recipe.result);
    }

}
