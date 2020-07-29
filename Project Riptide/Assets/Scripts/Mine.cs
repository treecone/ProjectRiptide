using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField]
    private float damage;
    [SerializeField]
    private StatusType statusType;
    [SerializeField]
    private float statusDuration;
    [SerializeField]
    private float statusLevel;

    private Hitbox _hitbox;
    // Start is called before the first frame update
    void Start()
    {
        _hitbox = GetComponentInChildren<Hitbox>();
        _hitbox.OnTrigger += Explode;
        
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(float damage, StatusType statusType, float statusDuration, float statusLevel)
    {
        this.damage = damage;
        this.statusType = statusType;
        this.statusDuration = statusDuration;
        this.statusLevel = statusLevel;
    }
    private void Explode(GameObject obj)
    {
        if(obj.tag == "Player")
        {
            obj.GetComponent<PlayerHealth>().TakeDamage(damage, false);
            obj.GetComponent<StatusEffects>().AddStatus(statusType, statusDuration, statusLevel);
            Destroy(gameObject);
        }
        
    }
}
