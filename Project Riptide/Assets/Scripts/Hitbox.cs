using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitboxType { PlayerHitbox, EnemyHitbox, PlayerHurtbox, EnemyHurtbox};
public delegate void HitboxEnter(GameObject hit);

public class Hitbox : MonoBehaviour
{
    public event HitboxEnter OnTrigger;

    private HitboxType type;
    private float damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHitbox(Vector3 position, Vector3 size, HitboxType type, float damage)
    {
        transform.position = position;
        transform.localScale = size;
        this.type = type;
        this.damage = damage;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(type == HitboxType.PlayerHurtbox)
            {
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
                OnTrigger?.Invoke(other.gameObject);
            }
        }
        else if(other.gameObject.CompareTag("Enemy"))
        {
            if(type == HitboxType.EnemyHurtbox)
            {
                other.gameObject.GetComponent<Enemy>().TakeDamage(damage);
                OnTrigger?.Invoke(other.gameObject);
            }
        }
    }
}
