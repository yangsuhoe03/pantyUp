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
    private ScoreManager scoreManager;
    //List<string> playerList = new List<string>();
    Dictionary<string, GameObject> playerDict = new Dictionary<string, GameObject>();

    [DllImport("__Internal")]
    private static extern void ConnectToSocket(string nickName);

    [DllImport("__Internal")]
    private static extern void SendPosToServer(string pos);

    [DllImport("__Internal")]           
    private static extern void SendAttackToServer(string attacked);
    [DllImport("__Internal")]
    private static extern void SendAnimToServer(string currentmove);

    [DllImport("__Internal")]
    private static extern void ScoreUp(string scoreData);

    [DllImport("__Internal")]
    private static extern void SendAttackToFaild(string attacked);


    [DllImport("__Internal")]
    private static extern void SendMyNickName(string nickName);

                    


    string nickName;
    void Start()
    {
        myPlayer.GetComponent<PlayerMove>();
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager를 찾을 수 없습니다!");
        }
        //Debug.Log(gameObject.name);
        gameObject.name = "SocketManager";
        //nickName = "myNickNameIssss";//이거 들어가면 설정하기(닉네임)
#if !UNITY_EDITOR && UNITY_WEBGL
        ConnectToSocket(nickName);
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

        public void SendMyName(string MyName)
    {
#if !UNITY_EDITOR && UNITY_WEBGL

    SendMyNickName(MyName);
#endif
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
    public void SendPlayerAnim(string currentAnim)
    {
#if !UNITY_EDITOR && UNITY_WEBGL

    SendAnimToServer(currentAnim);
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
            if (!playerDict.ContainsKey(id))
            {
                if (id != GetMySocketID())
                {//내가 아닌 플레이어일 때
                    GameObject enemy = Instantiate(otherPlayer, new Vector3(1, 1, 1), Quaternion.identity);
                    enemy.GetComponent<OtherPlayer>().SetPlayerID(id);
                    playerDict.Add(id, enemy);
                }
                else
                {//내가 플레이어일 때
                    GameObject isMine;
                    isMine = GameObject.Find("Player");
                    playerDict.Add(id, isMine);
                }
            }
        }
        foreach (KeyValuePair<string, GameObject> entry in playerDict)
        {
            Debug.Log($"[playerDict] ID: {entry.Key}, Object Name: {entry.Value.name}");
        }

        string myStatus = $"{GetMySocketID()},{nickName}";
        SendMyName(myStatus);
        if (scoreManager != null)
        {
            scoreManager.UpdatePlayerNickname(GetMySocketID(), nickName);
        }
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
            Debug.LogWarning($"ID {playerID}찾을 수 없음");
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
            //attacker.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f);
            //target.GetComponent<Renderer>().material.color = Color.black;
            
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
    public void ReceiveAnim(string anim)
    {
        string[] PlayerData = anim.Split(',');
        string playerID = PlayerData[0];

        if (playerDict.ContainsKey(playerID))
        {
            playerDict[playerID].GetComponent<OtherPlayer>().SetAnimation(PlayerData[1]);
        }
        else
        {
            Debug.Log("wwqwqwqwqwqwqwqwwww");
        }
    }
    public void ReceiveScoreUpdate(string scoreData)
    {
        Debug.Log($"점수 업데이트: {scoreData}");
        if (scoreManager != null)
        {
            scoreManager.UpdateScores(scoreData);
        }
    }

    public void ReceiveNickname(string data)
    {
        string[] parts = data.Split(',');
        if (parts.Length == 2)
        {
            string playerId = parts[0];
            string nickname = parts[1];
            if (scoreManager != null)
            {
                scoreManager.UpdatePlayerNickname(playerId, nickname);
            }
        }
    }

    public void SetPlayerNickname(string newNickname)
    {
        nickName = newNickname;
#if !UNITY_EDITOR && UNITY_WEBGL
        ConnectToSocket(nickName);
#endif
    }
}