using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeftLiz : MonoBehaviour
{
    private float speed = 20;
    private PlayerControllerLiz playerControllerScript;
    private float leftBound = -15;
    private bool canMove = false;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = FindObjectOfType<PlayerControllerLiz>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerControllerScript.gameOver == false && canMove)
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
        }

        if (transform.position.x < leftBound && gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }
}
