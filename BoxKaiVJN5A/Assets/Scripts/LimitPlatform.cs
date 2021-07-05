using Photon.Pun;
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

    private void Update()
    {
        if (PhotonNetwork.PlayerList.Length > 4)
        {
            GetDown();
        }
    }

    public void GetDown()
    {
        anim.SetTrigger("down");
    }

}
