using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OccupyTarget : NetworkBehaviour
{
    private GameController controller;
    public float judge_time;//占领一个点需要的时间
    public int occupy_score;//占领据点一段时间后要加的分数
    private int red_in = 0;//统计红队在点内的人数
    private int blue_in = 0;//统计蓝队在店内的人数
    private float red_time = 0;//红队开始占点时的时间
    private float blue_time = 0;//蓝队开始占点时的时间
    private bool red_occupy = false;//用来标记红队是否正在占据一个点了
    private bool blue_occupy = false;//用来标记蓝队是否正在占据一个点了

    [ServerCallback]
    void OnTriggerEnter(Collider other)//开始碰撞
    {
        //根据tag判断人数的增加
        if (other.tag == "red")
        {
            red_in += 1;
        }
        if (other.tag == "blue")
        {
            blue_in += 1;
        }
    }

    [ServerCallback]
    void OnTriggerExit(Collider other)//结束碰撞
    {
        //根据tag判断人数的减少
        if (other.tag == "red")
        {
            red_in -= 1;
        }
        if (other.tag == "blue")
        {
            blue_in -= 1;
        }
    }

    void Add_score()//加分
    {
        //如果系统时间减去某队开始占领据点时的时间大于判定间隔就加分
        //并且占据时间要大于0，代表该队在点内
        if(Time.time - red_time > judge_time && red_time > 0)//红队加分
        {
            controller.addRedScore(occupy_score);//加分
            red_occupy = false;//重置占领状态，以便于判定占点状态时重置占点开始的时间
        }
        if (Time.time - blue_time > judge_time && blue_time > 0)//蓝队加分
        {
            controller.addBlueScore(occupy_score);//加分
            blue_occupy = false;//重置占领状态，以便于判定占点状态时重置占点开始的时间
        }
    }


    void judge()//判断占点状态
    {
        if (red_in > 0 && blue_in == 0)//红队加分
        {
            if(red_occupy == false)//说明红队之前没有处在占点状态中，将red_time设置为当下
            {
                red_occupy = true;
                red_time = Time.time;
            }
        }
        else if (red_in == 0 && blue_in > 0)//蓝队加分
        {
            if (blue_occupy == false)//说明蓝队之前没有处在占点状态中，将red_time设置为当下
            {
                blue_occupy = true;
                blue_time = Time.time;
            }
        }
        else//两队都在点里或不在点里
        {
            //重置占领状态
            red_occupy = false;
            blue_occupy = false;
            //将时间设为负数，加分的计算就进行不了
            red_time = -1;
            blue_time = -1;
        }
    }
    [Server]
    void Start()
    {
        //找到GameController
        GameObject tmp = GameObject.FindGameObjectWithTag("GameController");
        controller = tmp.GetComponent<GameController>();
        if (controller == null)
        {
            Debug.LogError("Unable to find the GameController script");
        }
    }

    [Server]
    // Update is called once per frame
    void Update()
    {
        judge();//每帧都判定占点状态
        Add_score();//每帧都判定是否加分
    }
}
