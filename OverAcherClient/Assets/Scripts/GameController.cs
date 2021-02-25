using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameController : NetworkBehaviour
{
    [SyncVar] public int endSecond = 180; //结束时间
    [SyncVar] public int win_score; //目标分数
    [SyncVar] public int red_score = 0;
    [SyncVar] public int blue_score = 0;
    [SyncVar] private bool isGameEnd = false;
    [SyncVar] private string WinTeam;
    public GamePanel gamePanel;

    public float respawnTime = 2.0f;

    public bool teamtype;

    //false as red,true as blue 
    public GameObject MainCamera;
    public GameObject SecCamera;

    public GameObject StartPosition;

    // Start is called before the first frame update
    public override void OnStartClient()
    {
        MainCamera.SetActive(false);
    }

    public override void OnStartServer()
    {
        MainCamera.SetActive(false);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        MainCamera.SetActive(true);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        MainCamera.SetActive(true);
    }

    void Start()
    {
        //gamePanel = GameObject.Find("Canvas").GetComponentInChildren<GamePanel>();
        GameObject canvas = GameObject.Find("Canvas");
        gamePanel = GameObject.Find("GamePanel(Clone)").GetComponent<GamePanel>();
        StartCoroutine(UpdateUITime());
    }

    IEnumerator UpdateUITime()
    {
        yield return new WaitForSeconds(1.0f);
        while (true)
        {
            Debug.Log(NetworkTime.time);
            gamePanel.ShowGameTime(NetworkTime.time);
            yield return new WaitForSeconds(1.0f);
        }
    }

    // Update is called once per frame
    private bool hasEnded = false;

    void Update()
    {
        if (isGameEnd && !hasEnded)
        {
            //游戏结束
            gamePanel.GameOver();
            hasEnded = true;
        }

        testGameEnd();
    }

    [Server]
    void testGameEnd()
    {
        if (NetworkTime.time >= endSecond)
        {
            isGameEnd = true;
            WinTeam = red_score > blue_score ? "RedTeam" : "BlueTeam";
        }

        if (red_score >= win_score)
        {
            isGameEnd = true;
            WinTeam = "RedTeam";
            //红色赢了,游戏结束
        }
        else if (blue_score >= win_score)
        {
            isGameEnd = true;
            WinTeam = "BlueTeam";
            //蓝色赢了,游戏结束
        }
    }

    public void addRedScore(int score)
    {
        red_score += score;
        RpcUIShowScore(red_score, blue_score);
    }

    public void addBlueScore(int score)
    {
        blue_score += score;
        RpcUIShowScore(red_score, blue_score);
    }

    [ClientRpc]
    private void RpcUIShowScore(int red_score, int blue_score)
    {
        gamePanel.ShowScore(red_score, blue_score);
    }

    [ClientRpc]
    public void CmdOnPlayerDead(GameObject player)
    {
        Debug.Log("OnDead");
        resetPlayer(player);
        player.SetActive(false);
        StartCoroutine(respawnPlayer(player));
    }

    IEnumerator respawnPlayer(GameObject player)
    {
        yield return new WaitForSeconds(respawnTime);
        player.transform.SetPositionAndRotation(StartPosition.transform.position, StartPosition.transform.rotation);
        player.SetActive(true);
        yield break;
    }

    [ClientRpc]
    public void destroyExplo(GameObject explo)
    {
        StartCoroutine(DestoryDelayCoroutine(explo));
    }

    IEnumerator DestoryDelayCoroutine(GameObject temp)
    {
        yield return new WaitForSeconds(1.0f);
        NetworkServer.Destroy(temp);
        Debug.Log("Explo");
        yield break;
    }

    private void resetPlayer(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.health = 100;
        playerController.sheild = 0;
        playerController.damage = 10;
        playerController.isSpeedUp = false;
        playerController.canSplite = false;
        playerController.canHitFrozenArrow = false;
        playerController.canHitPoisonArrow = false;
        playerController.canHitBallArrow = false;
        playerController.haveShield = false;
        playerController.bePoisoned = false;
        playerController.beFrozen = false;
        playerController.canHitFireArrow = false;
        playerController.isDead = false;
        playerController.warriorMovementController.walkSpeed = 4f;
        playerController.warriorMovementController.runSpeed = 6f;
    }

    public void test()
    {
        //
    }
}