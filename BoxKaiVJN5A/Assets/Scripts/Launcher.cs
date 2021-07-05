using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public MyServer serverPrefab;
    public ControllerFA controllerPrefab;
    [SerializeField] string roomName;
    public void BTN_Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 20;

        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        PhotonNetwork.Instantiate(serverPrefab.name, Vector3.zero, Quaternion.identity);
    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(controllerPrefab.name, Vector3.zero, Quaternion.identity);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Connection failed: {cause.ToString()}");
    }
}
