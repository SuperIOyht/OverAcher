using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerAttack : NetworkBehaviour {

    public GameObject arrowPrefab;
    private Animator anim; // 通过Animator得到当前动画状态
    private Transform leftHandTrans; // 箭的位置是左手的位置
    private PlayerController playerController;
    private Vector3 shootDir;
    // private PlayerManager playerMng;
    
    void Start () {
        playerController = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
        leftHandTrans = transform.Find("ArrowSpawn");
    }
    
    void Update () {
        if (!isLocalPlayer)
        {
            return;
        }
        // 判断当前状态 是否是在地面
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            // 按下鼠标坐标攻击 攻击到鼠标点下的位置 控制发射方向 按下执行一次
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray= Camera.main.ScreenPointToRay(Input.mousePosition); // 将鼠标的点转换成射线
                RaycastHit hit; // 利用射线与环境做碰撞检测是否碰撞到任何东西
                bool isCollider = Physics.Raycast(ray, out hit);
                if (isCollider)
                {
                    Vector3 targetPoint = hit.point;
                    targetPoint.y = transform.position.y; // 方位的变化高度保持不变
                    //shootDir = targetPoint - transform.position;  // 得到射箭方向
                    //transform.rotation = Quaternion.LookRotation(shootDir);
                    transform.LookAt(targetPoint);
                    CmdAttack(transform);
                }
            }
        }
    }
    IEnumerator attackSync(Transform trans)
    {
        yield return new WaitForSeconds(1.15f);
        if (playerController.canSplite)
        {
            trans.Rotate(5, 0, 0);
            GameObject arrowleft = Instantiate(arrowPrefab, leftHandTrans.position, trans.rotation);
            ArrowController arrowController1 = arrowleft.GetComponent<ArrowController>();
            arrowController1.teamFrom = this.tag;
            arrowController1.damage = playerController.damage;
            trans.Rotate(-10, 0, 0);
            GameObject arrowright = Instantiate(arrowPrefab, leftHandTrans.position, trans.rotation);
            ArrowController arrowController2 = arrowright.GetComponent<ArrowController>();
            arrowController2.teamFrom = this.tag;
            arrowController2.damage = playerController.damage;
            NetworkServer.Spawn(arrowleft);
            NetworkServer.Spawn(arrowright);
            trans.Rotate(5, 0, 0);
        }
        
        GameObject arrow = Instantiate(arrowPrefab, leftHandTrans.position, trans.rotation);
        ArrowController arrowController =  arrow.GetComponent<ArrowController>();
        arrowController.teamFrom = this.tag;
        arrowController.damage = playerController.damage;
        NetworkServer.Spawn(arrow);
        yield break;
    }

    [Command]
    void CmdAttack(Transform trans)
    {
        RpcAttack();
        StartCoroutine(attackSync(trans));
        //GameObject arraw = Instantiate(arrowPrefab, leftHandTrans.position, trans.rotation);
        //NetworkServer.Spawn(arraw);
        //Invoke("CmdShoot", 0.1f);// 延迟调用，时转向和箭实例化同步
    }

    [ClientRpc]
    void RpcAttack()
    {
        anim.SetTrigger("Attack");
    }
}