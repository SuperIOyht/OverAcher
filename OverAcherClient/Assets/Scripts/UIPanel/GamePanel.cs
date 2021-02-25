using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;
using Mirror;

public class GamePanel : BasePanel
{
    private Text blue; //红方队伍分数
    private Text red; //蓝方队伍分数

    //占点UI设置,image为在不同状态下UI的背景颜色，这部分变量在OnTriggerEnter()中用到
    private OccupyTarget2[] target; //数组中放的是五个占点塔的对象
    private Image[] imageReds;
    private Image[] imageBlues;
    private Image[] imageBgs;
    private Text[] textPercents;

    //游戏时间
    private Text gameTime;
    private int minute;
    private int second;
    //private float timeSpend = 0.0f;

    private Text timer;
    private NetworkManager manager;

    private int time = -1; // 默认不显示
    // private Button successBtn;
    // private Button failBtn;
    // private Button exitBtn;

    // private QuitBattleRequest quitBattleRequest;
    private void Start()
    {
        timer = transform.Find("Timer").GetComponent<Text>();
        timer.gameObject.SetActive(false);
        // successBtn = transform.Find("SuccessButton").GetComponent<Button>();
        // successBtn.onClick.AddListener(OnResultClick);
        // successBtn.gameObject.SetActive(false);
        // failBtn = transform.Find("FailButton").GetComponent<Button>();
        // failBtn.onClick.AddListener(OnResultClick);
        // failBtn.gameObject.SetActive(false);
        // exitBtn = transform.Find("ExitButton").GetComponent<Button>();
        // exitBtn.onClick.AddListener(OnExitClick);
        // exitBtn.gameObject.SetActive(false);

        // quitBattleRequest = GetComponent<QuitBattleRequest>();
        GameObject network = GameObject.Find("NetworkManager");
        manager = network.GetComponent<NetworkManager>();

        //获取占点点位
        target = new OccupyTarget2[5];
        target[0] = GameObject.Find("OccupyTarget/Target1/Cube1").GetComponent<OccupyTarget2>();
        target[1] = GameObject.Find("OccupyTarget/Target2/Cube2").GetComponent<OccupyTarget2>();
        target[2] = GameObject.Find("OccupyTarget/Target3/Cube3").GetComponent<OccupyTarget2>();
        target[3] = GameObject.Find("OccupyTarget/Target4/Cube4").GetComponent<OccupyTarget2>();
        target[4] = GameObject.Find("OccupyTarget/Target5/Cube5").GetComponent<OccupyTarget2>();
        //gamePanel上的占点相关UI设置
        GameObject canvas = GameObject.Find("Canvas");
		blue=this.transform.Find("TextBlue").gameObject.GetComponent<Text>();
		red=this.transform.Find("TextRed").gameObject.GetComponent<Text>();
		gameTime=this.transform.Find("timeText").gameObject.GetComponent<Text>();

        imageReds = new Image[5];
        imageBlues = new Image[5];
        imageBgs = new Image[5];
        textPercents = new Text[5];
        string str1;
        string str2;
        string str3;
        string str4;
        for (int i = 0; i < 5; i++)
        {
            str1 = "TargetPanel/TextPercent" + (i + 1).ToString();
            str2 = str1 + "/ImageRed";
            str3 = str1 + "/ImageBlue";
            str4 = str1 + "/ImageBg";
            textPercents[i] = this.transform.Find(str1).gameObject.GetComponent<Text>();
            imageReds[i] = this.transform.Find(str2).gameObject.GetComponent<Image>();
            imageBlues[i] = this.transform.Find(str3).gameObject.GetComponent<Image>();
            imageBgs[i] = this.transform.Find(str4).gameObject.GetComponent<Image>();
            textPercents[i].gameObject.SetActive(false); //设置不可见
            imageReds[i].gameObject.SetActive(false);
            imageBlues[i].gameObject.SetActive(false);
            imageBgs[i].gameObject.SetActive(false);
        }
    }
    // public override void OnEnter()
    // {
    //     gameObject.SetActive(true);
    // }
    // public override void OnExit()
    // {
    //     successBtn.gameObject.SetActive(false);
    //     failBtn.gameObject.SetActive(false);
    //     exitBtn.gameObject.SetActive(false);
    //     gameObject.SetActive(false);
    // }

    //进入界面时滑板被激活
    public override void OnEnter()
    {
        base.OnEnter();
        EnterAnim();
    }

    // 当界面暂停 需要进行隐藏
    public override void OnPause()
    {
        HideAnim();
    }

    // 面板重新激活的时候
    public override void OnResume()
    {
        // 继续重新显示
        EnterAnim();
    }

    // 禁用 
    public override void OnExit()
    {
        // 移除时隐藏
        HideAnim();
    }

    // 控制时间显示
    private void Update()
    {
        if (time > -1)
        {
            ShowTime(time);
            time = -1;
        }

        //timeSpend += Time.deltaTime;
        Debug.Log("update");
        for (int i = 0; i < 5; i++)
        {
            setTargetPanel(i);
        }
    }
    // private void OnResultClick()
    // {
    //     uiMng.PopPanel();
    //     uiMng.PopPanel();
    //     // facade.GameOver();
    // }
    // private void OnExitClick()
    // {
    //     // quitBattleRequest.SendRequest();
    // }
    // public void OnExitResponse()
    // {
    //     OnResultClick();
    // }

    // 异步显示时间
    public void ShowTimeSync(int time)
    {
        this.time = time;
    }

    public void ShowTime(int time)
    {
        // if (time == 3)
        // {
        //     exitBtn.gameObject.SetActive(true);
        // }
        timer.gameObject.SetActive(true);
        timer.text = time.ToString();
        timer.transform.localScale = Vector3.one; // 大小重置
        Color tempColor = timer.color;
        tempColor.a = 1; // 1显示
        timer.color = tempColor;
        // 从小到大 SetDelay设置延迟
        timer.transform.DOScale(2, 0.3f).SetDelay(0.3f);
        // 进行隐藏
        timer.DOFade(0, 0.3f).SetDelay(0.3f).OnComplete(() => timer.gameObject.SetActive(false));
        facade.PlayNormalSound(AudioManager.Sound_Alert);
    }

    public void ShowScore(int redScore, int blueScore)
    {
        //改变分数
        blue.text = string.Format("{0:D2}", blueScore);
        red.text = string.Format("{0:D2}", redScore);
    }

    public void ShowGameTime(double time)
    {
        //改变游戏时间
		minute = (int)time / 60;
        second = (int)time - minute * 60;
        gameTime.text = "Time: " + string.Format("{0:D2}:{1:D2}", minute, second);
    }

    public void GameOver()
    {
        // if (manager.mode == NetworkManagerMode.Host)
        // {
        //     manager.StopHost();
        // }
        // if (manager.mode == NetworkManagerMode.ClientOnly)
        // {
        //     manager.StopClient();
        // }
        //show Gameover gui
        uiMng.PushPanelSync(UIPanelType.End);
    }

    // public void OnGameOverResponse(ReturnCode returnCode)
    // {
    //     Button tempBtn = null;
    //     switch (returnCode)
    //     {
    //         case ReturnCode.Success:
    //             tempBtn = successBtn;
    //             break;
    //         case ReturnCode.Fail:
    //             tempBtn = failBtn;
    //             break;
    //     }
    //     tempBtn.gameObject.SetActive(true);
    //     tempBtn.transform.localScale = Vector3.zero;
    //     tempBtn.transform.DOScale(1, 0.5f);
    // }

    private void EnterAnim()
    {
        // 进入时启用自身
        gameObject.SetActive(true);
        // 先设置成0，通过descale渐变成1 0.3s
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.3f);
        // 面板从外面进来 局部位置 结束位置屏幕正中间
        transform.localPosition = new Vector3(1000, 0, 0);
        transform.DOLocalMove(Vector3.zero, 0.3f);
    }

    private void HideAnim()
    {
        transform.DOScale(0, 0.3f);
        transform.DOLocalMoveX(1000, 0.3f).OnComplete(() => gameObject.SetActive(false));
    }

    //显示占点状态
    private void setTargetPanel(int n)
    {
        Debug.Log("update occupation info");
        Debug.Log(target[n].occupied_state);
        if (target[n].occupied_state == 0 && !target[n].getRed_occupy() && !target[n].getBlue_occupy())
        {
            textPercents[n].gameObject.SetActive(false);
        }
        else if (target[n].occupied_state == 1)
        {
            textPercents[n].gameObject.SetActive(true);
            imageReds[n].gameObject.SetActive(true);
            imageBlues[n].gameObject.SetActive(false);
            imageBgs[n].gameObject.SetActive(false);
            textPercents[n].text = null;
			if(target[n].getBlue_occupy()){
				int percent = target[n].Percent;
                textPercents[n].text = percent.ToString() + "%";
                textPercents[n].color = new Color(0 / 255f, 110 / 255f, 255 / 255f, 255 / 255f);
                imageReds[n].gameObject.SetActive(false);
                imageBlues[n].gameObject.SetActive(false);
                imageBgs[n].gameObject.SetActive(true);
			}
        }
        else if (target[n].occupied_state == 2)
        {
            textPercents[n].gameObject.SetActive(true);
            imageReds[n].gameObject.SetActive(false);
            imageBlues[n].gameObject.SetActive(true);
            imageBgs[n].gameObject.SetActive(false);
            textPercents[n].text = null;
			if(target[n].getRed_occupy()){
				int percent = target[n].Percent;
                textPercents[n].text = percent.ToString() + "%";
                textPercents[n].color = new Color(200 / 255f, 0 / 255f, 0 / 255f, 255 / 255f);
                imageReds[n].gameObject.SetActive(false);
                imageBlues[n].gameObject.SetActive(false);
                imageBgs[n].gameObject.SetActive(true);
			}
        }
        else
        {
            textPercents[n].gameObject.SetActive(true);
            if (target[n].getRed_occupy())
            {
                int percent = target[n].Percent;
                textPercents[n].text = percent.ToString() + "%";
                textPercents[n].color = new Color(200 / 255f, 0 / 255f, 0 / 255f, 255 / 255f);
                imageReds[n].gameObject.SetActive(false);
                imageBlues[n].gameObject.SetActive(false);
                imageBgs[n].gameObject.SetActive(true);
            }
            else if (target[n].getBlue_occupy())
            {
                int percent = target[n].Percent;
                textPercents[n].text = percent.ToString() + "%";
                textPercents[n].color = new Color(0 / 255f, 110 / 255f, 255 / 255f, 255 / 255f);
                imageReds[n].gameObject.SetActive(false);
                imageBlues[n].gameObject.SetActive(false);
                imageBgs[n].gameObject.SetActive(true);
            }
        }
    }
}