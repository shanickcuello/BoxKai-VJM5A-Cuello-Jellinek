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

    private void Update()
    {
        if (PhotonNetwork.PlayerList.Length > 4)
        {
            CheckPlayersAlive();
        }
    }

    private void CheckPlayersAlive()
    {
        int playersDeads = 0;
        
        List<CharacterFA> characters = new List<CharacterFA>();
        foreach (var item in playerModels)
        {
            characters.Add(item.Value);
        }

        foreach (var item in characters)
        {
            if (!item.alive)
            {
                playersDeads++;
            }
            else
            {
                return;
            }
        }

        if (playersDeads > 2)
        {
            foreach (var item in characters)
            {
                if (!item.alive)
                {
                    item.ShowLoose();
                }
                else
                {
                    item.ShowWin();
                }
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

    internal void RequestUnableColl(Player playerId)
    {
        photonView.RPC("UnableColl", server, playerId);
    }

    public void RequestLoose(Player player)
    {
        photonView.RPC("Loose", server, player);
    }

    public void RequestWin(Player player)
    {
        photonView.RPC("Win", server, player);
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

    internal void RequestAim(Player playerId, Vector3 mousePosition)
    {
        photonView.RPC("Aim", server, playerId, mousePosition);
    }

    /* FUNCIONES DEL SERVER ORIGINAL QUE LE LLEGAN DEL AVATAR */

    [PunRPC]
    void UnableColl(Player player)
    {
        if (playerModels.ContainsKey(player))
        {
            playerModels[player].UnableCollider();
        }
    }


    [PunRPC]
    void Aim(Player player, Vector3 mousePosition)
    {
        if (playerModels.ContainsKey(player))
        {
            Debug.Log("se llama al pun de AIM");
            playerModels[player].RotateMe(mousePosition);
        }
    }



    [PunRPC]
    void Win(Player player)
    {
        if (playerModels.ContainsKey(player))
        {
            playerModels[player].CheckWin();
        }
    }

    [PunRPC]
    void Loose(Player player)
    {
        if (playerModels.ContainsKey(player))
        {
            playerModels[player].CheckWin();
        }
    }


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
    void Shoot(Player player)
    {
        if (playerModels.ContainsKey(player))
        {
            playerModels[player].Shoot();
        }
    }
}
