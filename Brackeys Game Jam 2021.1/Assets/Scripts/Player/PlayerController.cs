using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // TODO Enemy necromancing
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    [Header("General Settings")]
    public GameObject weapon;
    public Animator HealthBar;
    public Animator MagicBar;  
    public float distance;
    public bool attacking;
    public int health;

    [Header("Movement Settings")]
    public float movementSpeed;
    public bool movementLocked;

    [Header("Attack Settings")]
    public float attackSpeed;
    public GameObject projectile;

    [Header("Minion Settings")]
    public int magic;
    public float magicRate;
    public List<MinionController> minions;
    public Vector2 currentTarget;
    public GameObject minionLevel1;
    public GameObject minionLevel2;
    public GameObject minionLevel3;
    public int level1Minions;
    public int level2Minions;
    public int level3Minions;
    public int amountOfMinionsNeededForLevel2;
    public int amountOfMinionsNeededForLevel3;
    


    // Start is called before the first frame update
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animator>();
        StartCoroutine(GeneratingMagic());
    }

    void Animators()
    {
        sr.sortingOrder = (int) -transform.position.y;
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            anim.SetInteger("DirectionFacing", 3);

            switch (Input.GetAxisRaw("Horizontal"))
            {
                case 1:
                    sr.flipX = false;
                    break;
                case -1:
                    sr.flipX = true;
                    break;
                case 0:
                    break;
            }

            anim.SetBool("IsRun", true);
        }   else if (Input.GetAxisRaw("Vertical") != 0)
        {
            anim.SetInteger("DirectionFacing",(int) Input.GetAxisRaw("Vertical") * -1);
        }   else
        {
            anim.SetBool("IsRun", false);
        }


    }
    // Update is called once per frame
    void Update()
    {
        HealthBar.SetInteger("Health", gameObject.GetComponent<Health>().health);

        MagicBar.SetInteger("Magic", magic);

        minionManager();

        movementLocked = gameObject.GetComponent<Health>().movementLocked;
        Animators();
        if (!movementLocked && !attacking)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            PlayerMovement();
            WeaponController();

            if (Input.GetKeyDown(KeyCode.Mouse1) && magic > 0)
            {
                StartCoroutine(Attack());
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                currentTarget = mousePosition;
            }   else
            {
                currentTarget = transform.position;
            }
        }
    }

    IEnumerator GeneratingMagic()
    {
        yield return new WaitForSeconds(magicRate);
        if (magic <= 20)
        {
            magic += 1;
        }
        StartCoroutine(GeneratingMagic());
    }

    void PlayerMovement()
    {
        // Get player input
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Set velocity
        Vector2 movementVelocity = playerInput * movementSpeed;

        rb.velocity = movementVelocity;
    }

    IEnumerator Attack()
    {
        movementLocked = true;
        attacking = true;
        Instantiate(projectile, weapon.transform.position, weapon.transform.rotation);
        anim.SetTrigger("Attack");
        rb.velocity = Vector2.zero;
        gameObject.GetComponent<AudioSource>().Play();
        magic -= 1;
        yield return new WaitForSeconds(attackSpeed);
        movementLocked = false;
        attacking = false;
    }

    void WeaponController()
    {
        // Inputs
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction =
            new Vector2(mousePosition.x - transform.position.x,
                        mousePosition.y - transform.position.y);

        weapon.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(direction.y, direction.x) * 60);

        weapon.transform.position = new Vector2(
            transform.position.x + distance * Mathf.Cos(Mathf.Atan2(direction.y, direction.x)),
            transform.position.y + distance * Mathf.Sin(Mathf.Atan2(direction.y, direction.x))
            );
    }

    public Button spawnButton;
    public Button mergeButton;
    public Button mergeButton2;
    public List<MinionController.minionType> minionList;
    public bool coroutineRunning;
    void minionManager()
    {
        if (magic >= 5)
        {
            spawnButton.interactable = true;
        }   else
        {
            spawnButton.interactable = false;
        }

        if (magic >= 10 && level1Minions >= amountOfMinionsNeededForLevel2)
        {
            mergeButton.interactable = true;
        }
        else
        {
            mergeButton.interactable = false;
        }

        if (magic >= 15 && level2Minions >= amountOfMinionsNeededForLevel3)
        {
            mergeButton2.interactable = true;
        }
        else
        {
            mergeButton2.interactable = false;
        }
        
        if (minions.Count != 0){
            foreach (MinionController minion in minions.ToArray())
            {
                if (minion == null)
                {
                    minions.Remove(minion);
                }
            }
        }

        if (minionList.Count != 0 && !coroutineRunning){
            StartCoroutine(spawn(minionList[0]));
        }
    }
    public IEnumerator spawn(MinionController.minionType spawnMinion){
        coroutineRunning = true;
        // Spawn type 1 minion
        if (spawnMinion == MinionController.minionType.Level1){
            minions.Add(Instantiate(minionLevel1, weapon.transform.position, Quaternion.identity).GetComponent<MinionController>());
            level1Minions += 1;
            gameObject.GetComponent<AudioSource>().Play();
        }
        // Spawn type 2 minion   
        else if (spawnMinion == MinionController.minionType.Level2){
            int enemyCount = 0;
            foreach (MinionController minion in minions.ToArray())
            {
                if (minion.type == MinionController.minionType.Level1)
                {
                    minions.Remove(minion);
                    minion.merge = true;
                    enemyCount += 1;
                }
                if (enemyCount >= amountOfMinionsNeededForLevel2)
                {
                    break;
                }
            }
            
            gameObject.GetComponent<AudioSource>().Play();

            minions.Add(Instantiate(minionLevel2, weapon.transform.position, Quaternion.identity).GetComponent<MinionController>());
            level2Minions += 1;
            level1Minions -= amountOfMinionsNeededForLevel2;
        }   
        // Spawn type 3 minions
        else if (spawnMinion == MinionController.minionType.Level3){
            int enemyCount = 0;
            foreach (MinionController minion in minions.ToArray())
            {
                if (minion.type == MinionController.minionType.Level2)
                {
                    minions.Remove(minion);
                    minion.merge = true;
                    enemyCount += 1;
                }
                if (enemyCount >= amountOfMinionsNeededForLevel3)
                {
                    break;
                }
            }
            
            gameObject.GetComponent<AudioSource>().Play();
            minions.Add(Instantiate(minionLevel3, weapon.transform.position, Quaternion.identity).GetComponent<MinionController>());
            level3Minions += 1;
            level2Minions -= amountOfMinionsNeededForLevel3;
        }
        yield return new WaitForSeconds(1);
        minionList.Remove(0);
        coroutineRunning = false;
    }
    
    public void spawnMinion()
    {
        if (magic >= 5)
        {
            magic -= 5;
            minionList.Add(MinionController.minionType.Level1);
        }
    }

    public void mergeLevel2()
    {
        if (magic >= 10 && level1Minions >= amountOfMinionsNeededForLevel2)
        {
            magic -= 10;
            minionList.Add(MinionController.minionType.Level2);
        }
    }

    public void mergeLevel3()
    {
        if (magic >= 15 && level2Minions >= amountOfMinionsNeededForLevel3)
        {
            magic -= 15;
            minionList.Add(MinionController.minionType.Level3);
        }
    }

}
