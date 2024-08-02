using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    private Vector3 spawnPosition = Vector3.zero;

    public void SpawnPointInit(Vector3 input)
    {
        spawnPosition = input;
    }

    public Vector3 OriginPoint()
    {
        return spawnPosition;
    }
}
