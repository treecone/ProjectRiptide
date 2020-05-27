using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryMethods : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textField;
    private Item activeItem;

    /// <summary>
    /// changes trash number
    /// </summary>
    /// <param name="num">change number in TextMeshPro</param>
    public void ChangeNumber(int num)
    {
        int amount = System.Convert.ToInt32(textField.text);

        /* when active item is here
        if (amount >= activeItem.Amount)
        {
            amount = activeItem.Amount;
        }
        else if (amount != 0)*/
        if (amount != 0)
        {
            amount += num;
        }
        textField.SetText("{0}", amount);
    }

    /// <summary>
    /// Sets time scale to 0, may add animation here later
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    /// <summary>
    /// Sets time scale to 1, may add animation here later
    /// </summary>
    public void UnpauseGame()
    {
        Time.timeScale = 1.0f;
    }
}
