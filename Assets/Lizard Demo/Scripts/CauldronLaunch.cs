using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronLaunch : MonoBehaviour
{
    public GameObject[] objectPrefabs;
    private float spawnDelay = 2;
    private float spawnInterval = 1.5f;
    public Transform spawnPoint; // Reference point for object spawning

    private PlayerControllerLiz playerControllerScript;
    public bool isGameActive = false;
    private Coroutine spawnRoutine;

    private GameManagerLiz gameManager;


    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerControllerLiz>();
        gameManager = FindObjectOfType<GameManagerLiz>();
    }

    public void StartSpawning()
    {
        if (!isGameActive)
        {
            isGameActive = true;
            InvokeRepeating("SpawnObjects", spawnDelay, spawnInterval);
        }
    }

    // Spawn obstacles
    void SpawnObjects()
    {
        // Set random object index
        int index = Random.Range(0, objectPrefabs.Length);

        // If game is still active, spawn new object
        if (isGameActive)
        {
            GameObject spawnedObject = Instantiate(objectPrefabs[index], spawnPoint.position, objectPrefabs[index].transform.rotation);
            Destroy(spawnedObject, 5f); // Destroy the spawned object after 5 seconds
        }
    }
    public void StopSpawning()
    {
        // Stop any spawning logic and reset variables
        isGameActive = false;
        CancelInvoke("SpawnObjects");
        StopAllCoroutines();
        // Reset any other variables or states related to spawning
    }
}