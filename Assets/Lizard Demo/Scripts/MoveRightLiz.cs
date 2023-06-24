using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRightLiz : MonoBehaviour
{
    public float speed;
    private PlayerControllerLiz playerControllerScript;
    private float rightBound = 20;
    private float spawnPositionX = -20;
    private bool isMoving;
    private bool isRespawning;
    private float maxPlayerXPosition = 17;
    public float desiredDetachPositionX;
    public float destroyDelay = 2f;
    private float minYPosition = 4f; // Minimum Y position for spawning the broom

    private List<Transform> spawnedObjects; // List to store spawned objects

    // Start is called before the first frame update
    void Start()
    {
        isMoving = true;
        isRespawning = false;

        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerControllerScript = player.GetComponent<PlayerControllerLiz>();
        }
        else
        {
            Debug.LogWarning("Player object not found. Make sure the player object is present in the scene.");
        }

        // Set initial position to the spawn position
        transform.position = new Vector3(spawnPositionX, transform.position.y, transform.position.z);

        spawnedObjects = new List<Transform>(); // Initialize the list of spawned objects
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRespawning)
        {
            if (playerControllerScript != null && !playerControllerScript.gameOver)
            {
                if (isMoving)
                {
                    MoveRight();
                }
                else
                {
                    StartCoroutine(RespawnBroom());
                }
            }
        }

        // If object goes off screen that is NOT the background, destroy it
        if (transform.position.x > rightBound && !gameObject.CompareTag("Background") && !gameObject.CompareTag("Player"))
        {
            StartCoroutine(DestroyBroomWithDelay());
        }

        // Check if player is on the broom and move the player along with it
        if (playerControllerScript != null && playerControllerScript.IsOnBroom)
        {
            Vector3 playerPosition = playerControllerScript.transform.position;
            playerPosition += Vector3.right * speed * Time.deltaTime;
            playerPosition.x = Mathf.Clamp(playerPosition.x, -Mathf.Infinity, maxPlayerXPosition);

            // Check if the player reaches the desired detach position
            if (playerPosition.x >= desiredDetachPositionX)
            {
                // Detach the player from the broom
                playerControllerScript.SetOnBroom(false);
                playerControllerScript = null; // Reset the reference to prevent further movement with the broom
            }
            else
            {
                playerControllerScript.transform.position = playerPosition;
            }
        }

        // Move the spawned objects to the left
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            Transform obj = spawnedObjects[i];
            obj.position += Vector3.left * speed * Time.deltaTime;

            // Remove the spawned object if it goes off screen
            if (obj.position.x < -rightBound)
            {
                spawnedObjects.Remove(obj);
                Destroy(obj.gameObject);
            }
        }
    }

    private void MoveRight()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
    }

    private IEnumerator RespawnBroom()
    {
        isRespawning = true;
        yield return new WaitForSeconds(2);

        transform.position = new Vector3(spawnPositionX, transform.position.y, transform.position.z);
        isMoving = true;
        isRespawning = false;

        if (playerControllerScript != null)
        {
            playerControllerScript.transform.position = new Vector3(spawnPositionX, playerControllerScript.transform.position.y, playerControllerScript.transform.position.z);
        }
    }

    private IEnumerator DestroyBroomWithDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Broom"))
        {
            // Check if the broom enters the boundary
            float clampedYPosition = Mathf.Clamp(other.transform.position.y, minYPosition, Mathf.Infinity);
            other.transform.position = new Vector3(other.transform.position.x, clampedYPosition, other.transform.position.z);
        }

        // Add the spawned object to the list when it enters the trigger
        spawnedObjects.Add(other.transform);
        if (other.CompareTag("Boundary"))
        {
            // Remove the spawned object from the list when it reaches the boundary
            spawnedObjects.Remove(other.transform);
        }
    }

    public void SetOnBoundary()
    {
        if (playerControllerScript != null)
        {
            playerControllerScript.SetOnBroom(false);
        }
    }
}