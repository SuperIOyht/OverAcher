using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OccupyTarget2 : NetworkBehaviour
{
    private GameController controller;
    [SyncVar] public int occupied_state = 0; //0代表无人占领，1代表红队占领，2代表蓝队占领
    [SyncVar] public float add_score_time; //占领完一个点后每隔一段时间加的分
    [SyncVar] public float judge_time; //占领一个点需要的时间
    [SyncVar] public int occupy_score; //占领据点一段时间后要加的分数
    [SyncVar] private int red_in = 0; //统计红队在点内的人数
    [SyncVar] private int blue_in = 0; //统计蓝队在店内的人数
    [SyncVar] private float red_time = 0; //红队开始占点时的时间
    [SyncVar] private float blue_time = 0; //蓝队开始占点时的时间
    [SyncVar] private bool red_occupy = false; //用来标记红队是否正在占据一个点了
    [SyncVar] private bool blue_occupy = false; //用来标记蓝队是否正在占据一个点了
    [SyncVar] private float red_occupy_time = 0; //红队占领成功点后的时间
    [SyncVar] private float blue_occupy_time = 0; //蓝队占领成功点后的时间

    [SyncVar] public int Percent;

    [ServerCallback]
    void OnTriggerEnter(Collider other) //开始碰撞
    {
        //根据tag判断人数的增加
        if (other.tag == "TeamRed")
        {
            red_in += 1;
        }

        if (other.tag == "TeamBlue")
        {
            blue_in += 1;
        }
    }

    [ServerCallback]
    void OnTriggerExit(Collider other) //结束碰撞
    {
        //根据tag判断人数的减少
        if (other.tag == "TeamRed")
        {
            red_in -= 1;
        }

        if (other.tag == "TeamBlue")
        {
            blue_in -= 1;
        }
    }

    [Server]
    void Add_score() //加分
    {
        //如果系统时间减去某队开始占领据点时的时间大于判定间隔就加分
        //并且占据时间要大于0，代表该队在点内
        if (Time.time - red_occupy_time > add_score_time && occupied_state == 1) //红队加分
        {
            controller.addRedScore(occupy_score); //加分
            red_occupy_time += add_score_time;
        }

        if (Time.time - blue_occupy_time > add_score_time && occupied_state == 2) //蓝队加分
        {
            controller.addBlueScore(occupy_score); //加分
            blue_occupy_time += add_score_time;
        }
    }

    [Server]
    void judge() //判断占点状态
    {
        // controller.test();
        if (red_in > 0 && blue_in == 0 && occupied_state != 1) //红队在占领
        {
            if (red_occupy == false)
            {
                //说明红队之前没有处在占点状态中，将red_time设置为当下
                blue_occupy = false;
                blue_time = -1;
                red_occupy = true;
                red_time = Time.time;
            }
        }
        else if (red_in == 0 && blue_in > 0 && occupied_state != 2) //蓝队加分
        {
            if (blue_occupy == false)
            {
                //说明蓝队之前没有处在占点状态中，将red_time设置为当下
                red_occupy = false;
                red_time = -1;
                blue_occupy = true;
                blue_time = Time.time;
            }
        }
        else //两队都在点里或不在点里
        {
            //重置占领状态
            red_occupy = false;
            blue_occupy = false;
            //将时间设为负数，加分的计算就进行不了
            red_time = -1;
            blue_time = -1;
        }

        //判断是否该改变占点状态
        if (Time.time - red_time > judge_time && red_time > 0)
        {
            occupied_state = 1;
            red_occupy_time = Time.time;
            controller.addRedScore(occupy_score);
        }

        if (Time.time - blue_time > judge_time && blue_time > 0)
        {
            occupied_state = 2;
            blue_occupy_time = Time.time;
            controller.addBlueScore(occupy_score);
        }

        if (getRed_occupy())
        {
            Percent = (int) ((Time.time - getRed_time()) / judge_time * 100);
        }
        else if (getBlue_occupy())
        {
            Percent = (int) ((Time.time - getBlue_time()) / judge_time * 100);
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

        controller.test();
    }

    // Update is called once per frame
    [Server]
    void Update()
    {
        judge(); //每帧都判定占点状态
        Add_score(); //每帧都判定是否加分
    }

    public bool getRed_occupy()
    {
        return red_occupy;
    }

    public bool getBlue_occupy()
    {
        return blue_occupy;
    }

    public float getRed_time()
    {
        return red_time;
    }

    public float getBlue_time()
    {
        return blue_time;
    }
}