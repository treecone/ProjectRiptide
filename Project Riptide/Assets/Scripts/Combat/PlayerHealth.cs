using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private Upgrades shipUpgradeScript;

    public GameObject healthBarObject;
    private Camera camera;

    private HealthBar healthBar;
    private float maxHealth;
    private float health;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 100;
        health = 100;
        healthBar = GetComponent<HealthBar>();
        healthBar.SetMaxHealth(100);
        camera = Camera.main;
        InvokeRepeating("UpdateHealth", 1.0f, 1.0f);
    }

    private void Update()
    {
        //Make health bar face player
        //healthBarObject.transform.rotation = new Quaternion(camera.transform.rotation.x, camera.transform.rotation.y, camera.transform.rotation.z, camera.transform.rotation.w);
    }

    /// <summary>
    /// Behavior for every second - health regen, update healthbar, etc
    /// </summary>
    private void UpdateHealth()
    {
        AddHealth(shipUpgradeScript.masterUpgrade["regeneration"]);

        //if the player's max health changed, then add health according to the change
        //in other words, adding max health adds the same amount of current health
        float lastMaxHealth = maxHealth;
        maxHealth = 100 + shipUpgradeScript.masterUpgrade["maxHealth"];
        if(lastMaxHealth != maxHealth)
        {
            health += maxHealth - lastMaxHealth;
        }
    }

    /// <summary>
    /// Player adds health, up to their max health
    /// </summary>
    /// <param name="health">The amount of health to add</param>
    public void AddHealth(float health)
    {
        this.health += health;
    }

    /// <summary>
    /// Player takes damage, if health is 0 they die
    /// </summary>
    /// <param name="damage">Amount of damage taken</param>
    public void TakeDamage(float damage)
    {
        health -= damage / (1.0f / (shipUpgradeScript.masterUpgrade["armor"] + 1.0f));
        healthBar.UpdateHealth(health);
        if (health <= 0)
        {
            health = 0;
            //Kill Player
            //SceneManager.LoadScene("CadenScene");
        }
    }
}
