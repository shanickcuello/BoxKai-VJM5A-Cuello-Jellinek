using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool ongameStarts;

    public bool OngameStarts { get => ongameStarts; }

    LimitPlatform limitPlatform;

    private void Start()
    {
        limitPlatform = FindObjectOfType<LimitPlatform>();
    }

    private void Update()
    {
        if (PhotonNetwork.PlayerList.Length > 4)
        {
            ongameStarts = true;
            limitPlatform.GetDown();
        }
    }

}
