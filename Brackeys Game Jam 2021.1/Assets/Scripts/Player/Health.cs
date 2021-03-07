using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public bool movementLocked;
    public GameObject Player;
    private Rigidbody2D rb;
    private Animator anim;

    public int health;
    public GameObject hitParticle;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    
    public IEnumerator Hit(int damage, float knockbackStrength, float knockbackTime, Vector2 enemyPosition)
    {

        movementLocked = true;
        health -= damage;

        anim.SetTrigger("Hurt");
        Vector2 knockbackDirection = new Vector2(enemyPosition.x - transform.position.x, enemyPosition.y - transform.position.y).normalized * -1;
        rb.velocity = knockbackDirection * knockbackStrength;

        if (health <= 0)
        {
            if (gameObject.tag == "Minion")
            {
                if (gameObject.GetComponent<MinionController>().type == MinionController.minionType.Level1)
                {
                    Player.GetComponent<PlayerController>().level1Minions -= 1;
                }   else if (gameObject.GetComponent<MinionController>().type == MinionController.minionType.Level2)
                {
                    Player.GetComponent<PlayerController>().level2Minions -= 1;
                }
            }
            Instantiate(hitParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        yield return new WaitForSeconds(knockbackTime);

        movementLocked = false;
    }
}
