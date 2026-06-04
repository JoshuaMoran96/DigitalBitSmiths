using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int amountToSpawn = 3;
    [SerializeField] float spawnRate = 0.5f;
    [SerializeField] float spawnDist = 5f;
    [SerializeField] float spawnYOffset = 0.5f;

    int spawnCount;
    float spawnTimer;
    bool startSpawning;
    bool registeredEnemies;

    void Update()
    {
        if (!startSpawning)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnCount < amountToSpawn && spawnTimer >= spawnRate)
        {
            spawn();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !startSpawning)
        {
            startSpawning = true;
            spawnTimer = spawnRate;

            if (!registeredEnemies && gamemanager.instance != null)
            {
                gamemanager.instance.updateEnemyCount(amountToSpawn);
                registeredEnemies = true;
            }
        }
    }

    void spawn()
    {
        spawnTimer = 0f;

        float spacing = spawnDist * 2f / Mathf.Max(1, amountToSpawn - 1);
        float spawnX = transform.position.x - spawnDist + spacing * spawnCount;

        Vector2 spawnPosition = new Vector2(
            spawnX,
            transform.position.y + spawnYOffset
        );

        Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);

        spawnCount++;
    }

}

