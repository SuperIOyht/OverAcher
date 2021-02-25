using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ArrowController : NetworkBehaviour
{
    public float destroyAfter = 5;
    private Rigidbody rigidBody;
    public string teamFrom;
    public float damage;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.AddForce(transform.right * (-speed));
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfter);
    }

    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
    [ServerCallback]
    void OnTriggerEnter(Collider co)
    {
        if(co.tag == "BlueArrow" || co.tag == "RedArrow" || co.tag == "Arrow")
        {
            return;
        }
        if (teamFrom == "TeamRed")
        {
            if (co.tag == "TeamBlue")
            {
                PlayerController playerController = co.gameObject.GetComponent<PlayerController>();
                playerController.BeAttacked(this.damage);
            }
        }
        else if (teamFrom == "TeamBlue")
        {
            if (co.tag == "TeamRed")
            {
                PlayerController playerController = co.gameObject.GetComponent<PlayerController>();
                playerController.BeAttacked(this.damage);
            }
        }
        NetworkServer.Destroy(gameObject);
        ////ceshi
        //PlayerController playerController = co.gameObject.GetComponent<PlayerController>();
        //playerController.BeAttacked(this.damage);
        //NetworkServer.Destroy(gameObject);
    }
}