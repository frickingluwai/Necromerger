using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    // TODO Change minion movement

    [Header("References")]
    private Animator anim;
    private GameObject player;
    public LayerMask enemyLayer;
    public LayerMask minionLayer;

    [Header("Minion Settings")]
    public List<MinionController> selectedMinions;
    public GameObject target;

    [Header("Mouse Settings")]
    public bool clicked;
    public float clickDelayTime;
    public float clickTime;

    // Start is called before the first frame update
    private void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Mouse();
    }
    
    private void Mouse() {
        
        // Mouse references
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D enemyRaycast = Physics2D.Raycast(mousePosition, Vector2.zero, 0,enemyLayer);
        RaycastHit2D minionRaycast = Physics2D.Raycast(mousePosition, Vector2.zero,0, minionLayer);

        
        // If mouse over enemy
        if (enemyRaycast.collider != null){
            if (Input.GetMouseButtonDown(0)){
                // Check if the enemy is already selected
                switch (enemyRaycast.collider.gameObject == target){
                    case true:
                        // Deselecting minion targets from enemy 
                        target = null;
                        foreach(MinionController minion in selectedMinions){
                            minion.enemyTarget = null;
                        }
                        break;
                    case false:
                        // Setting selected minion targets to the enemy
                        target =enemyRaycast.collider.gameObject;
                        foreach(MinionController minion in selectedMinions){
                            minion.enemyTarget = enemyRaycast.collider.gameObject;
                        }
                        break;
                }
            }
        }

        // If mouse over minion
        if (minionRaycast.collider != null){
            // If clicking
            if (Input.GetMouseButtonDown(0)){
                // Check if the minion is already selected
                switch (minionRaycast.collider.gameObject.GetComponent<MinionController>().selected){
                    case true:
                        // Deselect minion  
                        selectedMinions.Remove(minionRaycast.collider.gameObject.GetComponent<MinionController>());
                        minionRaycast.collider.gameObject.GetComponent<MinionController>().selected = false;
                        break;
                    case false:
                        // Select minion
                        if (!selectedMinions.Contains(minionRaycast.collider.gameObject.GetComponent<MinionController>())){

                        }
                        selectedMinions.Add(minionRaycast.collider.gameObject.GetComponent<MinionController>());
                        minionRaycast.collider.gameObject.GetComponent<MinionController>().selected = true;
                        break;
                }
            }
        }

        // If mouse over nothing
        if (minionRaycast.collider == null && enemyRaycast.collider == null){
            if (clicked){
                clickTime += Time.deltaTime;
                
                // Chuck for double click
                if (Input.GetMouseButtonDown(0)){
                    selectedMinions = null;
                    selectedMinions = player.GetComponent<PlayerController>().minions;
                    foreach (MinionController minion in selectedMinions){
                        minion.selected = false;
                    }
                    clickTime = 0;
                    clicked = false;
                }
                else if (clickTime >= clickDelayTime){
                    clickTime = 0;
                    clicked = false;
                }
            }else if (Input.GetMouseButtonDown(0)){
                clicked = true;
            }   

            if (Input.GetMouseButton(0) && !clicked){
                foreach (MinionController minion in selectedMinions){
                    minion.enemyTarget = null;
                    minion.target = mousePosition;
                }
            }
        }
    }
}
