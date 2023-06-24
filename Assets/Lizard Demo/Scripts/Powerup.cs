using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : SpawnableObject
{
    public override void Spawn(Vector3 spawnLocation)
    {
        // Implement spawning behavior for power-up INHERITANCE
        Instantiate(this, spawnLocation, Quaternion.identity);
    }
}
