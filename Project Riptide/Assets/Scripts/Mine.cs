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
    private string statusType;
    [SerializeField]
    private float statusDuration;
    [SerializeField]
    private float statusLevel;

    // Start is called before the first frame update
    void Start()
    {
        _playerStatusEffects = _player.GetComponent<StatusEffects>();
        _playerHealth = _player.GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            _playerHealth.TakeDamage(damage);
            _playerStatusEffects.AddStatus(statusType, statusDuration, statusLevel);
            Destroy(gameObject);
        }
    }
}
