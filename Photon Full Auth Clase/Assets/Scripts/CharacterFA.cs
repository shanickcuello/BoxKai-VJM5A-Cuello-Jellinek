using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharacterFA : MonoBehaviourPun
{
    Player playerId;

    [SerializeField] float speedMovement;
    [SerializeField] GameObject bulletSpawnPosition;

    public float maxLife, life, dmg;

    public BulletDisc bulletPrefab;
    public GameObject lookAtPoint;
    public Camera camera;
    public MeshFilter planeAim;
    SpawnPositionManager spawnPositionManager;

    private void Start()
    {
        spawnPositionManager = FindObjectOfType<SpawnPositionManager>();
        camera = Camera.main;
        planeAim = FindObjectOfType<AimPlane>().GetComponent<MeshFilter>();
        lookAtPoint = FindObjectOfType<LookAtPoint>().gameObject;
    }


    public void Move(Vector3 dir)
    {
        transform.position += dir * speedMovement * Time.deltaTime;
    }

    public void RotateMe()
    {
        if (planeAim != null)
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
        life -= dmg;

        if (life <= 0)
        {
            MyServer.Instance.PlayerDisconnect(playerId); // aca hacer que el player pierda pero no desconectarlo
            photonView.RPC("DisconnectOwner", playerId);
        }
        else
        {
            photonView.RPC("OnLifeChange", playerId, life);
        }
    }

    [PunRPC]
    void DisconnectOwner()
    {
        PhotonNetwork.Disconnect();
    }

    [PunRPC]
    void OnLifeChange(int life)
    {
        this.life = life;
    }
}
