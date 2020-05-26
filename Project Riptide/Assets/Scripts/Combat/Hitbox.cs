using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitboxType { PlayerHitbox, EnemyHitbox, PlayerHurtbox, EnemyHurtbox};
public delegate void HitboxEnter(GameObject hit);

public class Hitbox : MonoBehaviour
{
    public event HitboxEnter OnTrigger;
    public event HitboxEnter OnStay;

    [SerializeField]
    private HitboxType type;
    [SerializeField]
    private float damage;
    [SerializeField]
    private GameObject attachedObject;
    [SerializeField]
    private Vector2 launchAngle;
    [SerializeField]
    private float launchStrength;

    /// <summary>
    /// GameObject Hitbox is attached to
    /// </summary>
    public GameObject AttachedObject
    {
        get { return attachedObject; }
    }

    public float Damage
    {
        get { return damage; }
    }

    public HitboxType Type
    {
        get { return type; }
    }

    /// <summary>
    /// Set the values of the hitbox
    /// </summary>
    /// <param name="attached">GameObject hitbox is attached to</param>
    /// <param name="position">Position of hitbox</param>
    /// <param name="size">Size of hitbox</param>
    /// <param name="type">Type of hitbox</param>
    /// <param name="damage">Damage hitbox deals, or multipler for hurtbox</param>
    public void SetHitbox(GameObject attached, Vector3 position, Vector3 size, HitboxType type, float damage)
    {
        attachedObject = attached;
        transform.position = position;
        transform.localScale = size;
        this.type = type;
        this.damage = damage;
    }

    /// <summary>
    /// Set the values of the hitbox with a launch value
    /// </summary>
    /// <param name="attached">GameObject hitbox is attached to</param>
    /// <param name="position">Position of hitbox</param>
    /// <param name="size">Size of hitbox</param>
    /// <param name="type">Type of hitbox</param>
    /// <param name="damage">Damage hitbox deals, or multipler for hurtbox</param>
    /// <param name="launchAngle">Angle at which hitbox deals knockback</param>
    /// <param name="launchStrength">Strength of knockback</param>
    public void SetHitbox(GameObject attached, Vector3 position, Vector3 size, HitboxType type, float damage, Vector2 launchAngle, float launchStrength)
    {
        attachedObject = attached;
        transform.position = position;
        transform.localScale = size;
        this.type = type;
        this.damage = damage;
        this.launchAngle = launchAngle;
        this.launchStrength = launchStrength;
    }

    /// <summary>
    /// Called when the hitbox enters another collider
    /// </summary>
    /// <param name="other">Collider entered</param>
    public void OnTriggerEnter(Collider other)
    {
        //If the collision was with another hitbox
        if (other.gameObject.CompareTag("Hitbox"))
        {
            Hitbox hitbox = other.gameObject.GetComponent<Hitbox>();
            //If the collision was with a player hurtbox
            if (hitbox.Type == HitboxType.PlayerHurtbox && hitbox.AttachedObject.CompareTag("Player"))
            {
                //Make sure this hitbox is an enemy hitbox
                if (type == HitboxType.EnemyHitbox)
                {
                    //Player takes damage
                    hitbox.AttachedObject.GetComponent<PlayerHealth>().TakeDamage(damage * hitbox.damage);
                    //Add knockback if there is any
                    if (launchStrength != 0)
                    {
                        Vector3 knockback = Quaternion.Euler(0, launchAngle.x, -launchAngle.y) * transform.forward * launchStrength;
                        hitbox.AttachedObject.GetComponent<ShipMovement>().TakeKnockback(knockback);
                    }
                    //Trigger any events assosiated with collision
                    OnTrigger?.Invoke(hitbox.AttachedObject);
                }
            }
            //If the collision was with an enemy hurtbox
            else if (hitbox.Type == HitboxType.EnemyHurtbox && hitbox.AttachedObject.CompareTag("Enemy"))
            {
                //Make sure this hitbox is a player hitbox
                if (type == HitboxType.PlayerHitbox)
                {
                    //Add knockback if there is any
                    if (launchStrength != 0)
                    {
                        Vector3 knockback = Quaternion.Euler(0, launchAngle.x, -launchAngle.y) * transform.forward * launchStrength;
                        hitbox.AttachedObject.GetComponent<Enemy>().TakeKnockback(knockback);
                    }
                    //Calculate damage for enemies
                    hitbox.AttachedObject.GetComponent<Enemy>().TakeDamage(damage * hitbox.damage);
                    //Trigger any events associated with collision
                    OnTrigger?.Invoke(hitbox.AttachedObject);
                }
            }
        }
        else
           //Trigger any events associated with a non-hitbox collision
           OnTrigger?.Invoke(other.gameObject);
    }

    /// <summary>
    /// Called while a hitbox is in another collider
    /// </summary>
    /// <param name="other">Other collider</param>
    public void OnTriggerStay(Collider other)
    {
        //Trigger any events associated with staying collided with another gameobject
        OnStay?.Invoke(other.gameObject);
    }

    /// <summary>
    /// Draw hitbox position and launch angle with gizmos
    /// </summary>
    public void OnDrawGizmos()
    {
        //Draw launch angle
        if (launchStrength != 0)
        {
            Vector3 knockback = Quaternion.Euler(0, launchAngle.x, -launchAngle.y) * transform.forward * launchStrength / 100;
            Debug.DrawLine(transform.position, transform.position + knockback, Color.red);
        }

        //Pick color based on hitbox types
        if (type == HitboxType.EnemyHitbox || type == HitboxType.PlayerHitbox)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;

        //Draw wire cube where hitbox is
        Gizmos.DrawWireMesh(GetComponent<MeshFilter>().sharedMesh, transform.position, transform.rotation, transform.lossyScale);
    }
}
