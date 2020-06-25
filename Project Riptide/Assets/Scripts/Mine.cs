using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    private StatusEffects _playerStatusEffects;
    private PlayerHealth _playerHealth;

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
        _playerStatusEffects = _player.GetComponent<StatusEffects>();
        _playerHealth = _player.GetComponent<PlayerHealth>();
        _hitbox = GetComponentInChildren<Hitbox>();
        _hitbox.OnTrigger += Explode;
        
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(GameObject player, float damage, StatusType statusType, float statusDuration, float statusLevel)
    {
        this._player = player;
        this.damage = damage;
        this.statusType = statusType;
        this.statusDuration = statusDuration;
        this.statusLevel = statusLevel;
    }
    private void Explode(GameObject obj)
    {
        if(obj.tag == "Player")
        {
            _playerHealth.TakeDamage(damage);
            _playerStatusEffects.AddStatus(statusType, statusDuration, statusLevel);
            Destroy(gameObject);
        }
        
    }
}
