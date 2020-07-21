using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RecipeSlot : MonoBehaviour
{
    public Recipe recipe;
    public Item itemResult;

    public RecipeSlot(Recipe recipe)
    {
        this.recipe = recipe;
        itemResult = PlayerInventory.Instance.GetFromName(recipe.result);
    }

    public void UpdateSlotVisuals() //Updates the image and amount text
    {
        //if it has not been crafted yet
        if (PlayerInventory.Instance.GetFromName(recipe.result).Amount == 0)
        {
            //change rarity thing here
            if (itemResult.Rarity == 1)
            {
                gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.white;
            }
            else if (itemResult.Rarity == 2)
            {
                gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(27, 150, 71); //green
            }
            else if (itemResult.Rarity == 3)
            {
                gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(231, 181, 79);    //gold
            }
            else
            {
                gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(159, 114, 146);   //purple
            }
            //change image
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = itemResult.Icon;
            //change name
            gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().SetText(itemResult.Name);
            //change icons based on recipes
            for (int i = 0; i < 4; i++)
            {
                if (recipe.ingredients.Count < i)
                {
                    Item ingredient = PlayerInventory.Instance.GetFromName(recipe.ingredients[i]);
                    gameObject.transform.Find("Ingredients").GetChild(i).GetComponentInChildren<Image>().sprite = ingredient.Icon;
                }
                else
                {
                    gameObject.transform.Find("Ingredients").GetChild(i).GetComponent<GameObject>().SetActive(false);
                }
            }
        }
    }


}
