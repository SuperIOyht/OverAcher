using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class HealthPill : NetworkBehaviour
{
    public int refreashTime = 10;
    public int maxHealthPoint = 50;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
            player.health = player.health + maxHealthPoint > 100 ? 100 : player.health + maxHealthPoint;
            NetworkServer.Destroy(gameObject);
        }
    }
}
