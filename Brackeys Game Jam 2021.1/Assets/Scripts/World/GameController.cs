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
    public bool allMinionsSelected = true;

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
    
    // TODO All of this
    private void Mouse() {
        
        // Mouse references
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D enemyRaycast = Physics2D.Raycast(mousePosition, Vector2.zero, 0,enemyLayer);
        RaycastHit2D minionRaycast = Physics2D.Raycast(mousePosition, Vector2.zero,0, minionLayer);

        // If selected minions becomes null, select all minions
        if (selectedMinions == null || selectedMinions.Count <= 0){
            // Select all minions
            GameObject[] gameobjects = GameObject.FindGameObjectsWithTag("Minion");

            foreach (GameObject minion in gameobjects){
                if (!selectedMinions.Contains(minion.GetComponent<MinionController>())){
                    selectedMinions.Add(minion.GetComponent<MinionController>());
                }
            }
        }

        // If clicking 
        if (Input.GetMouseButtonDown(0)){
            // Clicked on enemy
            if (enemyRaycast.collider != null){
                // If clicked on already selected enemy, deselect enemy
                if (enemyRaycast.collider.gameObject == target){
                    target.GetComponent<EnemyHealth>().selected = false;
                    target = null;
                    foreach (MinionController minion in selectedMinions){
                        minion.enemyTarget = target;
                    }
                }  
                // If clicked on not selected enemy, select enemy
                else {
                    target = enemyRaycast.collider.gameObject;
                    target.GetComponent<EnemyHealth>().selected = true;
                    foreach (MinionController minion in selectedMinions){
                        minion.enemyTarget = target;
                    }
                }
            }
            // CLicked on minion
            else if (minionRaycast.collider != null){
                MinionController minion = minionRaycast.collider.gameObject.GetComponent<MinionController>();
                Debug.Log("Clicked on Minion");
                // Check if minion has been selected and deselect or select accordingly
                if (selectedMinions.Contains(minion)){
                    // If all minions are selected right not, deselect them
                    if (allMinionsSelected){
                        selectedMinions.Clear();
                        selectedMinions.Add(minion);
                        minion.selected = true;
                        allMinionsSelected = false;
                    }
                    // If not, deselect the minion
                    else {
                        selectedMinions.Remove(minion);
                        minion.selected = false;
                    }
                }   else {
                    selectedMinions.Add(minion);
                    minion.selected = true;
                }
            }
        }
        // If holding down mouse
        else if (Input.GetMouseButton(0)){
            // Move all selected minions to mouse position
            if (!clicked){
                foreach(MinionController minion in selectedMinions){
                    target = null;
                    minion.target  =mousePosition;
                    minion.enemyTarget = target;
                }
            }
        }

        // Double Clicking 

        // If click has not registered as double click, register it
        if (!clicked && Input.GetMouseButtonDown(0)){
            clicked = true;
        }   
        // If double click process has started, continue
        else if (clicked) {
            clickTime += Time.deltaTime;
            
            // If double click, reset selected minions
            if (Input.GetMouseButtonDown(0)){
                allMinionsSelected = true;
                clickTime = 0;
                clicked = false;
            }
            // If time passes the click delay time, reset click process
            if (clickTime > clickDelayTime){
                clickTime = 0;
                clicked = false;
            }
        }

        // If all minions are supposed to be selected
        if (allMinionsSelected){
            // Select all minions
            GameObject[] gameobjects = GameObject.FindGameObjectsWithTag("Minion");

            foreach (GameObject minion in gameobjects){
                if (!selectedMinions.Contains(minion.GetComponent<MinionController>())){
                    selectedMinions.Add(minion.GetComponent<MinionController>());
                }
            }
            // Show minions as not selected (Because all of them are already selected)
            foreach (MinionController minion in selectedMinions){
                minion.selected = false;
            }
        }  
    }
}
