using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPositionManager : MonoBehaviour
{
    [SerializeField] List<GameObject> spawnPositions;

    public Transform GetNextPos(int index)
    {
        Debug.LogWarning("Index es :" + index);
        return spawnPositions[index].transform;
    }

}
