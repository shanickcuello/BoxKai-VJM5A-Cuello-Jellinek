using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharacterFA : MonoBehaviourPun
{
    Player playerId;

    public GameObject winpanel, loosepanel;

    GameManager gameManager;

    Rigidbody rb;
    PhotonView pv;
    [SerializeField] float speedMovement;
    [SerializeField] GameObject bulletSpawnPosition;

    public float maxLife, life, dmg;

    public BulletDisc bulletPrefab;
    public GameObject lookAtPoint;
    public Camera camera;
    public MeshFilter planeAim;
    SpawnPositionManager spawnPositionManager;

    public bool alive;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        alive = true;
        pv = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        spawnPositionManager = FindObjectOfType<SpawnPositionManager>();
        camera = Camera.main;
        planeAim = FindObjectOfType<AimPlane>().GetComponent<MeshFilter>();
        lookAtPoint = FindObjectOfType<LookAtPoint>().gameObject;
    }

    private void Update()
    {
        CheckAlive();
        CheckWin();
        
        
    }

    public void CheckWin()
    {
        CharacterFA[] characters = FindObjectsOfType<CharacterFA>();

        int playersDeads = 0;

        foreach (var item in characters)
        {
            if (!item.alive)
            {
                playersDeads++;
            }
        }

        if (playersDeads > 2)
        {
            if (alive)
            {
                ShowWin();
            }
            else
            {
                ShowLoose();
            }
        }

    }

    private void CheckAlive()
    {
        if (pv.IsMine)
        {
            if (life <= 0 && alive)
            {
                alive = false;
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.mass = 0;
                rb.constraints = 0;
                rb.freezeRotation = true;
                rb.constraints = RigidbodyConstraints.FreezeAll;
                DeadPosition deadPosition = FindObjectOfType<DeadPosition>();

                Vector3 randomPosition = new Vector3(deadPosition.transform.position.x,
                    deadPosition.transform.position.y,
                    deadPosition.transform.position.z + Random.Range(-8, 9));

                transform.position = randomPosition;
            }
        }


    }

    internal void ShowWin()
    {
        winpanel.SetActive(true);
    }

    internal void ShowLoose()
    {
        loosepanel.SetActive(true);
    }

    public void Move(Vector3 dir)
    {
        if (!alive)
        {
            return;
        }
        transform.position += dir * speedMovement * Time.deltaTime;
    }

    public void RotateMe()
    {
        if (!alive)
        {
            return;
        }

        if (planeAim != null && pv.IsMine)
        {

            var plane = new Plane(planeAim.transform.position,
           planeAim.transform.position +
           planeAim.transform.forward,
           planeAim.transform.position +
           planeAim.transform.right);

            var ray = camera.ScreenPointToRay(Input.mousePosition);

            if (!plane.Raycast(ray, out var enter)) return;

            var hitPosition = ray.GetPoint(enter);

            if (hitPosition.x < planeAim.mesh.bounds.min.x * planeAim.transform.lossyScale.x
                || hitPosition.x > planeAim.mesh.bounds.max.x * planeAim.transform.lossyScale.x
                || hitPosition.z < planeAim.mesh.bounds.min.z * planeAim.transform.lossyScale.z
                || hitPosition.z > planeAim.mesh.bounds.max.z * planeAim.transform.lossyScale.z)
            {
                return;
            }

            lookAtPoint.transform.position = hitPosition + Vector3.up * 0.1f;
            Vector3 dirtoLook = new Vector3(lookAtPoint.transform.position.x, 0, lookAtPoint.transform.position.z);

            transform.forward = dirtoLook;
        }
    }

    public void Shoot()
    {
        if (!alive)
        {
            return;
        }
        PhotonNetwork.Instantiate(bulletPrefab.name, bulletSpawnPosition.transform.position, transform.rotation)
                     .GetComponent<BulletDisc>()
                     .SetOwner(this);
    }

    public CharacterFA SetInitialParameters(Player localPlayer)
    {
        playerId = localPlayer;
        life = maxLife;
        photonView.RPC("SetLocalParams", playerId, maxLife);
        return this;
    }

    [PunRPC]
    void SetLocalParams(float life)
    {
        this.life = maxLife;
        //GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void TakeDamage(int dmg)
    {
        if (!alive)
        {
            return;
        }
        life -= dmg;

        //if (life <= 0)
        //{
        //    MyServer.Instance.PlayerDisconnect(playerId); // aca hacer que el player pierda pero no desconectarlo
        //    photonView.RPC("DisconnectOwner", playerId);
        //}
        //else
        //{
        //    photonView.RPC("OnLifeChange", playerId, life);
        //}
    }

    //[PunRPC]
    //void DisconnectOwner()
    //{
    //    PhotonNetwork.Disconnect();
    //}

    //[PunRPC]
    //void OnLifeChange(int life)
    //{
    //    this.life = life;
    //}
}
