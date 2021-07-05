using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterViewFA : MonoBehaviour
{
    Animator _anim;

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void SetVel(float magnitude)
    {
        if (!_anim) return;
        _anim.SetFloat("axis", magnitude);
    }
}
