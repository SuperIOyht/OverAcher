using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class GameOverRequest : BaseRequest
{
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.GameOver;
        base.Awake();
    }

    public override void SendRequest()
    {
        base.SendRequest("r");
    }

    // 成功
    public override void OnResponse(string data)
    {
        Debug.Log(data);
        string[] dataSplit = data.Split(',');
        ReturnCode returnCode = (ReturnCode) int.Parse(dataSplit[0]);
    }
}