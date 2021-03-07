using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject Player;
    private GameObject attackBox;
    private EnemyHealth health;
    private Animator anim;
    private SpriteRenderer sr;

    [Header("General")]
    public float attackBoxDistance;
    public bool attackingPlayer;
    public bool movementLocked;
    public LayerMask playerLayer;
    public LayerMask minionLayer;
    public Vector2 currentDirection;

    [Header("Attack Settings")]
    public bool attacking;
    public float attackSpeed;
    public float attackTime;
    public float knockbackStrength;
    public float knockbackTime;
    private Vector2 angle = Vector2.one;

    [Header("Attack Speed Settings")]
    public float attackDistance;
    public float attackDetection;
    public int attackDamage;
    public float angleSpeed;
    public float lungeSpeed;
    public float lungeTime;
    public GameObject Target;

    [Header("Repel Settings")]
    public float playerRepelDistance;
    public float playerRepelStrength;

    public LayerMask enemyLayer;
    public float repelDistance;
    public float enemyRepelStrength;

    public Vector2 mainVelocity;

    // Start is called before the first frame update
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("Player");
        health = gameObject.GetComponent<EnemyHealth>();
        anim = gameObject.GetComponent<Animator>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        attackBox = gameObject.transform.GetChild(0).gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        if (Player != null)
        {
        movementLocked = health.hit;
        if (!movementLocked)
        {
            Animations();
            Main();
        }
        }
    }

    void Animations()
    {
        sr.sortingOrder = (int)transform.position.y;
       
        if (!attacking)
        {
            if (Mathf.Abs(rb.velocity.x) > 0.3 || Mathf.Abs(rb.velocity.y) > 0.3)
            {
                anim.SetBool("IsRun", true);


            }
            else
            {
                anim.SetBool("IsRun", false);
            }

            if (Mathf.Abs(currentDirection.x) > Mathf.Abs(currentDirection.y))
            {
                anim.SetInteger("Direction", 3);
                switch (Mathf.Sign(currentDirection.x))
                {
                    case -1:
                        sr.flipX = false;
                        break;
                    case 1:
                        sr.flipX = true;
                        break;
                }
            }
            else
            {
                anim.SetInteger("Direction", (int)Mathf.Sign(currentDirection.y));
            }
        }   else
        {

            anim.SetBool("IsRun", false);
        }
    }

   

    // Attacking player
    void Attack()
    {

        Collider2D[] nearbyMinions = Physics2D.OverlapCircleAll(transform.position, attackDetection, minionLayer);
        if (nearbyMinions.Length == 0)
        {
            Target = Player;
        }   else
        {
            Target = nearbyMinions[0].gameObject;
            foreach (Collider2D minion in nearbyMinions)
            {
                if (Vector2.Distance(transform.position, minion.gameObject.transform.position) < Vector2.Distance(transform.position, Target.transform.position))
                {
                    Target = minion.gameObject;
                }
            }
        }
        // When player in attack range
        if (Vector2.Distance(transform.position, Target.transform.position) < attackDistance && !attacking)
        {
            Vector2 desiredDistance = new Vector2(Target.transform.position.x - transform.position.x, Target.transform.position.y - transform.position.y).normalized;
            StartCoroutine(AttackPlayer(desiredDistance));
        }
        else if (!attacking)
        {
            Vector2 desiredDistance = new Vector2(Target.transform.position.x - transform.position.x, Target.transform.position.y - transform.position.y).normalized;

            // Move towards player
            angle = Vector3.Lerp(angle, desiredDistance, angleSpeed);

            Vector2 movementVelocity = new Vector2(attackSpeed * angle.x, attackSpeed * angle.y);

            // Repel from player
            if (Vector2.Distance(transform.position, Target.transform.position) < playerRepelDistance && Vector2.Distance(transform.position, Target.transform.position) > 0)
            {
                Vector2 repelVelocity = (-desiredDistance * playerRepelStrength) / Vector2.Distance(transform.position, Target.transform.position);

                mainVelocity = new Vector2(movementVelocity.x + repelVelocity.x, movementVelocity.y + repelVelocity.y);
            }
            else
            {
                mainVelocity = movementVelocity;
            }

            // Point attack box towards player 

            currentDirection = desiredDistance;

            attackBox.transform.position =
                new Vector2(transform.position.x + (angle.x * attackBoxDistance),
                            transform.position.y + (angle.y * attackBoxDistance));

            attackBox.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(angle.y, angle.x) * 60);
        }
    }

    IEnumerator AttackPlayer(Vector2 direction)
    {
        BoxCollider2D bc = attackBox.GetComponent<BoxCollider2D>();
        attacking = true;
        yield return new WaitForSeconds(attackTime);

        attackBox.GetComponent<Animator>().SetTrigger("Attack");

        Collider2D[] objectsInRange = Physics2D.OverlapBoxAll(bc.bounds.center, bc.bounds.size, transform.rotation.z);

        int hitEnemies = 0;
        foreach (Collider2D objects in objectsInRange)
        {
            if (hitEnemies <= 2)
            {
                if (objects.tag == "Player")
                {
                    hitEnemies += 1;
                    StartCoroutine(Player.GetComponent<Health>().Hit(1, knockbackStrength, knockbackTime, transform.position));
                }
                else if (objects.tag == "Minion")
                {
                    hitEnemies += 1;
                    StartCoroutine(objects.gameObject.GetComponent<Health>().Hit(attackDamage, knockbackStrength, knockbackTime, transform.position));
                }
            }
        }

        gameObject.GetComponent<AudioSource>().Play();
        mainVelocity = lungeSpeed * direction;

        yield return new WaitForSeconds(lungeTime);

        attacking = false;
    }

    // Repel from other enemies
    void Repel()
    {
        // Repel from nearby enemies    

        // Get list of nearby enemies
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(gameObject.GetComponent<BoxCollider2D>().bounds.center, repelDistance, enemyLayer);
        if (nearbyEnemies != null)
        {
            foreach (Collider2D enemy in nearbyEnemies)
            {
                // Apply repel force
                Transform enemyPosition = enemy.GetComponent<Transform>();
                Vector2 direction = new Vector2(enemyPosition.position.x - transform.position.x, enemyPosition.position.y - transform.position.y).normalized;

                // Don't divide by zero
                if (Vector2.Distance(transform.position, enemyPosition.position) != 0)
                {
                    Vector2 repelVelocity = new Vector2(-enemyRepelStrength * direction.x, -enemyRepelStrength * direction.y) / Vector2.Distance(transform.position, enemyPosition.position);
                    mainVelocity = new Vector2(mainVelocity.x + repelVelocity.x, mainVelocity.y + repelVelocity.y);
                }

            }
        }
    }

    // Main Events
    void Main()
    {
        mainVelocity = Vector2.zero;

        Attack();
        if (!attacking)
        {
            Repel();
        }


        rb.velocity = mainVelocity;
    }
}
