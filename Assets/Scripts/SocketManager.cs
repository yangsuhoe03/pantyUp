using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class SocketManager : MonoBehaviour
{

    public GameObject otherPlayer;
    public GameObject testObj;
    private string mySocketID;
    //List<string> playerList = new List<string>();
    Dictionary<string, GameObject> playerDict = new Dictionary<string, GameObject>();//성능 저하 시 이렇게 사용
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

    public void SetMySocketID(string id)
    {
        mySocketID = id;
        Debug.Log("내 소켓 ID 저장됨: " + mySocketID);
    }

    public string GetMySocketID()
    {
        return mySocketID;
    }


    public void SendPlayerPosition(string pos)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
    SendPosToServer(pos);
#endif
    }

    public void MakePlayer(string playerIDs)
    {
        string[] ids = playerIDs.Split(','); // 쉼표로 여러 ID가 올 수 있음

        foreach (string id in ids)
        {
            if (id == GetMySocketID()) continue; // 자신 제외

            if (!playerDict.ContainsKey(id))
            {
                GameObject enemy = Instantiate(otherPlayer, new Vector3(1, 1, 1), Quaternion.identity);
                enemy.GetComponent<OtherPlayer>().SetPlayerID(id);
                playerDict.Add(id, enemy);
                Debug.Log($"생성된 플레이어 ID: {id}");
            }
        }

        //enemy.GetComponent<OtherPlayer>().SetPlayerID(playerIDs);


    }

    public void ReceivePos(string data)
    {
        Debug.Log("받은 데이터: " + data);

        string[] PlayerData = data.Split(':');
        string playerID = PlayerData[0];

        if (playerDict.ContainsKey(playerID))
        {
            string[] PlayerPos = PlayerData[1].Split(',');
            float x = float.Parse(PlayerPos[0]);
            float y = float.Parse(PlayerPos[1]);
            float z = float.Parse(PlayerPos[2]);
            Vector3 pos = new Vector3(x, y, z);
            playerDict[playerID].GetComponent<OtherPlayer>().SetPosition(pos);
        }
        else
        {
            Debug.LogWarning($"플레이어 ID {playerID}를 찾을 수 없습니다.");
        }
        //GameObject.Find(playerID).GetComponent<OtherPlayer>().SetPosition(pos);//매번 이름으로 찾으면 성능 저하가 심각함


    }


}