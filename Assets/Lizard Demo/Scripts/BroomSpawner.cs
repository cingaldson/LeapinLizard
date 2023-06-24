using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroomSpawner : MonoBehaviour
{
    public GameObject broomPrefab;
    public float spawnInterval = 5f;
    public float spawnOffset = 30f;

    private bool isSpawning;
    private PlayerControllerLiz playerControllerScript;

    private void Start()
    {
        playerControllerScript = FindObjectOfType<PlayerControllerLiz>();

        // Start spawning brooms
        StartSpawning();
    }

    private void StartSpawning()
    {
        isSpawning = true;
        StartCoroutine(SpawnBrooms());
    }

    private IEnumerator SpawnBrooms()
    {
        while (isSpawning)
        {
            SpawnBroom();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnBroom()
    {
        // Calculate the spawn position for the broom
        Vector3 spawnPosition = new Vector3(transform.position.x + spawnOffset, Random.Range(4f, 8f), 0f);

        // Instantiate the broom object at the calculated position
        GameObject broomObject = Instantiate(broomPrefab, spawnPosition, Quaternion.identity);

        // Optionally set the player controller reference on the broom object if needed
        if (playerControllerScript != null)
        {
            // Add your code here to set the player controller reference on the broom object, if necessary
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopCoroutine(SpawnBrooms());
    }
}