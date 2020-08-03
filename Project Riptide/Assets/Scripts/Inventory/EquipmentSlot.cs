using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public Item equipment;

    public EquipmentSlot(Item equipment)
    {
        this.equipment = equipment;
    }

    public void UpdateSlotVisuals()
    {
        //change rarity thing here
        if (equipment.Rarity == 1)
        {
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.white;
        }
        else if (equipment.Rarity == 2)
        {
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color32(27, 150, 71, 255); //green
        }
        else if (equipment.Rarity == 3)
        {
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color32(231, 181, 79, 255);    //gold
        }
        else
        {
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color32(159, 114, 146, 255);   //purple
        }

        //change item icon
        gameObject.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = equipment.Icon;
        
        //change item name
        gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(equipment.Name);
        //change stats
        for (int i = 0; i < equipment.Upgrades.Count; i++)
        {
            //for however many upgrades there are
            if (equipment.Upgrades.Count > i)
            {
                float upgradeVal = equipment.Upgrades[i].upgradeValue;
                if (upgradeVal < 0)
                {
                    gameObject.transform.GetChild(i + 2).GetComponent<TextMeshProUGUI>().color = Color.red;
                    gameObject.transform.GetChild(i + 2).GetComponent<TextMeshProUGUI>().SetText(equipment.Upgrades[i].upgradeType.ToString() + " " + upgradeVal);
                }
                else
                {
                    gameObject.transform.GetChild(i + 2).GetComponent<TextMeshProUGUI>().color = Color.green;
                    gameObject.transform.GetChild(i + 2).GetComponent<TextMeshProUGUI>().SetText(equipment.Upgrades[i].upgradeType.ToString() + " +" + upgradeVal);
                }
            }
            else
            {
                gameObject.transform.GetChild(i + 2).GetComponent<TextMeshProUGUI>().SetText("");
            }
        }

        //check for passive
        if (equipment.PassiveText != null && equipment.PassiveText != "")
        {
            //active
            gameObject.transform.GetChild(7).GetComponent<TextMeshProUGUI>().SetText("Active: " + equipment.ActiveText);
            //active and passive
            gameObject.transform.GetChild(8).GetComponent<TextMeshProUGUI>().SetText("Passive: " + equipment.PassiveText);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1045f, 327.1178f);
        }
        //check for active
        else if (equipment.ActiveText != null && equipment.ActiveText != "") 
        {
            //active
            gameObject.transform.GetChild(7).GetComponent<TextMeshProUGUI>().SetText("Active: " + equipment.ActiveText);
            gameObject.transform.GetChild(8).gameObject.SetActive(false);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1045f, 276.8971f);
        }
        else
        {
            gameObject.transform.GetChild(7).gameObject.SetActive(false);
            gameObject.transform.GetChild(8).gameObject.SetActive(false);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1045f, 225.4819f);
        }
    }

}
