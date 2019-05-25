using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Mira Antolovich
/// Health Bars
/// </summary>
public class HealthBar : MonoBehaviour
{
    //timing for keyboard state (FOR DEBUGGING PURPOSES)
    private float timing;

    //health
    public int maxHealth;
    public int currentHealth;
    private float pctHealth;

    //health bar
    public Image foregroundImage;
    public float updateSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (timing <= 0f)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ModifyHealth(-10);
                timing += .5f;
            }
        }
        else
        {
            timing -= Time.deltaTime;
        }

    }

    /// <summary>
    /// IEnumerator that changes health bar smoothly
    /// </summary>
    /// <param name="pct">percent health</param>
    /// <returns>null</returns>
    private IEnumerator ChangeToPctHealth(float pct)
    {
        float preChangePct = foregroundImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < updateSpeed)
        {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(preChangePct, pct, elapsed / updateSpeed);
            yield return null;
        }
        foregroundImage.fillAmount = pct;
    }

    /// <summary>
    /// handles when health changes
    /// </summary>
    /// <param name="pct">takes percent health</param>
    private void HandleHealthChange(float pct)
    {
        StartCoroutine(ChangeToPctHealth(pct));
    }

    public void ModifyHealth(int amount)
    {
        currentHealth += amount;
        float currentHealthPct = (float)currentHealth / (float)maxHealth;
        HandleHealthChange(currentHealthPct);
    }


}
