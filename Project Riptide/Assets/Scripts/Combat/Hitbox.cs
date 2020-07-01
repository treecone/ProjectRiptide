using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitboxType { PlayerHitbox, EnemyHitbox, PlayerHurtbox, EnemyHurtbox};
public delegate void HitboxEnter(GameObject hit);
public delegate void HitboxDestroy();

public class Hitbox : MonoBehaviour
{
    public event HitboxEnter OnTrigger;
    public event HitboxEnter OnStay;
    public event HitboxEnter OnExit;
    public event HitboxDestroy OnDestruction;

    [SerializeField]
    private HitboxType _type;
    [SerializeField]
    private float _damage;
    [SerializeField]
    private GameObject _attachedObject;
    [SerializeField]
    private Vector2 _launchAngle;
    [SerializeField]
    private float _launchStrength;

    public GameObject AttachedObject => _attachedObject;
    public float Damage => _damage;
    public HitboxType Type => _type;

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
        _attachedObject = attached;
        transform.localPosition = position;
        transform.localScale = size;
        this._type = type;
        this._damage = damage;
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
        _attachedObject = attached;
        transform.localPosition = position;
        transform.localScale = size;
        _type = type;
        _damage = damage;
        _launchAngle = launchAngle;
        _launchStrength = launchStrength;
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
                if (_type == HitboxType.EnemyHitbox)
                {
                    //Player takes damage
                    hitbox.AttachedObject.GetComponent<PlayerHealth>().TakeDamage(_damage * hitbox._damage);
                    //Add knockback if there is any
                    if (_launchStrength != 0)
                    {
                        Vector3 knockback = Quaternion.Euler(0, _launchAngle.x, -_launchAngle.y) * transform.forward * _launchStrength * (60 * Time.deltaTime);
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
                if (_type == HitboxType.PlayerHitbox)
                {
                    //Add knockback if there is any
                    if (_launchStrength != 0)
                    {
                        Vector3 knockback = Quaternion.Euler(0, _launchAngle.x, -_launchAngle.y) * transform.forward * _launchStrength * (60 * Time.deltaTime);
                        hitbox.AttachedObject.GetComponent<Enemy>().TakeKnockback(knockback);
                    }
                    //Calculate damage for enemies
                    hitbox.AttachedObject.GetComponent<Enemy>().TakeDamage(_damage * hitbox._damage);
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
    /// Called when collider exits hitbox
    /// </summary>
    /// <param name="other">Other collider</param>
    private void OnTriggerExit(Collider other)
    {
        OnExit?.Invoke(other.gameObject);
    }

    /// <summary>
    /// Called when hitbox is destroyed
    /// </summary>
    private void OnDestroy()
    {
        OnDestruction?.Invoke();
    }

    /// <summary>
    /// Draw hitbox position and launch angle with gizmos
    /// </summary>
    public void OnDrawGizmos()
    {
        //Draw launch angle
        if (_launchStrength != 0)
        {
            Vector3 knockback = Quaternion.Euler(0, _launchAngle.x, -_launchAngle.y) * transform.forward * _launchStrength / 100;
            Debug.DrawLine(transform.position, transform.position + knockback, Color.red);
        }

        //Pick color based on hitbox types
        if (_type == HitboxType.EnemyHitbox || _type == HitboxType.PlayerHitbox)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;

        //Draw wire cube where hitbox is
        Gizmos.DrawWireMesh(GetComponent<MeshFilter>().sharedMesh, transform.position, transform.rotation, transform.lossyScale);
    }
}
