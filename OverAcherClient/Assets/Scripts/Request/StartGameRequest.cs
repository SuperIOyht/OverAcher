using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class StartGameRequest : BaseRequest
{
    private RoomPanel roomPanel;

    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.StartGame;
        roomPanel = GetComponent<RoomPanel>();
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
        if (returnCode == ReturnCode.Fail)
        {
            roomPanel.OnStartResponse(returnCode, "", "");
        }
        if (returnCode == ReturnCode.Success)
        {
            roomPanel.OnStartResponse(returnCode, dataSplit[1], dataSplit[1] == "host" ? "0.0.0.0" : dataSplit[2]);
        }
    }
}