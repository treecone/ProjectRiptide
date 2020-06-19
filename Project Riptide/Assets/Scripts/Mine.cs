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
    [SerializeField]
    private float bobAmount;
    private float bobOffset;
    private float startY;

    // Start is called before the first frame update
    void Start()
    {
        _playerStatusEffects = _player.GetComponent<StatusEffects>();
        _playerHealth = _player.GetComponent<PlayerHealth>();
        bobOffset = Random.Range(0, Mathf.PI * 2);
        startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, startY + Mathf.Sin(Time.time + bobOffset) * bobAmount, transform.position.z);
    }

    public void SetData(GameObject player, float damage, string statusType, float statusDuration, float statusLevel)
    {
        this._player = player;
        this.damage = damage;
        this.statusType = statusType;
        this.statusDuration = statusDuration;
        this.statusLevel = statusLevel;
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
