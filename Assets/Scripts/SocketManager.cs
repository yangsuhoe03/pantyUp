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
    string nickName;
    string myRoomName;
    float gameTimer = 600.0f;
    string allPlayerStatus;
    string[] roomInPlayerIds;
    private GameObject uiManager;

    public GameObject[] itemPoints; //아이템 스폰 위치
    public GameObject itemPrefab; //아이템 프리팹

    //List<string> playerList = new List<string>();
    Dictionary<string, GameObject> playerDict = new Dictionary<string, GameObject>();

    [DllImport("__Internal")]
    private static extern void ConnectToSocket();


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

    [DllImport("__Internal")]
    private static extern void SendMakePlayers(string playerID);

    [DllImport("__Internal")]
    private static extern void JoinRandomRoom(string playerId);

    [DllImport("__Internal")]
    private static extern void SendPlayerGetItem(string mySocketID);

    private float itemTestTimer = 0f;
    private float itemTestInterval = 10f; // 10초

    void Awake()
    {
        //Debug.Log("SocketManager Awake called");
#if !UNITY_EDITOR && UNITY_WEBGL
        ConnectToSocket();
#endif
    }
    void Start()
    {
        
        myPlayer.GetComponent<PlayerMove>();
        
        uiManager = GameObject.Find("UIManager");
        
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager를 찾을 수 없습니다!");
        }
        //Debug.Log(gameObject.name);
        gameObject.name = "SocketManager";
        //nickName = "myNickNameIssss";//이거 들어가면 설정하기(닉네임)

        //Instantiate(otherPlayer, new Vector3(1, 1, 1), Quaternion.identity);

    }
    public void SetMySocketID(string id)
    {
        mySocketID = id;
        Debug.Log(mySocketID);
        // 소켓 ID를 받자마자 랜덤 방 참가 요청
#if !UNITY_EDITOR && UNITY_WEBGL
        JoinRandomRoom(id);
#endif
    }

    public void SetPlayerNickname(string newNickname)
    {
        nickName = newNickname;
        string myStatus = $"{GetMySocketID()},{nickName}";
#if !UNITY_EDITOR && UNITY_WEBGL
        SendMyNickName(myStatus);
#endif
        // 닉네임 설정 후 플레이어 생성
        //CreatePlayers();
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


    public void SendGetItem(string mySocketID)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        SendPlayerGetItem(mySocketID);
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

    public void OnJoinedRoom(string roomName)
    {
        Debug.Log($"방 참가 성공: {roomName}");
        myRoomName = roomName;
    }
    public void OnRoomPlayerList(string roomInPlayers)
    {
        Debug.Log($"방 플레이어 목록: {roomInPlayers}");
        string[] newRoomPlayerIds = roomInPlayers.Split(',');


        roomInPlayerIds = newRoomPlayerIds;

    }
    private void CreatePlayers()
    {
        //타이머 정보 보내기
    
        Debug.Log(roomInPlayerIds);
        if (roomInPlayerIds == null) return;

        // 새로운 플레이어 생성
        foreach (string id in roomInPlayerIds)
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

        // 디버그 로그
        foreach (KeyValuePair<string, GameObject> entry in playerDict)
        {
            Debug.Log($"[playerDict] ID: {entry.Key}, Object Name: {entry.Value.name}");
        }
    }


    // public void MakePlayer(string playerIDs)
    // {
    //     string[] ids = playerIDs.Split(',');

    //     foreach (string id in ids)
    //     {
    //         if (!playerDict.ContainsKey(id))
    //         {
    //             if (id != GetMySocketID())
    //             {//내가 아닌 플레이어일 때
    //                 GameObject enemy = Instantiate(otherPlayer, new Vector3(1, 1, 1), Quaternion.identity);
    //                 enemy.GetComponent<OtherPlayer>().SetPlayerID(id);
    //                 playerDict.Add(id, enemy);
    //             }
    //             else
    //             {//내가 플레이어일 때
    //                 GameObject isMine;
    //                 isMine = GameObject.Find("Player");
    //                 playerDict.Add(id, isMine);
    //             }
    //         }
    //     }
    //     foreach (KeyValuePair<string, GameObject> entry in playerDict)
    //     {
    //         Debug.Log($"[playerDict] ID: {entry.Key}, Object Name: {entry.Value.name}");
    //     }

    //     //string myStatus = $"{GetMySocketID()},{nickName}";
    //     //SendMyName(myStatus);
    //     if (scoreManager != null)
    //     {
    //         //scoreManager.UpdatePlayerNickname(GetMySocketID(), nickName);
    //     }
    // }
    // void Update(){
    //     GameObject sun = GameObject.Find("Sun");
    //     if (sun != null)
    //     {
    //         // gameTimer가 600에서 0으로 줄어들 때 x축 각도가 180도에서 0도로 회전하도록 설정
    //         float startAngle = 180f;
    //         float endAngle = 0f;
    //         float totalTime = 600f;
    //         float t = Mathf.Clamp01(gameTimer / totalTime); // 600~0 -> 1~0
    //         float sunAngle = Mathf.Lerp(startAngle, endAngle, 1 - t); // 600일 때 180, 0일 때 0

    //         sun.transform.rotation = Quaternion.Euler(
    //             sunAngle, // x축 각도
    //             0f,       // y축 각도(고정)
    //             0f        // z축 각도(고정)
    //         );
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Sun 오브젝트를 찾을 수 없습니다. Directional Light의 이름이 'Sun'인지 확인하세요.");
    //     }
    // }

    public void ReceiveTimeSync(string time)
    {
        Debug.Log($"타이머 동기화: {time}");
        gameTimer = float.Parse(time);
        Debug.Log(gameTimer);
        uiManager.GetComponent<UIManager>().UpdateTimerDisplay(gameTimer);
        // 햇빛 방향(주광) 바꾸기 기능 추가
        // "Sun"이라는 이름의 Directional Light 오브젝트가 있다고 가정
        GameObject sun = GameObject.Find("Sun");
        if (sun != null)
        {
            // gameTimer가 600에서 0으로 줄어들 때 x축 각도가 180도에서 0도로 회전하도록 설정
            float startAngle = 180f;
            float endAngle = 0f;
            float totalTime = 600f;
            float t = Mathf.Clamp01(gameTimer / totalTime); // 600~0 -> 1~0
            float sunAngle = Mathf.Lerp(startAngle, endAngle, 1 - t); // 600일 때 180, 0일 때 0

            sun.transform.rotation = Quaternion.Euler(
                sunAngle, // x축 각도
                0f,       // y축 각도(고정)
                0f        // z축 각도(고정)
            );
        }
        else
        {
            Debug.LogWarning("Sun 오브젝트를 찾을 수 없습니다. Directional Light의 이름이 'Sun'인지 확인하세요.");
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
            //Debug.LogWarning($"ID {playerID}찾을 수 없음");
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
            myPlayer.GetComponent<PlayerMove>().Wedgied(attacker.GetComponent<OtherPlayer>().playerRightHand);
        }
        else if (playerDict.ContainsKey(attackerID) && playerDict.ContainsKey(targetID))//여기서는 공격자와 타겟 둘다 내가 아닐 때 실행(공격자면, 프론트에서 그냥 실행)
        {
            //attacker.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f);
            //target.GetComponent<Renderer>().material.color = Color.black;

            // 공격자의 OtherPlayer 컴포넌트에서 팬티 잡기 실행
            OtherPlayer attackerOtherPlayer = attacker.GetComponent<OtherPlayer>();
            if (attackerOtherPlayer != null)
            {
                attackerOtherPlayer.SetIsAttacked(true);
                // 타겟의 팬티를 공격자의 손으로 이동
                attackerOtherPlayer.otherPlayerPanty = target.GetComponent<OtherPlayer>().playerPanty;
            }
            target.GetComponent<OtherPlayer>().isWedgied = true;
        }
    }
    public void ReceiveSucceseAttack(string attacks)
    {
        Debug.Log($"{attacks}2 최종 공격 성공");
        string[] ids = attacks.Split(',');
        if (ids.Length != 2) return;

        string attackerID = ids[0];
        string targetID = ids[1];
        GameObject attacker = playerDict[attackerID];
        GameObject target = playerDict[targetID];
        if (targetID == GetMySocketID())//내가 공격을 당하면
        {
            Debug.Log("you died");
            if (playerDict[attackerID] != null)
            {
                myPlayer.GetComponent<PlayerMove>().Death(attacker);
                attacker.GetComponent<OtherPlayer>().SetAnimation("9"); //공격실패(손 내림)
            }
        }
        else if (playerDict.ContainsKey(attackerID) && playerDict.ContainsKey(targetID))//여기서는 공격자와 타겟 둘다 내가 아닐 때 실행(공격자면, 프론트에서 그냥 실행)
        {


            Debug.Log($"{attackerID}가 점수를 1 얻음 {targetID}는 죽음");

            Debug.Log($"{attackerID}가 팬티를 뺏음 {targetID}는 죽음");
            target.GetComponent<OtherPlayer>().SetIsAttacked(false);
            target.GetComponent<OtherPlayer>().SetPantypos();
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
            myPlayer.GetComponent<PlayerMove>().SetPantypos();
        }
        else if (playerDict.ContainsKey(attackerID) && playerDict.ContainsKey(targetID))//여기서는 공격자와 타겟 둘다 내가 아닐 때 실행(공격자면, 프론트에서 그냥 실행)
        {

            GameObject attacker = playerDict[attackerID];
            GameObject target = playerDict[targetID];
            Debug.Log($"{attackerID}가 공격 실패 {targetID}는 팬티 끊킴");
            target.GetComponent<OtherPlayer>().SetIsAttacked(false);
            target.GetComponent<OtherPlayer>().SetPantypos();
            
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
            //Debug.Log("wwqwqwqwqwqwqwqwwww");
        }
    }


    public void ReceiveUpdatePlayerStatus(string data)
    {
        Debug.Log($"플레이어 상태 업데이트: {data}");
        allPlayerStatus = data;
        if (scoreManager != null)
        {
            scoreManager.ReceiveUpdatePlayerStatus(data);
        }
    }

    public void ReceiveItemSpawn(string spawnString)
    {
        Debug.Log($"아이템 스폰: {spawnString}");
        // "0,1,0,0,1,0" → int 배열로 변환
        int[] spawnArray = Array.ConvertAll(spawnString.Split(','), int.Parse);
        Debug.Log($"아이템 스폰 배열: {spawnArray}");
        // spawnArray[0]~[5]를 보고 1인 곳에만 아이템 스폰
        for (int i = 0; i < 6; i++)
        {
            if (spawnArray[i] == 1)
            {
                if (itemPoints[i].GetComponent<isActive>().isItemActive == false){//중복방지
                    GameObject item = Instantiate(itemPrefab, itemPoints[i].transform.position, Quaternion.identity);
                    item.GetComponent<ScoreUpItem>().spawnPoint = itemPoints[i];
                    itemPoints[i].GetComponent<isActive>().isItemActive = true;
                }
    

            }
        }
    }

    public void ReceiveGameOver()
    {
        Debug.Log("게임 종료");
        uiManager.GetComponent<UIManager>().GameEnd();
    }

    void Update()
    {
        
            //ReceiveItemSpawn("1,1,1,1,1,1");

    }

}