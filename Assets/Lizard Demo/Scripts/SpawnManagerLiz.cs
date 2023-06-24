using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagerLiz : MonoBehaviour
{
    public GameObject[] objectPrefabs;
    private float spawnDelay = 2;
    private float spawnInterval = 5f;
    private bool isGameActive = false;
    private Coroutine spawnRoutine;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    [SerializeField] private PlayerControllerLiz playerControllerScript;
    [SerializeField] private GameManagerLiz gameManager;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = FindObjectOfType<PlayerControllerLiz>();
        gameManager = FindObjectOfType<GameManagerLiz>();
    }

    public void StartSpawning()
    {
        if (gameManager != null && gameManager.isGameActive)
        {
            isGameActive = true;

            // Assign the playerControllerScript reference if it's not already assigned
            if (playerControllerScript == null)
            {
                playerControllerScript = FindObjectOfType<PlayerControllerLiz>();
            }

            InvokeRepeating("SpawnObjects", spawnDelay, spawnInterval);
        }
    }

    // Spawn obstacles
    void SpawnObjects()
    {
        // If game is still active, spawn new object
        if (isGameActive)
        {
            int index = Random.Range(0, objectPrefabs.Length);

            Vector3 spawnLocation;
            if (objectPrefabs[index].name == "Broom")
            {
                spawnLocation = new Vector3(30, Random.Range(4, 9), 0);
            }
            else
            {
                spawnLocation = new Vector3(30, Random.Range(1, 8), 0);
            }

            Debug.Log("Spawn Location: " + spawnLocation + ", Index: " + index);

            if (objectPrefabs[index].name == "Broom")
            {
                // Adjust the Y position if the object is a broom
                float adjustedY = Mathf.Clamp(spawnLocation.y, 4, 8);
                spawnLocation = new Vector3(spawnLocation.x, adjustedY, spawnLocation.z);
            }

            GameObject spawnedObject = Instantiate(objectPrefabs[index], spawnLocation, objectPrefabs[index].transform.rotation);
            spawnedObjects.Add(spawnedObject);
        }
    }


    public void StopSpawning()
    {
        // Stop any spawning logic and reset variables
        if (isGameActive)
        {
            isGameActive = false;
            CancelInvoke("SpawnObjects");

            // Destroy spawned objects
            foreach (GameObject spawnedObject in spawnedObjects)
            {
                Destroy(spawnedObject);
            }
            spawnedObjects.Clear();
        }
        // Reset any other variables or states related to spawning
    }
}