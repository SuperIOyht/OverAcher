using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemRespawn : NetworkBehaviour
{
    public int refreashTime = 15;
    public List<GameObject> items;
    // Start is called before the first frame update
    void Start()
    {
        startRespawn();
    }
    [Server]
    private void startRespawn()
    {
        StartCoroutine(RespawnItem());
    }
    IEnumerator RespawnItem()
    {
        //yield return new WaitForSeconds(refreashTime);
        while (true)
        {
            int number = Random.Range(0, 7);
            GameObject temp = Instantiate(items[number], transform.position, transform.rotation);
            NetworkServer.Spawn(temp);
            yield return new WaitForSeconds(refreashTime);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
