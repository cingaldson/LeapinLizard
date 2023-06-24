using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Broom : SpawnableObject
{
    public override void Spawn(Vector3 spawnLocation)
    {
        // Implement spawning behavior for broom
        Instantiate(this, spawnLocation, Quaternion.identity);
    }
}
