using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerMove : NetworkBehaviour
{
    public float forward = 0;
    public float turnSpeed;
    public float speed = 3;
    public float jumpForce = 500;
    public float jumpTime = 1;

    private Animator anim; // 控制动画
    private bool isJump = false;//控制空中不能跳跃
    
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        // 攻击状态下不控制移动
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded") == false) return;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
       
        // 当有键按下的时候再进行移动控制
        if (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0)
        {
            // 控制移动使用世界坐标Space.World
            transform.Translate(new Vector3(h, 0, v) * (speed * Time.deltaTime));
            // 朝向设置与按键方向一致
            //transform.rotation = Quaternion.LookRotation(new Vector3(h, 0, v));
            transform.Rotate(new Vector3(0.0f, h * turnSpeed, 0.0f));
            float res = Mathf.Max(Mathf.Abs(h), Mathf.Abs(v));
            forward = res;
            //anim.SetFloat("Forward", res);
            CmdMove(res);
        }

    }
    [Command]
    void CmdMove(float res)
    {
        this.RpcMoveAni(res);
    }
    [ClientRpc]
    void RpcMoveAni(float res)
    {
        anim.SetFloat("Forward", res);
    }
}