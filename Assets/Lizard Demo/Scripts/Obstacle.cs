using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : SpawnableObject
{
    public override void Spawn(Vector3 spawnLocation)
    {
        // Implement spawning behavior for obstacle
        Instantiate(this, spawnLocation, Quaternion.identity);
    }
}
