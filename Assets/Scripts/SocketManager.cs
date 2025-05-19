using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class SocketManager : MonoBehaviour
{

    public GameObject otherPlayer;
    public GameObject testObj;

    // JavaScript와 통신 (JS 함수 정의 필요)
    [DllImport("__Internal")]
    private static extern void ConnectToSocket();

    [DllImport("__Internal")]
    private static extern void SendPosToServer(string pos);
    void Start()
    {
        //Debug.Log(gameObject.name);
        gameObject.name = "SocketManager";//이름이 다르대서 바꿔줌
#if !UNITY_EDITOR && UNITY_WEBGL
        ConnectToSocket();
#endif
        Instantiate(testObj, new Vector3(1, 1, 1), Quaternion.identity);

    }


    public void SendPlayerPosition(string pos)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
    SendPosToServer(pos);
#endif
    }

    public void MakePlayer(string Players)
    {

        Debug.Log("hello" + Players);

    }

    public void ReceivePos(string data)
    {
        //Debug.Log($"4 :{pos}");

        //Debug.Log("받은 위치 데이터: " + data);

        string[] PlayerData = data.Split(':');
        Debug.Log($"1 :{PlayerData[0]}");
        Debug.Log($"2 :{PlayerData[1]}");
        string[] PlayerPos = PlayerData[1].Split(',');
        float x = float.Parse(PlayerPos[0]);
        float y = float.Parse(PlayerPos[1]);
        float z = float.Parse(PlayerPos[2]);
        Debug.Log(x);
        Debug.Log(y);
        Debug.Log(z);
        Vector3 pos = new Vector3(x, y, z);
        PlayerPosUpdate(PlayerData[0], pos);




    }
    void PlayerPosUpdate(string PlayerID, Vector3 PlayerPos)
    {

    }

}