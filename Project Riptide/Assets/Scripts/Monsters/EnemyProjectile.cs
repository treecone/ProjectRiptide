using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementPattern { Forward }

public class EnemyProjectile : MonoBehaviour
{
    //Fields
    public GameObject hitbox;

    private Vector3 velocity;
    private int damage;
    private float speed;
    private float currLifeSpan;
    private float maxLifeSpan;
    private Vector2 launchAngle;
    private float launchStrength;
    private MovementPattern movementPattern;
    private GameObject projHitbox;

    // Start is called before the first frame update
    void Start()
    {
        projHitbox = Instantiate(hitbox, transform);
        projHitbox.GetComponent<Hitbox>().SetHitbox(gameObject, transform.position, new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z), HitboxType.EnemyHitbox, damage, launchAngle, launchStrength);
        projHitbox.GetComponent<Hitbox>().OnTrigger += DestroyProj;
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
    /// Loads projecitle
    /// </summary>
    /// <param name="velocity">Direction of projectile</param>
    /// <param name="speed">Speed of projectile</param>
    /// <param name="damage">Damage projectile inflicts</param>
    /// <param name="maxLifeSpan">Max life span of projectile</param>
    /// <param name="launchAngle">Knockback angle for player</param>
    /// <param name="launchStrength">Knockback strength</param>
    public void LoadProjectile(Vector3 velocity, float speed, int damage, float maxLifeSpan, MovementPattern movementPattern, Vector2 launchAngle, float launchStrength)
    {
        this.velocity = velocity;
        this.speed = speed;
        this.damage = damage;
        this.maxLifeSpan = maxLifeSpan;
        this.movementPattern = movementPattern;
        this.launchAngle = launchAngle;
        this.launchStrength = launchStrength;
    }

    /// <summary>
    /// Moves projectile in a straight line
    /// </summary>
    private void MoveProjectileForward()
    {
        transform.Translate(velocity.normalized * speed);
    }

    //Destroy projectile upon hitbox activation
    void DestroyProj(GameObject hit)
    {
        if(hit.tag == "Obstical")
            Destroy(gameObject);
        if (hit.tag == "Player")
            Destroy(gameObject);
    }
}
