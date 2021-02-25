using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WarriorAnimsFREE;

public class HPController : MonoBehaviour
{
	public PlayerController playcontroller;
	public Slider playerHP;//slider控制人物血条
	
	//人物状态UI设置，包括护盾，冰冻，加速，中毒状态
	public Image shieldImage;//护盾状态
	public Image speedUpImage;//加速状态
	public Image poisonImage;//中毒状态
	public Image frozenImage;//冰冻状态
	//弓箭形态
	public Image ballArrowImage;//巨型弓箭
	public Image fireArrowImage;//火箭
	public Image poisonArrowImage;//毒箭
	public Image frozenArrowImage;//冰冻箭
	public Image splitArrowImage;//分裂箭
	
    // Start is called before the first frame update
    void Start()
    {
		//人物状态UI设置，包括护盾，冰冻，加速，中毒状态
		shieldImage.gameObject.SetActive(false);
		speedUpImage.gameObject.SetActive(false);
		poisonImage.gameObject.SetActive(false);
		frozenImage.gameObject.SetActive(false);
		//弓箭形态设置
		ballArrowImage.gameObject.SetActive(false);
		fireArrowImage.gameObject.SetActive(false);
		poisonArrowImage.gameObject.SetActive(false);
		frozenArrowImage.gameObject.SetActive(false);
		splitArrowImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //控制进度条的值与PlayerController中的health变量值相等
        playerHP.value=playcontroller.health;
		
		//人物状态UI设置，包括护盾，冰冻，加速，中毒状态
		if (!shieldImage.gameObject.activeSelf && playcontroller.haveShield){//护盾状态
			shieldImage.gameObject.SetActive(true);
		}else{
			if(shieldImage.gameObject.activeSelf && !playcontroller.haveShield){
				shieldImage.gameObject.SetActive(false);
			}
		}
		
		if (!speedUpImage.gameObject.activeSelf && playcontroller.isSpeedUp){//加速状态
			speedUpImage.gameObject.SetActive(true);
		}else{
			if(speedUpImage.gameObject.activeSelf && !playcontroller.isSpeedUp){
				speedUpImage.gameObject.SetActive(false);
			}
		}
		
		if (!poisonImage.gameObject.activeSelf && playcontroller.bePoisoned){//中毒状态
			poisonImage.gameObject.SetActive(true);
		}else{
			if(poisonImage.gameObject.activeSelf && !playcontroller.bePoisoned){
				poisonImage.gameObject.SetActive(false);
			}
		}
		
		if (!frozenImage.gameObject.activeSelf && playcontroller.beFrozen){//冰冻状态
			frozenImage.gameObject.SetActive(true);
		}else{
			if(frozenImage.gameObject.activeSelf && !playcontroller.beFrozen){
				frozenImage.gameObject.SetActive(false);
			}
		}
		
		//弓箭形态设置
		if (!ballArrowImage.gameObject.activeSelf && playcontroller.canHitBallArrow){//巨型弓箭
			ballArrowImage.gameObject.SetActive(true);
		}else{
			if(ballArrowImage.gameObject.activeSelf && !playcontroller.canHitBallArrow){
				ballArrowImage.gameObject.SetActive(false);
			}
		}
		
		if (!fireArrowImage.gameObject.activeSelf && playcontroller.canHitFireArrow){//火箭
			fireArrowImage.gameObject.SetActive(true);
		}else{
			if(fireArrowImage.gameObject.activeSelf && !playcontroller.canHitFireArrow){
				fireArrowImage.gameObject.SetActive(false);
			}
		}
		
		if (!poisonArrowImage.gameObject.activeSelf && playcontroller.canHitPoisonArrow){//毒箭
			poisonArrowImage.gameObject.SetActive(true);
		}else{
			if(poisonArrowImage.gameObject.activeSelf && !playcontroller.canHitPoisonArrow){
				poisonArrowImage.gameObject.SetActive(false);
			}
		}
		
		if (!frozenArrowImage.gameObject.activeSelf && playcontroller.canHitFrozenArrow){//冰冻箭
			frozenArrowImage.gameObject.SetActive(true);
		}else{
			if(frozenArrowImage.gameObject.activeSelf && !playcontroller.canHitFrozenArrow){
				frozenArrowImage.gameObject.SetActive(false);
			}
		}
		
		if (!splitArrowImage.gameObject.activeSelf && playcontroller.canSplite){//分裂箭
			splitArrowImage.gameObject.SetActive(true);
		}else{
			if(splitArrowImage.gameObject.activeSelf && !playcontroller.canSplite){
				splitArrowImage.gameObject.SetActive(false);
			}
		}
    }
}
