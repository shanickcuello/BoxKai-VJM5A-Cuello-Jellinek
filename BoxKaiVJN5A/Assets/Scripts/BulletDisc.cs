using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDisc : MonoBehaviourPun
{
    [SerializeField] float speedMovement;
    [SerializeField] int damage;
    CharacterFA owner;
    float counter;

    private void Start()
    {
        counter = 20;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        transform.position += transform.forward * speedMovement * Time.deltaTime;

        counter -= Time.deltaTime;
        if (counter <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public BulletDisc SetOwner(CharacterFA owner)
    {
        this.owner = owner;
        return this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
            return;

        var character = other.GetComponent<CharacterFA>();
        if (character && character != owner)
        {
            character.TakeDamage(damage);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

