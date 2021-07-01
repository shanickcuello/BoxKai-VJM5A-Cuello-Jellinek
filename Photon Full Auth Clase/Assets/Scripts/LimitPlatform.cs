using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitPlatform : MonoBehaviour
{

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();


    }

    public void GetDown()
    {
        anim.SetTrigger("down");
    }

}
