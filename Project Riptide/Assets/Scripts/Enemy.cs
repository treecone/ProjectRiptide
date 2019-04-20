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
    private float speed;
    private Vector3 destination;
    private Quaternion lookRotation;
    private double timeBetween;
    private double timeCurrent;

    public int Health { get { return health; } }

    // Start is called before the first frame update
    void Start()
    {
        state = EnemyState.Passive;
        playerDistance = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        speed = 1.0f;
        timeBetween = 5.0;
        timeCurrent = timeBetween;
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
            destination = new Vector3(transform.position.x + Random.Range(-10, 10), transform.position.y, transform.position.z + Random.Range(-10, 10));
            timeCurrent = 0;
        }

        lookRotation = Quaternion.LookRotation(destination - transform.position);
        timeCurrent += Time.deltaTime;
        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
        RaycastHit hit;
        Ray ray1 = new Ray(transform.position + transform.right, new Vector3(transform.forward.x, 0, transform.forward.z) * speed * 6);
        Ray ray2 = new Ray(transform.position - transform.right , new Vector3(transform.forward.x, 0, transform.forward.z) * speed * 6);
        Debug.DrawRay(ray1.origin, ray1.direction, Color.black);
        Debug.DrawRay(ray2.origin, ray2.direction, Color.black);
        if (Physics.Raycast(ray1, out hit, 10.0f) || Physics.Raycast(ray2, out hit, 10.0f))
        {
            if (hit.collider.CompareTag("Obstical"))
            {
                Debug.Log("Obstical in front");
                lookRotation = Quaternion.LookRotation(Vector3.Cross(forward, Vector3.up));
                if(hit.collider.bounds.Contains(destination))
                {
                    timeCurrent += timeBetween;
                }
            }
        }
        Debug.Log(lookRotation);
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 0.4f);
        transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 40);
    }

    /// <summary>
    /// Moves the monster based on an AI (hostile)
    /// </summary>
    public void HostileMovement()
    {

    }
 
}
