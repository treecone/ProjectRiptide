using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private float maxHealth;
    private float currentHealth;

    //health bar
    public Slider slider;
    [SerializeField]
    private GameObject _sliderText;
    private TextMeshProUGUI _textMesh;

    // Start is called before the first frame update
    void Start()
    {
        if (_sliderText != null)
        {
            _textMesh = _sliderText.GetComponent<TextMeshProUGUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = CalculateHealth();
        if (_textMesh != null)
        {
            _textMesh.text = "" + currentHealth.ToString("N0") + "/" + maxHealth;
        }
        if(currentHealth <= 0)
        {
            //Kill Monster
        }

        if(currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    //Calculates percentage of health remaining
    private float CalculateHealth()
    {
        return currentHealth / maxHealth;
    }

    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = this.maxHealth;
    }

    public void UpdateHealth(float currentHealth)
    {
        this.currentHealth = currentHealth;
    }

    /// <summary>
    /// Monster takes damage, if health is 0 they die
    /// </summary>
    /// <param name="damage">Amount of damage taken</param>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealth(currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            //Kill monster
        }
    }

    /*
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
            Debug.Log("Health stuff");
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
    */

}
