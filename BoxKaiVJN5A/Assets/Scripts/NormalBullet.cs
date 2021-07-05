using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NormalBullet : MonoBehaviourPun
{
    public int dmg = 1;
    float _speed = 4;
    CharacterFA _owner;

    void Update()
    {
        if (!photonView.IsMine) return;

        transform.position += transform.right * _speed * Time.deltaTime;
    }

    public NormalBullet SetOwner(CharacterFA owner)
    {
        _owner = owner;
        return this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine)
            return;

        var character = collision.GetComponent<CharacterFA>();

        if (character && character != _owner)
        {
            character.TakeDamage(dmg);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
