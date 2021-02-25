using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagement : NetworkManager
{
    public struct playerInfo : NetworkMessage
    {
        public GameObject player;
        public string teamColor;
        public bool teamtype;
        //false as red,true as blue 
    }
    public Transform blueStartPosition;
    public Transform redStartPosition;
    public override void OnStartServer()
    {
        base.OnStartServer();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("start client");
    }

    //client connect to server ,called on server
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Transform start;
        playerInfo playerInfo = new playerInfo();
        Debug.Log(numPlayers);
        if (numPlayers % 2 == 0)
        {
            playerInfo.teamColor = "Archer Warrior Red";
            playerInfo.teamtype = false;
            start = redStartPosition;
        }
        else
        {
            playerInfo.teamColor = "Archer Warrior Blue";
            playerInfo.teamtype = true;
            start = blueStartPosition;
        }
        GameObject player = Instantiate(spawnPrefabs.Find(prefab => prefab.name == playerInfo.teamColor), start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);
        playerInfo.player = player;
        conn.Send(playerInfo);
        Debug.Log("send player info to client");
    }
    ////called on client when connect to server
    //public override void OnClientConnect(NetworkConnection conn)
    //{
    //    base.OnClientConnect(conn);
    //    Debug.Log("connect");
    //}
    public void replacePlayer(NetworkConnection conn, GameObject prefab, Transform spawnPos)
    {
        GameObject oldPlayer = conn.identity.gameObject;
        NetworkServer.Destroy(oldPlayer);
        StartCoroutine(respawnPlayer(conn, prefab, spawnPos));
    }
    IEnumerator respawnPlayer(NetworkConnection conn,GameObject prefab, Transform spawnPos)
    {
        //respawn time set
        yield return new WaitForSeconds(5f);
        NetworkServer.ReplacePlayerForConnection(conn, Instantiate(prefab, spawnPos.position, spawnPos.rotation));
        yield break;
    }
}
