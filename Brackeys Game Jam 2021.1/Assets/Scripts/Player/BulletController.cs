using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    public int attackDamage;
    public float attackKnockback;
    public GameObject bulletParticle;
    // Start is called before the first frame update
    void Start()
    {

        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Instantiate(bulletParticle, transform.position, transform.rotation);
        if (collision.tag == "Enemy")
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<ParticleSystem>().Stop();
            StartCoroutine(collision.GetComponent<EnemyHealth>().OnHit(attackDamage, attackKnockback, transform.position));
            StartCoroutine(destroy());
        }

        if (collision.tag == "Wall")
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<ParticleSystem>().Stop();
            StartCoroutine(destroy());
        }
    }

    IEnumerator destroy()
    {
        rb.velocity = Vector2.zero;
        Destroy(transform.GetChild(0));
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
