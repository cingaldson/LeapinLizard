using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeftObjects : MonoBehaviour
{
    public float speed;
    private PlayerControllerLiz playerControllerScript;
    private float leftBound = -10;
    private float rightBound = 30;
    private float maxPlayerXPosition = 17;

    // Start is called before the first frame update
    void Start()
    {
        // Find the PlayerControllerLiz component in the scene
        playerControllerScript = FindObjectOfType<PlayerControllerLiz>();

        // Set the initial X position of the obstacle based on the rightBound
        float initialXPosition = rightBound + Random.Range(1f, 3f);
        transform.position = new Vector3(initialXPosition, transform.position.y, transform.position.z);

        // Disable the script if the game is over
        if (playerControllerScript != null && playerControllerScript.gameOver)
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerControllerScript != null && !playerControllerScript.gameOver)
        {
            // If the player is on the broom, do not move the object to the left
            if (playerControllerScript.IsOnBroom)
                return;

            // If game is not over, move to the left
            transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

            // If object goes off screen that is NOT the background, destroy it
            if (transform.position.x < leftBound && !gameObject.CompareTag("Background"))
            {
                Destroy(gameObject);
            }

            // Check if the broom has reached the right boundary and remove the player from the broom
            if (transform.position.x > rightBound && !gameObject.CompareTag("Background"))
            {
                if (playerControllerScript.IsOnBroom)
                {
                    playerControllerScript.SetOnBroom(false);
                }
            }
        }
    }
}
