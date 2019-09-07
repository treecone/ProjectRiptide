using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public GameObject healthBarObject;
    public Camera camera;

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
    }

    private void Update()
    {
        //Make health bar face player
        healthBarObject.transform.rotation = new Quaternion(camera.transform.rotation.x, camera.transform.rotation.y, camera.transform.rotation.z, camera.transform.rotation.w);
    }

    /// <summary>
    /// Player takes damage, if health is 0 they die
    /// </summary>
    /// <param name="damage">Amount of damage taken</param>
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.UpdateHealth(health);
        if (health <= 0)
        {
            health = 0;
            //Kill Player
            SceneManager.LoadScene("MonsterScene");
        }
    }
}
