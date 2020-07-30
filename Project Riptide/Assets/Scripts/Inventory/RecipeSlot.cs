using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RecipeSlot : MonoBehaviour
{
    private Recipe recipe;
    public Item itemResult;

    public Recipe Recipe
    {
        get { return recipe; }
        set
        {
            recipe = value;
            //itemResult = PlayerInventory.Instance.GetFromName(recipe.result);
        }
    }

    public RecipeSlot(Recipe recipe)
    {
        this.recipe = recipe;
        itemResult = ItemDB.Instance.FindItem(recipe.result);
    }

    public void UpdateSlotVisuals() //Updates the image and amount text
    {
        //change rarity thing here
        if (itemResult.Rarity == 1)
        {
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.white;
        }
        else if (itemResult.Rarity == 2)
        {
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color32(27, 150, 71, 255); //green
        }
        else if (itemResult.Rarity == 3)
        {
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color32(253, 185, 63, 255);    //gold
        }
        else
        {
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color32(137, 77, 158, 255);   //purple
        }
        //change image
        gameObject.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = itemResult.Icon;
        //change name
        gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().SetText(itemResult.Name);
        //change icons based on recipes
        Transform ingredients = gameObject.transform.Find("Ingredients");
        for (int i = 0; i < 5; i++)
        {
            if (recipe.ingredients.Count > i)
            {
                Item ingredient = ItemDB.Instance.FindItem(recipe.ingredients[i]);
                ingredients.GetChild(i).GetChild(1).GetComponent<Image>().sprite = ingredient.Icon;
                
                if (ingredient.Rarity == 1 && ingredient.Category != ItemCategory.Material)
                {
                    ingredients.GetChild(i).GetChild(0).GetComponent<Image>().color = Color.white;
                }
                else if (ingredient.Rarity == 1)
                {
                    ingredients.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color32(125, 82, 52, 255);
                }
                else if (ingredient.Rarity == 2)
                {
                    ingredients.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color32(27, 150, 71, 255); //green
                }
                else if (ingredient.Rarity == 3)
                {
                    ingredients.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color32(253, 185, 63, 255);    //gold
                }
                else
                {
                    ingredients.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color32(137, 77, 158, 255);   //purple
                }
            }
            else
            {
                ingredients.GetChild(i).gameObject.SetActive(false);
            }
        }
    }


}
