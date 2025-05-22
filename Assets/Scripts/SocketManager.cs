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
    Dictionary<string, GameObject> playerDict = new Dictionary<string, GameObject>();//���� ���� �� �̷��� ���
    // JavaScript�� ��� (JS �Լ� ���� �ʿ�)
    [DllImport("__Internal")]
    private static extern void ConnectToSocket();

    [DllImport("__Internal")]
    private static extern void SendPosToServer(string pos);
    void Start()
    {
        //Debug.Log(gameObject.name);
        gameObject.name = "SocketManager";//�̸��� �ٸ��뼭 �ٲ���
#if !UNITY_EDITOR && UNITY_WEBGL
        ConnectToSocket();
#endif
        Instantiate(testObj, new Vector3(1, 1, 1), Quaternion.identity);

    }

    public void SetMySocketID(string id)
    {
        mySocketID = id;
        Debug.Log("�� ���� ID �����: " + mySocketID);
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
        string[] ids = playerIDs.Split(','); // ��ǥ�� ���� ID�� �� �� ����

        foreach (string id in ids)
        {
            if (id == GetMySocketID()) continue; // �ڽ� ����

            if (!playerDict.ContainsKey(id))
            {
                GameObject enemy = Instantiate(otherPlayer, new Vector3(1, 1, 1), Quaternion.identity);
                enemy.GetComponent<OtherPlayer>().SetPlayerID(id);
                playerDict.Add(id, enemy);
                Debug.Log($"������ �÷��̾� ID: {id}");
            }
        }

        //enemy.GetComponent<OtherPlayer>().SetPlayerID(playerIDs);


    }

    public void ReceivePos(string data)
    {
        Debug.Log("���� ������: " + data);

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
            Debug.LogWarning($"�÷��̾� ID {playerID}�� ã�� �� �����ϴ�.");
        }
        //GameObject.Find(playerID).GetComponent<OtherPlayer>().SetPosition(pos);//�Ź� �̸����� ã���� ���� ���ϰ� �ɰ���


    }


}