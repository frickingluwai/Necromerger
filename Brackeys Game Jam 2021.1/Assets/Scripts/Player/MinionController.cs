using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour
{
    public enum minionType {Level1, Level2, Level3, Level4 };

    [Header("References")]
    private Rigidbody2D rb;
    private GameObject Player;
    private SpriteRenderer sr;
    private Animator anim;
    public Vector2 target;
    public float spawnTime;
    public bool wasEnemy;
    public GameObject gameController;


    [Header("General Settings")]
    public minionType type;
    public Vector2 mainVelocity;
    public bool merge;
    private bool merging;
    public bool active;
    public Animator selectDisplay;
    public Vector2 currentDirection;
    public bool selected;


    [Header("Movement Settings")]
    public float movementSpeed;
    public float targetMaxDistance;


    [Header("Attack Settings")]
    public GameObject enemyTarget;
    public GameObject attackBox;
    public float attackBoxDistance;
    public bool attacking;
    public int attackDamage;
    public float attackKnockback;
    public float attackSpeed;
    public float attackDistance;
    public float attackDetectionDistance;
    public LayerMask enemyLayer;

    public float attackLungeSpeed;
    public float attackLungeTime;

    [Header("Repel Settings")]
    public float repelDistance;
    public float repelStrength;
    public LayerMask minionLayer;
    // Start is called before the first frame update
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("Player");
        sr = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animator>();
        gameController = GameObject.FindGameObjectWithTag("thing");
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        sr.enabled = false;
        yield return new WaitForSeconds(spawnTime);
        sr.enabled = true;
        active = true;
    }

    void weaponControl(){
        
        attackBox.transform.position =
            new Vector2(transform.position.x + (currentDirection.x * attackBoxDistance),
                        transform.position.y + (currentDirection.y * attackBoxDistance));

        attackBox.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(currentDirection.y, currentDirection.x) * 60);
    }

    // Update is called once per frame
    void Update()
    {   
        weaponControl();
        if (!gameController.GetComponent<GameController>().selectedMinions.Contains(gameObject.GetComponent<MinionController>()) || !Input.GetMouseButton(0)){
            target = Player.transform.position;
        }
        if (active )
        {
            Main();
            Animators();
        }   else if (!wasEnemy)
        {
            transform.position = Player.GetComponent<PlayerController>().weapon.transform.position;
        }
        selectDisplay.SetBool("Selected",selected);

    }

    void Animators()
    {

        sr.sortingOrder = (int) -transform.position.y;
        if (!attacking)
        {
            if (Mathf.Abs(rb.velocity.x) > 0.25 || Mathf.Abs(rb.velocity.y) > 0.25)
            {
                anim.SetBool("IsRunning", true);

                
            }   else
            {
                anim.SetBool("IsRunning", false);
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
        }
    }

    // Main function
    void Main()
    {
        if (!merge && !attacking)
        {
            Attack();

            Repel();
        }
        else if (merge)
        {
            Merge();
        }
        rb.velocity = mainVelocity;
    }
    void Attack()
    {
        // Attacking  

        if (enemyTarget != null){
            target = enemyTarget.transform.position;

            // If not in renge of the enemy, follow enemy
            if (Vector2.Distance(transform.position, enemyTarget.transform.position) > attackDistance)
            {
                Vector2 direction = new Vector2(enemyTarget.transform.position.x - transform.position.x, enemyTarget.transform.position.y - transform.position.y).normalized;

                mainVelocity = movementSpeed * direction;
            }
            // If in range, attack
            else
            {
                StartCoroutine(Attacking());
            }  
        }   
        // If a target is not currently selected
        else{
            
            FollowTarget();
            
            // Find nearby enemies and if there is a neaby enemy, set the current enemy to it
            Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(gameObject.GetComponent<BoxCollider2D>().bounds.center, attackDetectionDistance, enemyLayer);
            if (nearbyEnemies.Length > 0)
            {
              GameObject closestEnemy = nearbyEnemies[0].gameObject;
             foreach (Collider2D enemy in nearbyEnemies)
              {
                 closestEnemy = enemy.gameObject;
                }
               enemyTarget = closestEnemy;
            } 
        }

        
    }

    IEnumerator Attacking()
    {
        attacking = true;


        Vector2 direction = new Vector2(enemyTarget.transform.position.x - transform.position.x, enemyTarget.transform.position.y - transform.position.y).normalized;

        mainVelocity = direction * attackLungeSpeed;

        currentDirection = direction;

        attackBox.GetComponent<Animator>().SetTrigger("Attack");

        yield return new WaitForSeconds(attackLungeTime);

        mainVelocity = Vector2.zero;

        if (enemyTarget != null)
        {
            if (Vector2.Distance(transform.position, enemyTarget.transform.position) < attackDistance && enemyTarget != null)
            {
                StartCoroutine(enemyTarget.GetComponent<EnemyHealth>().OnHit(attackDamage, attackKnockback, transform.position));
            }
        }
        yield return new WaitForSeconds(attackSpeed);
        attacking = false;
    }

    void FollowTarget()
    {
        Vector2 direction = new Vector2(target.x - transform.position.x, target.y - transform.position.y).normalized;

        currentDirection = direction;

        mainVelocity = movementSpeed * direction;
    }

    void Merge()
    {
        if (!merging)
        {
            StartCoroutine(Merging());
        }
        Vector2 weaponPosition = Player.GetComponent<PlayerController>().weapon.transform.position;
        Vector2 direction = new Vector2(weaponPosition.x - transform.position.x, weaponPosition.y - transform.position.y).normalized;

        mainVelocity = movementSpeed * direction;
    }

    IEnumerator Merging()
    {
        merging = true;
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
    // Repel from other enemies
    void Repel()
    {
        // Repel from player
        Vector2 playerDirection = new Vector2(Player.transform.position.x - transform.position.x, Player.transform.position.y - transform.position.y).normalized;

        if (Vector2.Distance(transform.position, Player.transform.position) != 0 && Vector2.Distance(transform.position, Player.transform.position) < repelDistance)
        {
            Vector2 repelVelocity = new Vector2(-repelStrength * playerDirection.x, -repelStrength * playerDirection.y) / Vector2.Distance(transform.position, Player.transform.position);
            mainVelocity = new Vector2(mainVelocity.x + repelVelocity.x, mainVelocity.y + repelVelocity.y);
        }


        // Repel from nearby enemies

        // Get list of nearby enemies
        Collider2D[] nearbyMinions = Physics2D.OverlapCircleAll(gameObject.GetComponent<BoxCollider2D>().bounds.center, repelDistance, minionLayer);
        if (nearbyMinions != null)
        {
            foreach (Collider2D minion in nearbyMinions)
            {
                if (minion.gameObject != gameObject)
                {
                    // Apply repel force
                    Transform minionPosition = minion.GetComponent<Transform>();
                    Vector2 direction = new Vector2(minionPosition.position.x - transform.position.x, minionPosition.position.y - transform.position.y).normalized;

                    // Don't divide by zero
                    if (Vector2.Distance(transform.position, minionPosition.position) != 0 && Vector2.Distance(transform.position, minionPosition.position) < repelDistance)
                    {
                        Vector2 repelVelocity = new Vector2(-repelStrength * direction.x, -repelStrength * direction.y) / Vector2.Distance(transform.position, minionPosition.position);
                        mainVelocity = new Vector2(mainVelocity.x + repelVelocity.x, mainVelocity.y + repelVelocity.y);
                    }
                }
            }
        }
    }
}
