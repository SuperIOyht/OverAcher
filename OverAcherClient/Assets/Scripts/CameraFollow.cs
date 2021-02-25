using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class CameraFollow : NetworkBehaviour
{
    public Vector3 offset;
    public GameObject player;
    private void Start()
    {
        offset = new Vector3(0, 14, -14);
    }
    private void Update()
    {
        this.transform.position = player.transform.position + offset;
    }
}