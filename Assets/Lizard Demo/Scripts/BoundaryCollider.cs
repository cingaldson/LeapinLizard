using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryCollider : MonoBehaviour
{
    public GameObject boundaryObject;
    private bool isGameOver = false;


    public float forceMagnitude = 10f; // Magnitude of the force applied to the player

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                // Calculate the direction opposite to the collision normal
                Vector3 forceDirection = -collision.contacts[0].normal;

                // Apply the force to the player
                playerRigidbody.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
            }
        }
    }
}

