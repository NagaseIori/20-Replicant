using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyWithProbability {
    public Enemy enemyPrefab;
    [Range(0, 1)]
    public double probability = 1.0;
}

public class EnemySpawner : MonoBehaviour
{
    public List<EnemyWithProbability> enemyWithProbabilities;
    public double enemySpawnDelay = 1;     // Delay between enemies' spawning.
    protected double enemySpawnTimer = 0;

    void Update()
    {
        // If player is dead, destroy self.
        if(!GameManager.IsPlayerAlive())
            Destroy(gameObject);

        enemySpawnTimer += Time.deltaTime;
        if(enemySpawnTimer > enemySpawnDelay) {
            SpawnEnemies();
            enemySpawnTimer = 0;
        }
    }

    void SpawnEnemies() {
        foreach(var enemyPair in enemyWithProbabilities) {
            // Probability test.
            if(Random.Range(0f, 1f) <= enemyPair.probability) {
                SpawnEnemyOutsideView(enemyPair.enemyPrefab);
            }
        }
    }

    void SpawnEnemyOutsideView(Enemy enemyPrefab)
    {
        Camera camera = Camera.main;

        float orthographicHeight = camera.orthographicSize;
        float orthographicWidth = orthographicHeight * camera.aspect;

        float leftBound = camera.transform.position.x - orthographicWidth;
        float rightBound = camera.transform.position.x + orthographicWidth;
        float topBound = camera.transform.position.y + orthographicHeight;
        float bottomBound = camera.transform.position.y - orthographicHeight;

        int randomEdge = Random.Range(0, 4);
        Vector3 spawnPosition = Vector3.zero;

        float offset = 0.5f;

        switch (randomEdge)
        {
            case 0:
                spawnPosition = new Vector3(leftBound - offset, Random.Range(bottomBound, topBound), 0);
                break;
            case 1:
                spawnPosition = new Vector3(rightBound + offset, Random.Range(bottomBound, topBound), 0);
                break;
            case 2:
                spawnPosition = new Vector3(Random.Range(leftBound, rightBound), topBound + offset, 0);
                break;
            case 3:
                spawnPosition = new Vector3(Random.Range(leftBound, rightBound), bottomBound - offset, 0);
                break;
        }
        var inst = Instantiate(enemyPrefab);
        inst.transform.position = spawnPosition;
    }
}