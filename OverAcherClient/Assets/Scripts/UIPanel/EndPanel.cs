using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Common;

public class EndPanel : BasePanel
{
	private Text resultText;
	private Button exitButton;
	private GameController controller;
	private bool endGame=true;
	
    // Start is called before the first frame update
    void Start()
    {
		controller=GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		
        resultText = transform.Find("resultText").GetComponent<Text>();
		exitButton = transform.Find("exitButton").GetComponent<Button>();
		exitButton.onClick.AddListener(OnCloseClick);
    }

    // Update is called once per frame
    void Update()
    {
    }
	
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
	
	// 处理exitButton按钮的点击
    private void OnCloseClick()
    {
        PlayClikSound(); // 播放点击声音
        // 关闭时进行移除面板 触发OnExit()
		while(!uiMng.StackIsEmpty()){
			uiMng.PopPanel();
		}
		uiMng.OnInit();
    }
	
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
}
