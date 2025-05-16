using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using MiniJSON; // MiniJSON 사용
public class SocketManager : MonoBehaviour
{
    public GameObject myPlayerPrefab;
    public GameObject otherPlayerPrefab;

    private Dictionary<string, GameObject> playerDict = new Dictionary<string, GameObject>();
    private string myId;

    [DllImport("__Internal")]
    private static extern void ConnectToSocket();

    [DllImport("__Internal")]
    private static extern void SendPosToServer(string pos);

    void Start()
    {
        gameObject.name = "SocketManager";
#if !UNITY_EDITOR && UNITY_WEBGL
        ConnectToSocket();
#endif
    }

    public void ReceivePos(string pos)
    {
        // 위치 동기화는 이후 구현
        Debug.Log("위치 수신: " + pos);
    }

    public void MakePlayer(string playersJson)
    {

    }

    public void SetMyId(string id)
    {
        Debug.Log("내 ID 설정됨: " + id);
        myId = id;
    }

}
