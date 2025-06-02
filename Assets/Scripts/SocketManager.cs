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
    public GameObject myPlayer;
    //List<string> playerList = new List<string>();
    Dictionary<string, GameObject> playerDict = new Dictionary<string, GameObject>();

    [DllImport("__Internal")]
    private static extern void ConnectToSocket();

    [DllImport("__Internal")]
    private static extern void SendPosToServer(string pos);

    [DllImport("__Internal")]
    private static extern void SendAttackToServer(string attacked);

    void Start()
    {
         myPlayer.GetComponent<PlayerMove>();
        //Debug.Log(gameObject.name);
        gameObject.name = "SocketManager";
#if !UNITY_EDITOR && UNITY_WEBGL
        ConnectToSocket();
#endif
        //Instantiate(otherPlayer, new Vector3(1, 1, 1), Quaternion.identity);
    }

    public void SetMySocketID(string id)
    {
        mySocketID = id;

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
    public void SendAttack(string attacked)
    {
#if !UNITY_EDITOR && UNITY_WEBGL

    SendAttackToServer(attacked);
#endif
    }

    public void MakePlayer(string playerIDs)
    {
        string[] ids = playerIDs.Split(','); 

        foreach (string id in ids)
        {
            if (id == GetMySocketID()) continue; 

            if (!playerDict.ContainsKey(id))
            {
                GameObject enemy = Instantiate(otherPlayer, new Vector3(1, 1, 1), Quaternion.identity);
                enemy.GetComponent<OtherPlayer>().SetPlayerID(id);
                playerDict.Add(id, enemy);
               
            }
        }

        //enemy.GetComponent<OtherPlayer>().SetPlayerID(playerIDs);


    }

    public void ReceivePos(string data)
    {


        string[] PlayerData = data.Split(':');
        string playerID = PlayerData[0];

        if (playerDict.ContainsKey(playerID))
        {
            string[] PlayerPos = PlayerData[1].Split(',');
            float x = float.Parse(PlayerPos[0]);
            float y = float.Parse(PlayerPos[1]);
            float z = float.Parse(PlayerPos[2]);
            float rotationY = float.Parse(PlayerPos[3]);
            Vector3 pos = new Vector3(x, y, z);
            playerDict[playerID].GetComponent<OtherPlayer>().SetPosition(pos, rotationY);
        }
        else
        {
            Debug.LogWarning($" {playerID}");
        }
        //GameObject.Find(playerID).GetComponent<OtherPlayer>().SetPosition(pos);

    }
    public void Attacking(string attacks)
    {
        string[] ids = attacks.Split(',');
        if (ids.Length != 2) return;

        string attackerID = ids[0];
        string targetID = ids[1];

        foreach (var kvp in playerDict)
        {
            Debug.Log($"[playerDict] Key: {kvp.Key}, Value: {kvp.Value.name}");
        }
        Debug.Log($"[Wedgie] attackerID: {attackerID}, targetID: {targetID}");


        if (playerDict.ContainsKey(attackerID) && playerDict.ContainsKey(targetID))
        {


            GameObject attacker = playerDict[attackerID];
            GameObject target = playerDict[targetID];


            attacker.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f);
            target.GetComponent<Renderer>().material.color = Color.black;

            if (targetID == GetMySocketID())
            {

                myPlayer.GetComponent<PlayerMove>().IsWedgied();

            }

        }
        else
        {
            Debug.Log("wqwer");
        }

    }

}