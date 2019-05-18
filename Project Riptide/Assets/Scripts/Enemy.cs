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
    private Vector3 startPos;
    private float wanderRadius;
    private float specialTimer;
    private float specialCooldown;
    private bool inSpecial;
    private bool playerCollision;
    private bool obsticalCollision;

    public int Health { get { return health; } }

    // Start is called before the first frame update
    void Start()
    {
        state = EnemyState.Passive;
        playerDistance = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        speed = 1.0f;
        timeBetween = 5.0;
        timeCurrent = timeBetween;
        startPos = transform.position;
        wanderRadius = 30.0f;
        hostileRadius = 10.0f;
        passiveRadius = 50.0f;
        specialTimer = 0.0f;
        specialCooldown = 0.0f;
        inSpecial = false;
        playerCollision = false;
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
                    state = EnemyState.Hostile;
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

        playerCollision = false;
        obsticalCollision = false;
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
        //If time between movements is over select a new destination
        if(timeCurrent >= timeBetween)
        {
            //Select new destination that is inside wander radius
            do
            {
                destination = new Vector3(transform.position.x + Random.Range(-10, 10), transform.position.y, transform.position.z + Random.Range(-10, 10));
            } while (Vector3.Distance(destination, startPos) > wanderRadius);
            timeCurrent = 0;
        }

        //Find the direction the monster should be looking
        lookRotation = Quaternion.LookRotation(destination - transform.position);
        //Increment time
        timeCurrent += Time.deltaTime;
        //Find local forward vector
        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
        CheckCollision();
        //Rotate and move monster
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 0.4f);
        transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 40);
    }

    /// <summary>
    /// Moves the monster based on an AI (hostile)
    /// </summary>
    public void HostileMovement()
    {
        if (!inSpecial)
        {
            //Track player
            destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
            //Find the direction the monster should be looking
            lookRotation = Quaternion.LookRotation(destination - transform.position);
            //Find local forward vector
            Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            //When monster gets close circle player
            if (!CheckCollision() && playerDistance < 5.0f)
            {
                lookRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, transform.forward));
            }

            //Cooldown special while in a 10 units of player
            if(playerDistance < 10.0f)
            {
                specialCooldown -= Time.deltaTime;
            }
            //If cooldown is finished, switch to special
            if(specialCooldown <= 0)
            {
                inSpecial = true;
            }

            //Rotate and move monster
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 0.4f);
            transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 40);
        }
        else
        {
            //Monster stays still and charges for 2 seconds
            if(specialTimer < 2.0f)
            {
                //Track player
                destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
                //Find the direction the monster should be looking
                lookRotation = Quaternion.LookRotation(destination - transform.position);
                //Find local forward vector
                Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                specialTimer += Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 1.0f);
            }
            else if(specialTimer < 3.0f)
            {
                //Find local forward vector
                Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                CheckCollision();
                //If monster hits player, stop special
                if (playerCollision || obsticalCollision)
                    specialTimer = 5.0f;
                specialTimer += Time.deltaTime;
                transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 2);
            }
            else if( specialTimer >= 5.0f && specialTimer < 5.3f)
            {
                //Find local forward vector
                Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                specialTimer += Time.deltaTime;
                transform.Translate(new Vector3(-forward.x, 0, -forward.z) * speed / 4);
            }
            else
            {
                inSpecial = false;
                specialTimer = 0.0f;
                specialCooldown = 5.0f;
            }
            
        }
    }

    /// <summary>
    /// Checks monster collisions with surrounding obsticals
    /// </summary>
    public bool CheckCollision()
    {
        //Create rays for hit detection
        RaycastHit hit;
        Ray rightFRay = new Ray(transform.position + transform.right * 1.2f, new Vector3(transform.forward.x, 0, transform.forward.z) * speed * 6);
        Ray leftFRay = new Ray(transform.position - transform.right * 1.2f, new Vector3(transform.forward.x, 0, transform.forward.z) * speed * 6);
        Ray rightSRay = new Ray(transform.position + transform.right * 1.2f, new Vector3(transform.right.x, 0, transform.right.z) * speed);
        Ray leftSRay = new Ray(transform.position - transform.right * 1.2f, new Vector3(-transform.right.x, 0, -transform.right.z) * speed);
        Debug.DrawRay(rightFRay.origin, rightFRay.direction, Color.black);
        Debug.DrawRay(leftFRay.origin, leftFRay.direction, Color.black);
        Debug.DrawRay(rightSRay.origin, rightSRay.direction, Color.black);
        Debug.DrawRay(leftSRay.origin, leftSRay.direction, Color.black);
        //Check for collision and change rotation accodingly 
        if (Physics.Raycast(leftFRay, out hit, 5.0f) || Physics.Raycast(leftSRay, out hit, 1.0f))
        {
            if (hit.collider.CompareTag("Obstical"))
            {
                lookRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, transform.forward));
                return true;
            }
        }
        else if (Physics.Raycast(rightFRay, out hit, 5.0f) || Physics.Raycast(rightSRay, out hit, 1.0f))
        {
            if (hit.collider.CompareTag("Obstical"))
            {
                lookRotation = Quaternion.LookRotation(Vector3.Cross(transform.forward, Vector3.up));
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Check for collision with player
    /// </summary>
    /// <param name="col">Collision detected</param>
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
            playerCollision = true;
        if (col.gameObject.tag == "Obstical")
            obsticalCollision = true;
    }

}
