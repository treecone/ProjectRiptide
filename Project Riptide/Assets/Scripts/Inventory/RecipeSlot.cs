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
        itemResult = null;
        for (int i = 0; i < PlayerInventory.Instance.items.Count; i++)
        {
            if (PlayerInventory.Instance.items[i].Name == recipe.result)
            {
                itemResult = PlayerInventory.Instance.items[i];
                break;
            }
        }
    }

    public void UpdateSlotVisuals() //Updates the image and amount text
    {
        //change rarity thing here
        if (itemResult.Rarity == 1)
        {
            gameObject.transform.Find("Icon").GetComponent<Image>().color = Color.white;
        }
        else if (itemResult.Rarity == 2)
        {
            gameObject.transform.Find("Icon").GetComponent<Image>().color = new Color(27, 150, 71); //green
        }
        else if (itemResult.Rarity == 3)
        {
            gameObject.transform.Find("Icon").GetComponent<Image>().color = new Color(231, 181, 79);    //gold
        }
        else
        {
            gameObject.transform.Find("Icon").GetComponent<Image>().color = new Color(159, 114, 146);   //purple
        }
        //change image
        gameObject.transform.Find("Icon").GetComponentInChildren<Image>().sprite = itemResult.Icon;
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
