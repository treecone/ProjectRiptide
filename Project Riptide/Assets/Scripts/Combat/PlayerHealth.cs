using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private Upgrades shipUpgradeScript;

    public GameObject healthBarObject;
    private Camera camera;

    private HealthBar healthBar;
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private float health;
    [SerializeField]
    private GameObject _deathUI;
    [SerializeField]
    private GameObject buybackText;
    [SerializeField]
    private GameObject buybackButton;
    [SerializeField]
    private InventoryMethods _inventoryMethods;
    [SerializeField]
    private InputManager _inputManager;
    private bool dead;
    private bool canBuyback;
    private int buybackCost;

    public float Health { get; }
    public float MaxHealth { get; }

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 100;
        health = 100;
        healthBar = GetComponent<HealthBar>();
        healthBar.SetMaxHealth(maxHealth);
        healthBar.UpdateHealth(health);
        camera = Camera.main;
        dead = false;
    }

    private void Update()
    {
        //Make health bar face player
        //healthBarObject.transform.rotation = new Quaternion(camera.transform.rotation.x, camera.transform.rotation.y, camera.transform.rotation.z, camera.transform.rotation.w);
        UpdateHealth();
    }

    /// <summary>
    /// Behavior for every second - health regen, update healthbar, etc
    /// </summary>
    private void UpdateHealth()
    {
        AddHealth(shipUpgradeScript.masterUpgrade[StatusType.Regeneration] * Time.deltaTime);

        //if the player's max health changed, then add health according to the change
        //in other words, adding max health adds the same amount of current health
        float lastMaxHealth = maxHealth;
        maxHealth = 100 + shipUpgradeScript.masterUpgrade[StatusType.MaxHealth];
        if(lastMaxHealth != maxHealth)
        {
            AddHealth(maxHealth - lastMaxHealth);
            healthBar.SetMaxHealth(maxHealth);
        }

        if(health > maxHealth)
        {
            health = maxHealth;
            healthBar.UpdateHealth(health);
        }
    }

    /// <summary>
    /// Player adds health, up to their max health
    /// </summary>
    /// <param name="health">The amount of health to add</param>
    public void AddHealth(float health)
    {
        this.health += health;
        healthBar.UpdateHealth(this.health);
    }

    /// <summary>
    /// Player takes damage, if health is 0 they die
    /// </summary>
    /// <param name="damage">Amount of damage taken</param>
    public void TakeDamage(float damage)
    {
        if(damage < 0)
        {
            AddHealth(-damage);
        } else
        {
            health -= damage * (100f / (shipUpgradeScript.masterUpgrade[StatusType.Armor] + 100f));
            healthBar.UpdateHealth(health);
            if (health <= 0 && !dead)
            {
                health = 0;
                Die();
            }
        }
        
    }

    public void Die()
    {
        _deathUI.SetActive(true);
        dead = true;
        gameObject.GetComponent<StatusEffects>().ClearStatuses();

        buybackCost = (int)(maxHealth * 2);
        buybackText.GetComponent<TMPro.TextMeshProUGUI>().text = "" + buybackCost;
        if (buybackCost > PlayerInventory.Instance.TotalGold)
        {
            buybackButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
            canBuyback = false;
        } else
        {
            buybackButton.GetComponent<Image>().color = new Color(1,1,1,1);
            canBuyback = true;
        }
        _inventoryMethods.PauseGame();
    }

    public void Respawn(bool inPlace)
    {
        if(inPlace)
        {
            if(!canBuyback)
            {
                //not enough money to buyback
                return;
            }
        }
        if(dead)
        {
            health = maxHealth;
            _inputManager.ResetMovement();
            if(PortManager.LastPortVisited && !inPlace)
            {
                transform.position = PortManager.LastPortVisited.transform.position + new Vector3(5, 0, 5);
                transform.LookAt(PortManager.LastPortVisited.transform);
                transform.rotation.SetEulerAngles(0, transform.rotation.eulerAngles.y, 0);
                transform.Rotate(0, 180, 0);
            }
            _deathUI.SetActive(false);
            if(inPlace)
            {
                PlayerInventory.Instance.TotalGold -= buybackCost;
            }
            _inventoryMethods.UnpauseGame();
        }
    }
}
