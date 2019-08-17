using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementPattern { Forward }

public class EnemyProjectile : MonoBehaviour
{
    //Fields
    private Vector3 velocity;
    private int damage;
    private float speed;
    private float currLifeSpan;
    private float maxLifeSpan;
    private MovementPattern movementPattern;

    // Start is called before the first frame update
    void Start()
    {
        currLifeSpan = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Increment lifespan
        currLifeSpan += Time.deltaTime;

        if(currLifeSpan >= maxLifeSpan)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            //Move projectile based on its movement pattern
            switch(movementPattern)
            {
                case MovementPattern.Forward:
                    MoveProjectileForward();
                    break;
            }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        //If projectile collides with an obstical destory it 
        if (collision.gameObject.tag == "Player")
        {
            //Deal damage to the player based on
            //Projectile's damage
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            GameObject.Destroy(gameObject);
        }
        else
            GameObject.Destroy(gameObject);
    }

    /// <summary>
    /// Loads projecitle
    /// </summary>
    /// <param name="velocity">Direction of projectile</param>
    /// <param name="speed">Speed of projectile</param>
    /// <param name="damage">Damage projectile inflicts</param>
    /// <param name="maxLifeSpan">Max life span of projectile</param>
    public void LoadProjectile(Vector3 velocity, float speed, int damage, float maxLifeSpan, MovementPattern movementPattern)
    {
        this.velocity = velocity;
        this.speed = speed;
        this.damage = damage;
        this.maxLifeSpan = maxLifeSpan;
        this.movementPattern = movementPattern;
    }

    /// <summary>
    /// Moves projectile in a straight line
    /// </summary>
    private void MoveProjectileForward()
    {
        transform.Translate(velocity.normalized * speed);
    }
}
