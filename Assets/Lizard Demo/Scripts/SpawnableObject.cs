using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnableObject : MonoBehaviour
{
    // Common properties and methods for all spawnable objects go here POLYMORPHISM

    public abstract void Spawn(Vector3 spawnLocation);
}
