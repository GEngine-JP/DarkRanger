using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
*  作   者 ：胡朋
*  github : https://github.com/xiaomoinfo
*  描   述 ：
*/
public class Player : MonoBehaviour
{
    public float moveSpeed = 10;
    private CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");
        cc.SimpleMove(new Vector3(x * moveSpeed, 0, z * moveSpeed));
    }
}