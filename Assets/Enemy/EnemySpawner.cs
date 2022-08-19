using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject pEnemy;
    [Header("Spawner Settings")]
    [SerializeField] private float spawnTime;
    [SerializeField] private int maxNoEnemies = 3;
    [SerializeField] private bool randomizeSpawnTime = false;
    [SerializeField] private float minRandomRange = 0.0f;
    [SerializeField] private float maxRandomRange = 1.0f;

    private float spawnTimer;
    
    void Start()
    {
        spawnTimer = 10.0f;
    }

    void Update()
    {
        Spawn();
    }
    void Spawn()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0.0f && this.transform.childCount < maxNoEnemies)
        {
            GameObject enemyGO = Instantiate(pEnemy, transform.position, Quaternion.identity);
            enemyGO.transform.SetParent(this.transform);
            spawnTimer = randomizeSpawnTime ? Random.Range(minRandomRange, maxRandomRange) : spawnTime;
        }
    }
}
