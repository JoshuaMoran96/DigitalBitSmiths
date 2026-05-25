using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    //going to set to a basic default for now but we can make it a slider and scale up for later
    [SerializeField] int amountToSpawn = 3;
    [SerializeField] float spawnRate = 0.5f;
    [SerializeField] float spawnDist = 5f;


    [Header("Ground Check")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float raycastHeight = 5f;
    [SerializeField] float spawnYOffset = 0.5f;
    int spawnCount;
    float spawnTimer;
    bool startSpawning;
    bool registeredEnemies;

    // Update is called once per frame
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
    //Adjusted for a 2D enviornment instead of the 3d class setup
    //Took template from raycasted enemy AI
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            startSpawning = true;
            spawnTimer = spawnRate;
        }
        //Get the npcs out and moving of listed spawn amount
        if (!registeredEnemies && gamemanager.instance != null)
        {
            gamemanager.instance.updateEnemyCount(amountToSpawn);
            registeredEnemies = true;
        }
    }

    void spawn()
    {
        spawnTimer = 0f;

        Vector2 randomStartPos = new Vector2( transform.position.x + Random.Range(-spawnDist, spawnDist),transform.position.y + raycastHeight);

        RaycastHit2D hit = Physics2D.Raycast(randomStartPos,Vector2.down,raycastHeight * 2f,groundLayer);

        if (hit.collider != null)
        {
            Vector2 spawnPosition = hit.point + Vector2.up * spawnYOffset;
            Instantiate(objectToSpawn, hit.point, Quaternion.identity);
            spawnCount++;
        }
        else
        {
            Debug.LogWarning("Spawner could not find ground.");
        }
    }

}

