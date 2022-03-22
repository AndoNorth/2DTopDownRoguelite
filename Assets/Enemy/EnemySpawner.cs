using UnityEngine;
// kieran wrote this
public class EnemySpawner : MonoBehaviour
{
    //Public Variables
    public GameObject enemy;

    public float spawnTime; // maybe change to per second then calculate the spawnRate in start function
    public float spawnTimeRandom; // then we can make this a random scaled version of that
    public GameObject target;
    public int maxNoEnemies = 3;

    //Private Variables
    private float spawnTimer;
    
    //Used for initialisation
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Character");
        ResetSpawnTimer();
    }

    //Update is called once per frame
    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0.0f && this.transform.childCount < maxNoEnemies)
        {
            GameObject enemyGO = Instantiate(enemy, transform.position, Quaternion.identity);
            enemyGO.transform.SetParent(this.transform);
            ResetSpawnTimer();
        }
    }

    //Resets the spawn timer with a random offset
    void ResetSpawnTimer()
    {
        spawnTimer = (float)(spawnTime + Random.Range(0, spawnTimeRandom * 100) / 100.0);
    }
}
