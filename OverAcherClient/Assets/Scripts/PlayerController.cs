using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WarriorAnimsFREE;
using Mirror;
using UnityEngine.UI;

public class PlayerController : WarriorControllerFREE
{
    private float SpeedDecrease = 0.8f;
    public GameObject playerExplo;
    [SyncVar]
    private bool teamtype;
    [SyncVar]
    public bool isDead = false;
    private GameObject player;
    private Transform playerRespawnPos;
    private NetworkManagement networkManagement;
    private GameController gameController;
	

	
	//人物血条的画布；
	private Canvas hp;
	
	/* //人物状态UI设置，包括护盾，冰冻，加速，中毒状态
	public Image shieldImage;//护盾状态
	public Image speedUpImage;//加速状态
	public Image poisonImage;//中毒状态
	public Image frozenImage;//冰冻状态
	//弓箭形态
	public Image ballArrowImage;//巨型弓箭
	public Image fireArrowImage;//火箭
	public Image poisonArrowImage;//毒箭
	public Image frozenArrowImage;//冰冻箭
	public Image splitArrowImage;//分裂箭 */

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        player = this.gameObject;
        GameObject.Find("Camera").GetComponent<CameraFollow>().player = this.gameObject;
		
		hp=player.transform.Find("Canvas_HP").GetComponent<Canvas>();
		hp.worldCamera=GameObject.Find("Camera").GetComponent<Camera>();//设置人物血条的相机
		
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        if(this.gameObject.tag == "TeamBlue")
        {
            gameController.teamtype = true;
            gameController.StartPosition = GameObject.Find("BlueSpawn");
        }
        else
        {
            gameController.teamtype = false;
            gameController.StartPosition = GameObject.Find("RedSpawn");
        }
        networkManagement = GameObject.Find("NetworkManager").GetComponent<NetworkManagement>();
        teamtype = GameObject.Find("GameController").GetComponent<GameController>().teamtype;
        if (teamtype)
        {
            //blue
            //playerPrefab = networkManagement.spawnPrefabs.Find(prefab => prefab.name == "Hunter_BLUE");
            playerRespawnPos = networkManagement.blueStartPosition;
        }
        else
        {
            //playerPrefab = networkManagement.spawnPrefabs.Find(prefab => prefab.name == "Hunter_RED");
            playerRespawnPos = networkManagement.redStartPosition;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForStart());
		
		/* //人物状态UI设置，包括护盾，冰冻，加速，中毒状态
		shieldImage.gameObject.SetActive(false);
		speedUpImage.gameObject.SetActive(false);
		poisonImage.gameObject.SetActive(false);
		frozenImage.gameObject.SetActive(false);
		//弓箭形态设置
		ballArrowImage.gameObject.SetActive(false);
		fireArrowImage.gameObject.SetActive(false);
		poisonArrowImage.gameObject.SetActive(false);
		frozenArrowImage.gameObject.SetActive(false);
		splitArrowImage.gameObject.SetActive(false); */
    }
    IEnumerator WaitForStart()
    {
        yield return new WaitForSeconds(3.0f);
        this.gameObject.GetComponent<PlayerAttack>().enabled = true;
        this.gameObject.GetComponent<PlayerMove>().enabled = true;
        yield break;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (!isLocalPlayer)
            return;
        if(health <= 0 && isDead == false)
        {
            isDead = true;
            if(teamtype == false)
            {
                gameController.addBlueScore(1);
            }
            else
            {
                gameController.addRedScore(1);
            }
            CmdDestoryPlayer();
            Debug.Log("Dead");
        }
		
		/* //人物状态UI设置，包括护盾，冰冻，加速，中毒状态
		if (!shieldImage.gameObject.activeSelf && haveShield){//护盾状态
			shieldImage.gameObject.SetActive(true);
		}else{
			if(shieldImage.gameObject.activeSelf && !haveShield){
				shieldImage.gameObject.SetActive(false);
			}
		}
		
		if (!speedUpImage.gameObject.activeSelf && isSpeedUp){//加速状态
			speedUpImage.gameObject.SetActive(true);
		}else{
			if(speedUpImage.gameObject.activeSelf && !isSpeedUp){
				speedUpImage.gameObject.SetActive(false);
			}
		}
		
		if (!poisonImage.gameObject.activeSelf && bePoisoned){//中毒状态
			poisonImage.gameObject.SetActive(true);
		}else{
			if(poisonImage.gameObject.activeSelf && !bePoisoned){
				poisonImage.gameObject.SetActive(false);
			}
		}
		
		if (!frozenImage.gameObject.activeSelf && beFrozen){//冰冻状态
			frozenImage.gameObject.SetActive(true);
		}else{
			if(frozenImage.gameObject.activeSelf && !beFrozen){
				frozenImage.gameObject.SetActive(false);
			}
		}
		
		//弓箭形态设置
		if (!ballArrowImage.gameObject.activeSelf && canHitBallArrow){//巨型弓箭
			ballArrowImage.gameObject.SetActive(true);
		}else{
			if(ballArrowImage.gameObject.activeSelf && !canHitBallArrow){
				ballArrowImage.gameObject.SetActive(false);
			}
		}
		
		if (!fireArrowImage.gameObject.activeSelf && canHitFireArrow){//火箭
			fireArrowImage.gameObject.SetActive(true);
		}else{
			if(fireArrowImage.gameObject.activeSelf && !canHitFireArrow){
				fireArrowImage.gameObject.SetActive(false);
			}
		}
		
		if (!poisonArrowImage.gameObject.activeSelf && canHitPoisonArrow){//毒箭
			poisonArrowImage.gameObject.SetActive(true);
		}else{
			if(poisonArrowImage.gameObject.activeSelf && !canHitPoisonArrow){
				poisonArrowImage.gameObject.SetActive(false);
			}
		}
		
		if (!frozenArrowImage.gameObject.activeSelf && canHitFrozenArrow){//冰冻箭
			frozenArrowImage.gameObject.SetActive(true);
		}else{
			if(frozenArrowImage.gameObject.activeSelf && !canHitFrozenArrow){
				frozenArrowImage.gameObject.SetActive(false);
			}
		}
		
		if (!splitArrowImage.gameObject.activeSelf && canSplite){//分裂箭
			splitArrowImage.gameObject.SetActive(true);
		}else{
			if(splitArrowImage.gameObject.activeSelf && !canSplite){
				splitArrowImage.gameObject.SetActive(false);
			}
		} */
    }
    public void PlayerFrozend()
    {
        this.warriorMovementController.walkSpeed *= SpeedDecrease;
        this.warriorMovementController.runSpeed *= SpeedDecrease;
        beFrozen = true;
        StartCoroutine(frozenDelay());
    }
    IEnumerator frozenDelay()
    {
        yield return new WaitForSeconds(5);
        beFrozen = false;
        warriorMovementController.walkSpeed = 4f;
        warriorMovementController.runSpeed = 6f;
        yield break;
    }
    public void playerPoisoned()
    {
        bePoisoned = true;
        StartCoroutine(poisonDelay());
    }
    IEnumerator poisonDelay()
    {
        float nextTime = Time.time;
        int i = 0;
        while (i < 5)
        {
            if (Time.time > nextTime)
            {
                health -= 2;
                i += 1;
                nextTime = Time.time + 1;
            }
        }
        //yield return new WaitForSeconds(1);
        //health -= 2;
        //for(int i = 0; i < 4; i++)
        //{
        //    new WaitForSeconds(1);
        //    health -= 2;
        //}
        bePoisoned = false;
        yield break;
    }
    [Command]
    void CmdDestoryPlayer()
    {
        GameObject temp = Instantiate(playerExplo, transform.position, transform.rotation);
        NetworkServer.Spawn(temp);
        gameController.CmdOnPlayerDead(player);
        gameController.destroyExplo(temp);
    }
    public void BeAttacked(float arrowDamage)
    {
        health = health - (arrowDamage - sheild);
    }
}
