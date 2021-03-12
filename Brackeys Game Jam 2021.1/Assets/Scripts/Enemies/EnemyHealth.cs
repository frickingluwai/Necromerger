using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    // TODO Enemy necromancing
    [Header("Health Settings")]
    private GameObject player;
    private Animator anim;
    private Rigidbody2D rb;
    public int health;
    public bool hit;
    public float knockbackTime;
    public GameObject hitParticle;
    public bool dead;

    [Header("Select Settings")]
    public bool selected;
    public Animator selectDisplay;


    [Header("Necromance Settings")]
    public GameObject minionPrefab;

    public float necromanceDistance;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update() {
        selectDisplay.SetBool("Selected", selected);
    }
    // Called when hit by player
    public IEnumerator OnHit(int damage, float knockback, Vector2 direction)
    {
        anim.SetTrigger("Hurt");
        health -= damage;
        hit = true;
        StartCoroutine(hitStop());
        Vector2 knockbackDirection =
            new Vector2(direction.x - transform.position.x,
                        direction.y - transform.position.y).normalized * -1;
        rb.velocity = knockbackDirection * knockback;

        if (health <= 0)
        {
            player.GetComponent<PlayerController>().magic += 2;
            Instantiate(hitParticle, transform.position, Quaternion.identity);
            gameObject.tag = "Minion";
            Destroy(gameObject.GetComponent<MeleeEnemyController>());
            dead = true;
        }
        yield return new WaitForSeconds(knockbackTime);
        hit = false;
    }


    public void WhenDead(){
        if (dead) {
            if (Vector3.Distance(transform.position, player.transform.position) < necromanceDistance){
                return;
            }
        }
    }
    IEnumerator hitStop()
    {
        yield return new WaitForSeconds(1);
        if (hit)
        {
            hit = false;
        }
    }

    private void OnMouseDown() {
        if (dead) {
            if (Vector3.Distance(transform.position, player.transform.position) < necromanceDistance){
                MinionController minion = Instantiate(minionPrefab, transform.position, Quaternion.identity).GetComponent<MinionController>();
                player.GetComponent<PlayerController>().minions.Add(minion);
                minion.selected = false;
                minion.wasEnemy = true;
                Destroy(gameObject);
            }
        }   
        else {
            if (selected){
                selected = false;
            }   else {
                selected = true;
            }
        }   
    }
}
