using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Caden Messenger, Mira Antolovich
/// 4/6/2019
/// </summary>
enum EnemyState { Passive, Hostile }
public delegate void AI();
public enum EnemyType { FirstEnemy, KoiBoss }

public partial class Enemy : MonoBehaviour
{
    //public fields
    public EnemyType enemyType;
    public GameObject projectile;
    public GameObject shadow;
    public GameObject healthBarObject;
    public Camera camera;

    private HealthBar healthBar;

    //fields
    public float health;
    private float maxHealth;
    private EnemyState state;
    //player's distance from enemy
    private float playerDistance;
    //monsters distance from start position
    private float enemyDistance;
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
    private float maxRadius;
    private float passiveCooldown;
    private float[] specialTimer;
    private float[] specialCooldown;
    private bool[] inSpecial;
    private bool playerCollision;
    private bool obsticalCollision;
    private bool isRaming;
    private int ramingDamage;
    private AI HostileAI;
    private AI PassiveAI;

    //Fields for collision detection
    public float lengthMult;
    public float widthMult;
    public float heightMult;

    public float Health { get { return health; } }

    // Start is called before the first frame update
    void Start()
    {
        state = EnemyState.Passive;
        playerDistance = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        healthBar = GetComponent<HealthBar>();
        LoadEnemy(enemyType);
    }

    // Update is called once per frame
    void Update()
    {
        //updates player position
        playerDistance = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        enemyDistance = Vector3.Distance(startPos, transform.position);

        //checks for states
        switch (state)
        {
            case EnemyState.Passive:
                PassiveAI();
                //check for hostile behavior trigger event stuff -> if you get close enough, or shoot it
                //also make sure enemy is not in a passive cooldown
                if(playerDistance < hostileRadius && passiveCooldown <= 0)
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

        //Make health bar face player
        healthBarObject.transform.rotation = new Quaternion(camera.transform.rotation.x, camera.transform.rotation.y, camera.transform.rotation.z, healthBarObject.transform.rotation.w);

        if (passiveCooldown > 0)
            passiveCooldown -= Time.deltaTime;

        playerCollision = false;
        obsticalCollision = false;
    }

    private void LoadEnemy(EnemyType type)
    {
        switch(type)
        {
            case EnemyType.FirstEnemy:
                speed = 1.0f;
                health = 20;
                maxHealth = 20;
                timeBetween = 5.0;
                timeCurrent = timeBetween;
                startPos = transform.position;
                wanderRadius = 30.0f;
                hostileRadius = 10.0f;
                passiveRadius = 50.0f;
                maxRadius = 120.0f;
                specialTimer = new float[1] { 0.0f };
                specialCooldown = new float[1] { 5.0f };
                inSpecial = new bool[1] { false };
                playerCollision = false;
                isRaming = false;
                ramingDamage = 15;
                HostileAI = HostileFollowAndDash;
                PassiveAI = PassiveWanderRadius;
                break;
            case EnemyType.KoiBoss:
                speed = 1.4f;
                health = 100;
                maxHealth = 100;
                timeBetween = 5.0;
                timeCurrent = timeBetween;
                startPos = transform.position;
                wanderRadius = 45.0f;
                hostileRadius = 15.0f;
                passiveRadius = 60.0f;
                maxRadius = 240.0f;
                specialTimer = new float[6] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
                specialCooldown = new float[6] { 5.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
                inSpecial = new bool[6] { false, false, false, false, false, false };
                playerCollision = false;
                isRaming = false;
                ramingDamage = 20;
                HostileAI = KoiBossHostile;
                PassiveAI = PassiveWanderRadius;
                break;
        }

        //Setup health bar
        healthBar.SetMaxHealth(maxHealth);
    }

    

    /// <summary>
    /// Checks monster collisions with surrounding obsticals
    /// </summary>
    public bool CheckCollision()
    {
        //Create rays for hit detection
        RaycastHit hit;
        Ray rightFRay = new Ray(new Vector3(transform.position.x, transform.position.y + heightMult, transform.position.z) + (transform.right * widthMult), new Vector3(transform.forward.x, 0, transform.forward.z) * lengthMult);
        Ray leftFRay = new Ray(new Vector3(transform.position.x, transform.position.y + heightMult, transform.position.z) - (transform.right * widthMult), new Vector3(transform.forward.x, 0, transform.forward.z) * lengthMult);
        Ray rightSRay = new Ray(new Vector3(transform.position.x, transform.position.y + heightMult, transform.position.z) + (transform.right * widthMult), new Vector3(transform.right.x, 0, transform.right.z) * lengthMult / 6);
        Ray leftSRay = new Ray(new Vector3(transform.position.x, transform.position.y + heightMult, transform.position.z) - (transform.right * widthMult), new Vector3(-transform.right.x, 0, -transform.right.z) * lengthMult / 6);
        Debug.DrawRay(rightFRay.origin, rightFRay.direction * lengthMult, Color.black);
        Debug.DrawRay(leftFRay.origin, leftFRay.direction * lengthMult, Color.black);
        Debug.DrawRay(rightSRay.origin, rightSRay.direction * lengthMult / 6, Color.black);
        Debug.DrawRay(leftSRay.origin, leftSRay.direction * lengthMult / 6, Color.black);
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
    /// Resets enemy's hostile AI values
    /// </summary>
    public void ResetHostile()
    {
        for(int i = 0; i > inSpecial.Length; i++)
        {
            inSpecial[i] = false;
            specialCooldown[i] = 0.0f;
            specialTimer[i] = 0.0f;
        }
    }

    /// <summary>
    /// Check for collision with player
    /// </summary>
    /// <param name="col">Collision detected</param>
    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerCollision = true;
            if(isRaming)
            {
                //Deal damage to the player
                isRaming = false;
            }
        }
        if (col.gameObject.tag == "Obstical")
            obsticalCollision = true;
    }

    /// <summary>
    /// Spawns an enemy projectile
    /// </summary>
    /// <param name="position">Position relative to enemy</param>
    /// <param name="speed">Speed of projectile</param>
    /// <param name="damage">Damage projectile inflicts</param>
    /// <param name="maxLifeSpan">Max life span of projectile</param>
    /// <param name="movementPattern">Movement pattern of projectile</param>
    private void SpawnProjectile(Vector3 position, float speed, int damage, float maxLifeSpan, MovementPattern movementPattern)
    {
        GameObject.Instantiate(projectile,
            transform.position + transform.TransformVector(position),
            new Quaternion())
            .GetComponent<EnemyProjectile>().LoadProjectile(
            transform.TransformVector(position),
            speed,
            damage,
            maxLifeSpan,
            movementPattern);
    }
}
