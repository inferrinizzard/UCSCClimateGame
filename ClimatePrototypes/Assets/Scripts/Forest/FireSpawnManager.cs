using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/FireSpawnManager", order = 1)]
public class FireSpawnManager : ScriptableObject
{
    public string name;
    public int prefabs;
    public Vector3[] spawnPoints;

}
