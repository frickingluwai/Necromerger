using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManger : MonoBehaviour
{
    // TODO Rework this
    public GameObject[] meleeEnemies;

    public bool active;

    public Vector2[] waves;

    public Transform[] spawnPoints;

    public float timeBetweenWaves;
    public int currentWaves;

    public bool spawning;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWaves > waves.Length -1)
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().win = true;
            
        }
        if (active && !spawning)
        {
            if (currentWaves <= waves.Length - 1)
            {
                if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
                {

                    StartCoroutine(spawnWaves(waves[currentWaves]));
                }
            }
            
        }
    }

    
    IEnumerator spawnWaves(Vector2 wave)
    {
        spawning = true;
        yield return new WaitForSeconds(timeBetweenWaves);
        for (int enemies = 0; enemies <= wave.x; enemies++)
        {
            Instantiate(meleeEnemies[Random.Range(0, 2)], spawnPoints[Random.Range(0, 3)].transform.position, Quaternion.identity);
            yield return new WaitForSeconds(wave.y);
        }
        print(GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
        currentWaves += 1;
        spawning = false;
        
    }

    [ContextMenu("Spawn Enemy")]
    void SpawnEnemy()
    {
        
        Instantiate(meleeEnemies[Random.Range(0, 1)], spawnPoints[Random.Range(0, 3)].transform.position, Quaternion.identity);
    }

}
