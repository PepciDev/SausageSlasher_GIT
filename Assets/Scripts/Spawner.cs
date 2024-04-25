using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public GameObject ghostPrefab;

    int round = 0;

    int score = 0;
    public int tempPoints = 0;

    public Text scoreCounter;

    float spawnDelay = 2f;

    void Start()
    {
        //first enemy instantion
        GameObject instantion = Instantiate(enemyPrefab, transform.position, enemyPrefab.transform.rotation);
        instantion.GetComponent<EnemyController>().randomizeStats = false;
        //StartCoroutine("SpawnDelay");
    }

    public IEnumerator SpawnDelay()
    {
        yield return new WaitForSecondsRealtime(spawnDelay);
        Spawn();
    }

    void Spawn()
    {
        //spawn enemy
        Instantiate(enemyPrefab, transform.position, enemyPrefab.transform.rotation);
    }

    public void SpawnGhost()
    {
        //spawn ghost enemy with a little offset
        Instantiate(ghostPrefab, new Vector3(transform.position.x, transform.position.y + 0.74f, transform.position.z + 0.88f), ghostPrefab.transform.rotation);
    }

    public void AddScore(int points)
    {
        round += 1;
        score += points + tempPoints;
        tempPoints = 0;

        scoreCounter.text = score.ToString();
    }
}
