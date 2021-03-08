using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private GameObject player;
    private Animator anim;
    private Rigidbody2D rb;
    public int health;
    public bool hit;
    public float knockbackTime;
    public GameObject hitParticle;
    public bool selected;
    public Animator selectDisplay;
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
            Destroy(gameObject);
        }
        yield return new WaitForSeconds(knockbackTime);
        hit = false;
    }

    IEnumerator hitStop()
    {
        yield return new WaitForSeconds(1);
        if (hit)
        {
            hit = false;
        }
    }
}
