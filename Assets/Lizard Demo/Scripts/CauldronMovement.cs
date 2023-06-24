using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CauldronMovement : MonoBehaviour
{
    public float moveDuration = 5f; // Duration of the movement in seconds
    public float minPosition = 1f; // Minimum x position
    public float maxPosition = 15f; // Maximum x position

    private float targetPosition; // Target x position
    private float startPosition; // Starting x position
    private float startTime; // Time when the movement starts
    private bool delayComplete = false; // Flag to track if the initial delay is complete
    private bool isMoving = false; // Flag to track if the cauldron is currently moving

    // Start is called before the first frame update
    void Start()
    {
        // Set the starting position to 25
        startPosition = 25f;
        transform.position = new Vector3(startPosition, transform.position.y, transform.position.z);
    }

    void StartMovement()
    {
        delayComplete = true;

        // Set the target position
        targetPosition = Random.Range(minPosition, maxPosition);

        // Set the start time to the current time
        startTime = Time.time;

        isMoving = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the initial delay is complete and the game is active and the cauldron is currently moving
        if (!delayComplete || !isMoving)
            return;

        // Check if the game is over
        GameManagerLiz gameManager = FindObjectOfType<GameManagerLiz>();
        if (gameManager != null && gameManager.isGameOver)
        {
            // Set the starting position and target position to the current position
            startPosition = transform.position.x;
            targetPosition = transform.position.x;

            // Stop the movement
            isMoving = false;
            return;
        }

        // Calculate the normalized time between 0 and 1 based on the elapsed time and move duration
        float normalizedTime = (Time.time - startTime) / moveDuration;

        // Clamp the normalized time between 0 and 1 to ensure smooth movement even if the duration is adjusted
        normalizedTime = Mathf.Clamp01(normalizedTime);

        // Interpolate the position between the start and target positions using the normalized time
        float newPosition = Mathf.Lerp(startPosition, targetPosition, normalizedTime);

        // Update the object's position
        transform.position = new Vector3(newPosition, transform.position.y, transform.position.z);

        // Check if the movement has reached the target position
        if (normalizedTime >= 1f)
        {
            // Movement is complete

            // Set the starting position to the target position
            startPosition = targetPosition;

            // Set a new random target position
            targetPosition = Random.Range(minPosition, maxPosition);

            // Set the start time to the current time
            startTime = Time.time;
        }
    }

    public void StartGame()
    {
        if (!delayComplete)
        {
            delayComplete = true;
            Invoke("StartMovement", 10f);
        }
    }

    public void StopMovement()
    {
        GameManagerLiz gameManager = FindObjectOfType<GameManagerLiz>();
        if (gameManager != null && gameManager.isGameActive)
        {
            isMoving = false;
        }
    }
}





