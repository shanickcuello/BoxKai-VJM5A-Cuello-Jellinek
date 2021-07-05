using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class IngamePlayersList : MonoBehaviour
{
    public Text playerList;

    void Update()
    {
        UpdatePlayers();
    }

    public void UpdatePlayers()
    {
        playerList.text = "Players: \n";

        var allPlayers = PhotonNetwork.PlayerList;

        foreach (var pl in allPlayers)
        {
            playerList.text += pl.NickName + "\n";
        }
    }
}
