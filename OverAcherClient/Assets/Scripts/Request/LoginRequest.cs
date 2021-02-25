using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Common;
using UnityEngine;

public class LoginRequest : BaseRequest
{
    private LoginPanel loginPanel;

    // 重写父类Awake
    public override void Awake()
    {
        // 初始化在Base.Awake()之前
        requestCode = RequestCode.User;
        actionCode = ActionCode.Login;
        loginPanel = GetComponent<LoginPanel>();
        base.Awake();
    }

    // 发起登录请求 重写方法 数据的组拼
    public void SendRequest(string username, string password)
    {
        string ipv4_ips = getIpAddress();
        string data = username + "," + password + "," + ipv4_ips;
        base.SendRequest(data);
    }

    // 重写OnResponse data是服务器端发送的响应数据 数据的解析 具体处理交给LoginPanel 
    public override void OnResponse(string data)
    {
        // 逗号分割数据
        string[] strs = data.Split(',');

        // data先转int，然后整体强制转换枚举类型
        ReturnCode returnCode = (ReturnCode) int.Parse(strs[0]);

        // 通过LoginPanel调用响应方法
        loginPanel.OnLoginResponse(returnCode);

        if (returnCode == ReturnCode.Success)
        {
            string username = strs[1];
            int totalCount = int.Parse(strs[2]);
            int winCount = int.Parse(strs[3]);
            UserData ud = new UserData(username, totalCount, winCount);
            facade.SetUserData(ud);
        }
    }

    /// <summary>
    /// 获取本机ip地址
    /// </summary>
    /// <returns>ip地址</returns>
    String getIpAddress()
    {
        string localIP;
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("192.168.0.1", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint.Address.ToString();
        }

        return localIP;
    }
}