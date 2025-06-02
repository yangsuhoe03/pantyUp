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

    [DllImport("__Internal")]
    private static extern void ScoreUp(string scoreData);

    [DllImport("__Internal")]
    private static extern void SendAttackToFaild(string attacked);

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
        Debug.Log(mySocketID);

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

    public void AttackSuccess(string scoreData)
    {
#if !UNITY_EDITOR && UNITY_WEBGL

    ScoreUp(scoreData);
#endif
    }

    public void AttackFaild(string attacked)
    {
#if !UNITY_EDITOR && UNITY_WEBGL

    SendAttackToFaild(attacked);
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
    public void ReceiveAttacking(string attacks)
    {
        Debug.Log($"{attacks}1 공격 성공"); 
        string[] ids = attacks.Split(',');
        if (ids.Length != 2) return;

        string attackerID = ids[0];
        string targetID = ids[1];

        GameObject attacker = playerDict[attackerID];
        GameObject target = playerDict[targetID];
        if (targetID == GetMySocketID())//내가 공격을 당하면
        {
            Debug.Log("wedgied");

            myPlayer.GetComponent<PlayerMove>().IsWedgied();



        }

        if (playerDict.ContainsKey(attackerID) && playerDict.ContainsKey(targetID))//여기서는 공격자와 타겟 둘다 내가 아닐 때 실행(공격자면, 프론트에서 그냥 실행)
        {
            attacker.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f);
            target.GetComponent<Renderer>().material.color = Color.black;
        }
    }
    public void ReceiveSucceseAttack(string attacks)
    {
        Debug.Log($"{attacks}2 최종 공격 성공"); 
        string[] ids = attacks.Split(',');
        if (ids.Length != 2) return;

        string attackerID = ids[0];
        string targetID = ids[1];
        if (targetID == GetMySocketID())//내가 공격을 당하면
        {

            Debug.Log("you died");

        }
        if (playerDict.ContainsKey(attackerID) && playerDict.ContainsKey(targetID))//여기서는 공격자와 타겟 둘다 내가 아닐 때 실행(공격자면, 프론트에서 그냥 실행)
        {


            GameObject attacker = playerDict[attackerID];
            GameObject target = playerDict[targetID];
            target.GetComponent<Renderer>().material.color = Color.red;


            Debug.Log($"{attackerID}가 점수를 1 얻음 {targetID}는 죽음");



        }
        
        
    }
    public void ReceiveFaildAttack(string attacks)
    {
        Debug.Log($"{attacks}3 공격 실패");
        string[] ids = attacks.Split(',');
        if (ids.Length != 2) return;

        string attackerID = ids[0];
        string targetID = ids[1];
        if (targetID == GetMySocketID())//내가 공격을 당하면
        {

            Debug.Log("팬티 끊킴..!");

        }
        if (playerDict.ContainsKey(attackerID) && playerDict.ContainsKey(targetID))//여기서는 공격자와 타겟 둘다 내가 아닐 때 실행(공격자면, 프론트에서 그냥 실행)
        {


            GameObject attacker = playerDict[attackerID];
            GameObject target = playerDict[targetID];
            Debug.Log($"{attackerID}가 공격 실패 {targetID}는 팬티 끊킴");



        }
        
        
    }

}