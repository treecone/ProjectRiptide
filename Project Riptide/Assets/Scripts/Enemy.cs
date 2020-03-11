using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Caden Messenger, Mira Antolovich
/// 4/6/2019
/// </summary>
enum EnemyState { Passive, Hostile }
public delegate void AI();
public delegate bool MonsterAction(ref float time);
public delegate Vector3 GetVector();
public enum EnemyType { FirstEnemy, KoiBoss, DefensiveEnemy, PassiveEnemy }

public partial class Enemy : PhysicsScript
{
    //public fields
    public EnemyType enemyType;
    public GameObject projectile;
    public GameObject shadow;
    public GameObject healthBarObject;
    public GameObject hitbox;
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
    private Vector3 gravity;
    private float wanderRadius;
    private float maxRadius;
    private float passiveCooldown;
    private float[] specialCooldown;
    private bool[] activeStates;
    private bool playerCollision;
    private bool obsticalCollision;
    private bool isRaming;
    private bool inKnockback = false;
    private float initalPos;
    private float currTime = 0.0f;
    private int ramingDamage;
    private AI HostileAI;
    private AI PassiveAI;
    private List<GameObject> hitboxes;
    private List<GameObject> hurtboxes;
    private Queue<MonsterAction> actionQueue;
    private GetVector PlayerPosition;

    //Fields for collision detection
    public float lengthMult;
    public float widthMult;
    public float heightMult;
    public float baseHeightMult;
    private float halfView = 50.0f;
    private float viewRange = 15.0f;
    private Vector3 widthVector;

    public float Health { get { return health; } }

    // Start is called before the first frame update
    protected override void Start()
    {
        state = EnemyState.Passive;
        playerDistance = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        healthBar = GetComponent<HealthBar>();
        hitboxes = new List<GameObject>();
        actionQueue = new Queue<MonsterAction>();
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<ShipMovementScript>().GetPosition;
        foreach (Hitbox hitbox in GetComponentsInChildren<Hitbox>())
            hitbox.OnTrigger += HitboxTriggered;
        LoadEnemy(enemyType);
        camera = GameObject.FindGameObjectWithTag("MainCamera").transform.GetComponent<Camera>();

        widthVector = new Vector3(widthMult, 0, 0);

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        //updates player position
        playerDistance = Vector3.Distance(transform.position, PlayerPosition());
        enemyDistance = Vector3.Distance(startPos, transform.position);

        //checks for states
        switch (state)
        {
            case EnemyState.Passive:
                PassiveAI();
                //check for hostile behavior trigger event stuff -> if you get close enough, or shoot it
                //also make sure enemy is not in a passive cooldown
                if (playerDistance < hostileRadius && passiveCooldown <= 0)
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

        /*if (Input.GetKeyDown(KeyCode.Space))
            TakeDamage(10);*/

        //Make health bar face player
        healthBarObject.transform.rotation = new Quaternion(camera.transform.rotation.x, camera.transform.rotation.y, camera.transform.rotation.z, camera.transform.rotation.w);

        if (passiveCooldown > 0)
            passiveCooldown -= Time.deltaTime;

        SetHeightMult();
        SetShadowPosition();
        SetHealthBarPosition();

        playerCollision = false;
        obsticalCollision = false;

        base.Update();
    }

    private void LoadEnemy(EnemyType type)
    {
        switch (type)
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
                specialCooldown = new float[1] { 5.0f };
                activeStates = new bool[1] { false };
                playerCollision = false;
                isRaming = false;
                ramingDamage = 15;
                HostileAI = HostileFollowAndDash;
                PassiveAI = PassiveWanderRadius;
                break;
            case EnemyType.KoiBoss:
                speed = 1.0f;
                health = 200;
                maxHealth = 200;
                timeBetween = 5.0;
                timeCurrent = timeBetween;
                startPos = transform.position;
                wanderRadius = 45.0f;
                hostileRadius = 30.0f;
                passiveRadius = 120.0f;
                maxRadius = 240.0f;
                specialCooldown = new float[5] { 5.0f, 0.0f, 0.0f, 0.0f, 0.0f };
                activeStates = new bool[3] { false, false, false };
                playerCollision = false;
                isRaming = false;
                ramingDamage = 20;
                HostileAI = KoiBossHostile;
                PassiveAI = PassiveWanderRadius;
                break;
            case EnemyType.DefensiveEnemy:
                speed = 1.0f;
                health = 30;
                maxHealth = 30;
                timeBetween = 5.0;
                timeCurrent = timeBetween;
                startPos = transform.position;
                wanderRadius = 30.0f;
                hostileRadius = 0.0f;
                passiveRadius = 50.0f;
                maxRadius = 120.0f;
                specialCooldown = new float[1] { 5.0f };
                activeStates = new bool[1] { false };
                playerCollision = false;
                isRaming = false;
                ramingDamage = 15;
                HostileAI = HostileFollowAndDash;
                PassiveAI = PassiveWanderRadius;
                break;
            case EnemyType.PassiveEnemy:
                speed = 1.2f;
                health = 20;
                maxHealth = 20;
                timeBetween = 5.0;
                timeCurrent = timeBetween;
                startPos = transform.position;
                wanderRadius = 30.0f;
                hostileRadius = 10.0f;
                passiveRadius = 30.0f;
                maxRadius = 120.0f;
                specialCooldown = new float[1] { 5.0f };
                activeStates = new bool[1] { false };
                playerCollision = false;
                isRaming = false;
                ramingDamage = 5;
                HostileAI = HostileRunAway;
                PassiveAI = PassiveWanderRadius;
                break;
        }

        //Setup health bar
        healthBar.SetMaxHealth(maxHealth);
    }

    /// <summary>
    /// Monster takes damage, if health is 0 they die
    /// </summary>
    /// <param name="damage">Amount of damage taken</param>
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.UpdateHealth(health);
        if (state == EnemyState.Passive)
            state = EnemyState.Hostile;
        if (health <= 0)
        {
            health = 0;
            //Drop loot
            GameObject lootable = Instantiate(Resources.Load("Inventory/Lootable"), new Vector3(transform.position.x + Random.Range(-2.0f, 2.0f), transform.position.y, transform.position.z + Random.Range(-2.0f, 2.0f)), Quaternion.identity) as GameObject;
            lootable.GetComponent<Lootable>().itemStored = GameObject.FindWithTag("GameManager").GetComponent<ItemDatabase>().FindItem("Carp Scale");
            lootable.GetComponent<Lootable>().lightColor = GameObject.FindWithTag("GameManager").GetComponent<ItemDatabase>().rarityColors[lootable.GetComponent<Lootable>().itemStored.rarity];
            //Kill monster
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Checks monster collisions with surrounding obsticals
    /// </summary>
    public bool CheckCollision()
    {
        //Create rays for hit detection
        RaycastHit hit;
        Ray rightFRay = new Ray(new Vector3(transform.position.x, baseHeightMult, transform.position.z) + (transform.right * widthMult), new Vector3(transform.forward.x, 0, transform.forward.z) * lengthMult);
        Ray leftFRay = new Ray(new Vector3(transform.position.x, baseHeightMult, transform.position.z) - (transform.right * widthMult), new Vector3(transform.forward.x, 0, transform.forward.z) * lengthMult);
        Ray rightSRay = new Ray(new Vector3(transform.position.x, baseHeightMult, transform.position.z) + (transform.right * widthMult), new Vector3(transform.right.x, 0, transform.right.z) * lengthMult / 6);
        Ray leftSRay = new Ray(new Vector3(transform.position.x, baseHeightMult, transform.position.z) - (transform.right * widthMult), new Vector3(-transform.right.x, 0, -transform.right.z) * lengthMult / 6);
        Debug.DrawRay(rightFRay.origin, rightFRay.direction * lengthMult, Color.black);
        Debug.DrawRay(leftFRay.origin, leftFRay.direction * lengthMult, Color.black);
        Debug.DrawRay(rightSRay.origin, rightSRay.direction * lengthMult / 6, Color.black);
        Debug.DrawRay(leftSRay.origin, leftSRay.direction * lengthMult / 6, Color.black);
        //Check for collision and change rotation accodingly 
        if (Physics.Raycast(leftFRay, out hit, 5.0f) || Physics.Raycast(leftSRay, out hit, 1.0f))
        {
            if (hit.collider.CompareTag("Obstical"))
            {
                Debug.Log("Obstical collision");
                //lookRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, transform.forward));
                return true;
            }
        }
        else if (Physics.Raycast(rightFRay, out hit, 5.0f) || Physics.Raycast(rightSRay, out hit, 1.0f))
        {
            if (hit.collider.CompareTag("Obstical"))
            {
                Debug.Log("Obstical collision");
                //lookRotation = Quaternion.LookRotation(Vector3.Cross(transform.forward, Vector3.up));
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
        //reset states
        for (int i = 0; i > activeStates.Length; i++)
        {
            activeStates[i] = false;
        }
        //reset cooldowns
        for (int i = 0; i > specialCooldown.Length; i++)
        {
            specialCooldown[i] = 0.0f;
        }
        isRaming = false;
        inKnockback = false;
        actionQueue.Clear();
        ClearHitboxes();
        currTime = 0;
    }

    /// <summary>
    /// Called when a hitbox is triggered
    /// </summary>
    /// <param name="collision">GameObject that triggered hitbox</param>
    public void HitboxTriggered(GameObject collision)
    {
        if (collision.tag == "Obstical")
        {
            obsticalCollision = true;
            Debug.Log("Obstical Collision");
        }
        if (collision.tag == "Player")
            playerCollision = true;

    }

    /// <summary>
    /// Creates a hitbox as a child of the enemy
    /// </summary>
    /// <param name="position">Position relative to enemy</param>
    /// <param name="scale">Size of hitbox</param>
    /// <param name="type">Type of hitbox</param>
    /// <param name="damage">Damage dealt by hitbox</param>
    /// <returns></returns>
    public GameObject CreateHitbox(Vector3 position, Vector3 scale, HitboxType type, float damage)
    {
        GameObject temp = Instantiate(hitbox, transform);
        temp.GetComponent<Hitbox>().SetHitbox(gameObject, position, scale, type, damage);
        temp.GetComponent<Hitbox>().OnTrigger += HitboxTriggered;
        return temp;
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
            0.75f,
            damage,
            maxLifeSpan,
            movementPattern);
    }

    private void ClearHitboxes()
    {
        for (int i = 0; i < hitboxes.Count; i++)
        {
            GameObject.Destroy(hitboxes[i]);
        }
        hitboxes.Clear();
    }

    /// <summary>
    /// Returns enemy to inital position on Y axis
    /// </summary>
    private void ReturnToInitalPosition()
    {

        position = new Vector3(transform.position.x, initalPos, transform.position.z);
    }

    /// <summary>
    /// Applys a force to move the enemy in an arc
    /// </summary>
    /// <param name="dir">Direction of movement</param>
    /// <param name="dist">Horizontal distance covered</param>
    /// <param name="time">Time that the arc takes place</param>
    /// <param name="gravity">Gravity being applied each frame</param>
    private void ApplyArcForce(Vector3 dir, float dist, float time, Vector3 gravity)
    {
        float xForce = mass * (dist / (time * Time.deltaTime));
        float yForce = (-gravity.y * time) / (2 * Time.deltaTime);
        Vector3 netForce = dir * xForce;
        netForce += yForce * Vector3.up;
        ApplyForce(netForce);
    }

    /// <summary>
    /// Applys a force to move the enemy in an arc
    /// </summary>
    /// <param name="dir">Direction of movement</param>
    /// <param name="dist">Horizontal distance covered</param>
    /// <param name="yMax">Maximum vertical distance</param>
    /// <param name="time">Time that the arc takes place</param>
    /// <returns>Gravity to be applied each frame</returns>
    private Vector3 ApplyArcForce(Vector3 dir, float dist, float yMax, float time)
    {
        float xForce = mass * (dist / (time * Time.deltaTime));
        float gravity = (-8 * mass * yMax) / (time * time);
        float yForce = (-gravity * time) / (2 * Time.deltaTime);
        Vector3 netForce = dir * xForce;
        netForce += yForce * Vector3.up;
        ApplyForce(netForce);
        return Vector3.up * gravity;
    }

    private void ApplyMoveForce(Vector3 dir, float dist, float time)
    {
        float moveForce = mass * (dist / (time * Time.deltaTime));
        Vector3 netForce = dir * moveForce;
        ApplyForce(netForce);
    }

    private void ApplyConstantMoveForce(Vector3 dir, float dist, float time)
    {
        float moveForce = (2 * mass * dist) / (time * time);
        Vector3 netForce = dir * moveForce;
        ApplyForce(netForce);
    }

    private void SetHeightMult()
    {
        heightMult = (transform.worldToLocalMatrix * new Vector3(transform.position.x, baseHeightMult, transform.position.z)).y;
    }

    private void SetShadowPosition()
    {
        shadow.transform.position = new Vector3(transform.position.x, heightMult, transform.position.z);
    }

    private void SetHealthBarPosition()
    {
        healthBarObject.transform.position = new Vector3(transform.position.x, baseHeightMult + 1.5f * transform.localScale.y, transform.position.z);
    }

    public bool CheckObstacle()
    {
        RaycastHit hit = new RaycastHit();
        Vector3 detectPosition = new Vector3(transform.position.x, 4.5f, transform.position.z);
        for (int i = 0; i <= halfView; i += 4)
        {
            Debug.DrawRay(detectPosition, Quaternion.AngleAxis(i, Vector3.up) * transform.forward * viewRange, Color.red);
            Debug.DrawRay(detectPosition, Quaternion.AngleAxis(-i, Vector3.up) * transform.forward * viewRange, Color.red);
            if (Physics.Raycast(detectPosition, Quaternion.AngleAxis(i, Vector3.up) * transform.forward, out hit, viewRange))
            {
                return true;
            }
            if (Physics.Raycast(detectPosition, Quaternion.AngleAxis(-i, Vector3.up) * transform.forward, out hit, viewRange))
            {
                return true;
            }
        }

        return false;
    }

    public Vector3 AvoidObstacle()
    {
        Vector3 dir = Vector3.zero;
        bool found = false;

        Vector3 detectPosition = new Vector3(transform.position.x, 4.0f, transform.position.z);

        for (int i = 0; i <= 90; i += 4)
        {
            if (!Physics.Raycast(detectPosition + transform.TransformDirection(widthVector), Quaternion.AngleAxis(i, Vector3.up) * transform.forward, viewRange))
            {
                if (!Physics.Raycast(detectPosition + transform.TransformDirection(-widthVector), Quaternion.AngleAxis(i, Vector3.up) * transform.forward, viewRange + 2.0f))
                {
                    //dir = Quaternion.AngleAxis(i, Vector3.up) * transform.forward;
                    dir = Quaternion.AngleAxis(90, Vector3.up) * transform.forward;
                    found = true;
                }
            }
            if (!Physics.Raycast(detectPosition + transform.TransformDirection(-widthVector), Quaternion.AngleAxis(-i, Vector3.up) * transform.forward, viewRange))
            {
                if (!Physics.Raycast(detectPosition + transform.TransformDirection(widthVector), Quaternion.AngleAxis(-i, Vector3.up) * transform.forward, viewRange + 2.0f))
                {
                    if (!found || Random.Range(0, 1) == 0)
                    {
                        //dir = Quaternion.AngleAxis(-i, Vector3.up) * transform.forward;
                        dir = Quaternion.AngleAxis(-90, Vector3.up) * transform.forward;
                        found = true;
                    }
                }
            }
            if (found)
                return dir;
        }

        if (Random.Range(0, 1) == 0)
            return Quaternion.AngleAxis(90, Vector3.up) * transform.forward;
        else
            return Quaternion.AngleAxis(-90, Vector3.up) * transform.forward;
    }
}