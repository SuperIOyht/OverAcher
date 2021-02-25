﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SwordDamage : NetworkBehaviour
{
    public int refreashTime = 10;
    public int DamageIncrease = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 1f);
    }
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), refreashTime);
    }

    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TeamRed" || other.tag == "TeamBlue")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.damage += DamageIncrease;
            player.resetEffect();
            player.canHitFireArrow = true;
            NetworkServer.Destroy(gameObject);
        }
        
    }
}