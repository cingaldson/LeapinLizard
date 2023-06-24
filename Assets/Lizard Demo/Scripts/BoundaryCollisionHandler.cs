using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryCollisionHandler : MonoBehaviour
{
    private MoveRightLiz moveRightLiz;
    private bool isGameOver = false;

    private void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && moveRightLiz != null && !isGameOver)
        {
            moveRightLiz.SetOnBoundary();
            isGameOver = true;

            // Call a game over function or trigger game over state here
            // You can handle the game over condition according to your game's requirements
            // For example, you can display a game over UI, stop the game, or reload the level.

            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.isKinematic = true;
                playerRb.velocity = Vector3.zero;
            }
        }
    }
}
