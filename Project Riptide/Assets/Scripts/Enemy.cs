using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Caden Messenger, Mira Antolovich
/// 4/6/2019
/// </summary>
enum EnemyState { Passive, Hostile }
public delegate void AI();


public partial class Enemy : MonoBehaviour
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
    private float[] specialTimer;
    private float[] specialCooldown;
    private bool[] inSpecial;
    private bool playerCollision;
    private bool obsticalCollision;
    AI HostileAI;
    AI PassiveAI;

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
        specialTimer = new float[1] { 0.0f };
        specialCooldown = new float[1] { 5.0f };
        inSpecial = new bool[1] { false };
        playerCollision = false;
        HostileAI = HostileFollowAndDash;
        PassiveAI = PassiveWanderRadius;
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
                PassiveAI();
                //check for hostile behavior trigger event stuff -> if you get close enough, or shoot it
                if(playerDistance < hostileRadius)
                {
                    state = EnemyState.Hostile;
                }
                break;
            case EnemyState.Hostile:
                HostileAI();
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
