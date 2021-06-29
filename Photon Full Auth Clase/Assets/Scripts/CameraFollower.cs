using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public GameObject target;
    Vector3 offset;

    private void Start()
    {
        transform.SetParent(null);
    }

    private void Update()
    {
        if (target != null)
        {
            transform.LookAt(target.transform.position);
        }
    }


}
