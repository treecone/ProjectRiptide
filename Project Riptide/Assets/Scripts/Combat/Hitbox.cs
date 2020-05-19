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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHitbox(GameObject attached, Vector3 position, Vector3 size, HitboxType type, float damage)
    {
        attachedObject = attached;
        transform.position = position;
        transform.localScale = size;
        this.type = type;
        this.damage = damage;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hitbox"))
        {
            Hitbox hitbox = other.gameObject.GetComponent<Hitbox>();
            if (hitbox.Type == HitboxType.PlayerHurtbox && hitbox.AttachedObject.CompareTag("Player"))
            {
                if (type == HitboxType.EnemyHitbox)
                {
                    hitbox.AttachedObject.GetComponent<PlayerHealth>().TakeDamage(damage * hitbox.damage);
                    OnTrigger?.Invoke(hitbox.AttachedObject);
                }
            }
            else if (hitbox.Type == HitboxType.EnemyHurtbox && hitbox.AttachedObject.CompareTag("Enemy"))
            {
                if (type == HitboxType.PlayerHitbox)
                {
                    //Calculate damage for enemies
                    hitbox.AttachedObject.GetComponent<Enemy>().TakeDamage(damage * hitbox.damage);
                    OnTrigger?.Invoke(hitbox.AttachedObject);
                }
            }
        }
        else
           OnTrigger?.Invoke(other.gameObject);
    }

    public void OnTriggerStay(Collider other)
    {
        OnStay?.Invoke(other.gameObject);
    }
}
