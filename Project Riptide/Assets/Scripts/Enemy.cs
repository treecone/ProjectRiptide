using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Caden Messenger, Mira Antolovich
/// 4/6/2019
/// </summary>
enum EnemyState { Passive, Hostile }

public class Enemy : MonoBehaviour
{
    //fields
    private int health;
    private EnemyState state;
    //player's distance from enemy
    private float playerDistance;
    //Radius to trigger hostile AI
    private float hostileRadius;
    //Radius to trigger passive AI
    private float passiveRadius;
    //Fields for AI
    private Vector3 oldPosition;
    private Vector3 destination;
    private double timeBetween;
    private double timeCurrent;

    public int Health { get { return health; } }

    // Start is called before the first frame update
    void Start()
    {
        state = EnemyState.Passive;
        playerDistance = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        timeBetween = 5.0;
        timeCurrent = 5.0;
    }

    // Update is called once per frame
    void Update()
    {
        //updates player position
        playerDistance = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);

        //checks for states
        switch (state)
        {
            case EnemyState.Passive:
                PassiveMovement();
                //check for hostile behavior trigger event stuff -> if you get close enough, or shoot it
                if(playerDistance < hostileRadius)
                {
                    //state = EnemyState.Hostile;
                }
                break;
            case EnemyState.Hostile:
                HostileMovement();
                //check for passive behavior trigger, if you get far enough away
                if (playerDistance >= passiveRadius)
                {
                    state = EnemyState.Passive;
                }
                break;
        }
    }

    /// <summary>
    /// Monster takes damage, if health is 0 they die
    /// </summary>
    /// <param name="damage">int Amount of damage taken</param>
    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            health = 0;
            //Kill monster
        }
    }

    /// <summary>
    /// Moves the monster based on a AI (passive)
    /// </summary>
    public void PassiveMovement()
    {
        if(timeCurrent >= timeBetween)
        {
            oldPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            destination = new Vector3(transform.position.x + Random.Range(-10, 10), transform.position.y, transform.position.z + Random.Range(-10, 10));
            timeCurrent = 0;
        }

        timeCurrent += Time.deltaTime;
        transform.position = Vector3.Lerp(oldPosition, destination, (float)(timeCurrent / timeBetween));
        Debug.Log(transform.position.x);
        Debug.Log(timeCurrent);
    }

    /// <summary>
    /// Moves the monster based on an AI (hostile)
    /// </summary>
    public void HostileMovement()
    {

    }
 
}
