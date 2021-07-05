using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerForTest : MonoBehaviour
{
    public GameObject targetTolook;

    private void Update()
    {
        transform.forward = new Vector3(targetTolook.transform.position.x, 0, targetTolook.transform.position.z);
    }

}
