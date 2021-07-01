using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class MyServer : MonoBehaviourPun
{
    public static MyServer Instance;
    Player server;
    public CharacterFA characterPrefab;
    Dictionary<Player, CharacterFA> playerModels = new Dictionary<Player, CharacterFA>();
    Dictionary<Player, CharacterViewFA> _dicViews = new Dictionary<Player, CharacterViewFA>();



    SpawnPositionManager spawnPositionManager;

    public int PackagesPerSecond { get; private set; }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        if (Instance == null)
        {
            if (photonView.IsMine)
            {
                photonView.RPC("SetServer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer, 1);
            }
        }
    }

    [PunRPC]
    void SetServer(Player serverPlayer, int sceneIndex = 1)
    {
        if (Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;


        server = serverPlayer;

        PackagesPerSecond = 60;

        PhotonNetwork.LoadLevel(sceneIndex);

        var playerLocal = PhotonNetwork.LocalPlayer;


        if (playerLocal != server)
        {
            photonView.RPC("AddPlayer", server, playerLocal);
        }
    }

    [PunRPC]
    void AddPlayer(Player player)
    {
        StartCoroutine(WaitForLevel(player));
    }

    IEnumerator WaitForLevel(Player player)
    {
        while (PhotonNetwork.LevelLoadingProgress > 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }

        CharacterFA newCharacter = PhotonNetwork.Instantiate(characterPrefab.name, Vector3.zero, Quaternion.identity)
                                   .GetComponent<CharacterFA>().SetInitialParameters(player);

        spawnPositionManager = FindObjectOfType<SpawnPositionManager>();
        newCharacter.transform.position = spawnPositionManager.GetNextPos(playerModels.Count + 1).position;

        playerModels.Add(player, newCharacter);
        _dicViews.Add(player, newCharacter.GetComponent<CharacterViewFA>());
    }




    /* REQUESTS QUE LE LLEGAN AL SERVER AVATAR */
    public void RequestMove(Player player, Vector3 dir)
    {
        photonView.RPC("Move", server, player, dir);
    }

    internal void RequestRotation(Player playerid)
    {
        photonView.RPC("RotatePlayer", server, playerid);
    }

    public void RequestShoot(Player player)
    {
        photonView.RPC("Shoot", server, player);
    }

    public void PlayerDisconnect(Player player)
    {
        PhotonNetwork.Destroy(playerModels[player].gameObject);
        playerModels.Remove(player);
        _dicViews.Remove(player);
    }

    /* FUNCIONES DEL SERVER ORIGINAL QUE LE LLEGAN DEL AVATAR */
    [PunRPC]
    void Move(Player player, Vector3 dir)
    {
        if (playerModels.ContainsKey(player))
        {
            playerModels[player].Move(dir);
            _dicViews[player].SetVel(dir.magnitude);
        }
    }

    [PunRPC]
    void RotatePlayer(Player player)
    {
        if (playerModels.ContainsKey(player))
        {
            playerModels[player].RotateMe();
        }
    }

    [PunRPC]
    void Shoot(Player player)
    {
        if (playerModels.ContainsKey(player))
        {
            playerModels[player].Shoot();
        }
    }
}
