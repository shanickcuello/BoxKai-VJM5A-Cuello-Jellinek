using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ControllerFA : MonoBehaviourPun
{
    Player playerId;
    float shootCoolDown;
    [SerializeField] float shootRecail;
    [SerializeField] Transform lookAtPoint;


    private void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        playerId = PhotonNetwork.LocalPlayer;
        StartCoroutine(SendPackages());
    }

    IEnumerator SendPackages()
    {
        while (true)
        {
            PackageUpdate();
            yield return new WaitForSeconds(1 / MyServer.Instance.PackagesPerSecond);
        }
    }

    private void PackageUpdate()
    {
        if (PhotonNetwork.PlayerList.Length < 5)
            return;
        RequestMovement();
    }

    private void RequestMovement()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        if (h != 0 || v != 0)
        {
            var dir = new Vector3(-h, 0, -v).normalized;
            MyServer.Instance.RequestMove(playerId, dir);
        }

    }

    private void Update()
    {
        Aim();
        if (PhotonNetwork.PlayerList.Length < 5)
            return;
        Shoot();
        Reloading();
    }

    private void Aim()
    {
        MyServer.Instance.RequestRotation(playerId);
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b) => Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;

    void Shoot()
    {
        if (Input.GetKeyUp(KeyCode.Space) && shootCoolDown <= 0)
        {
            MyServer.Instance.RequestShoot(playerId);
            shootCoolDown += 0.3f;
        }
    }

    void Reloading()
    {
        if (shootCoolDown <= 0)
        {
            shootCoolDown = 0;
        }
        else
        {
            shootCoolDown -= Time.deltaTime;
        }
    }

}
