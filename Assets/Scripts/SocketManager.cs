using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class SocketManager : MonoBehaviour
{

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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("");

        }
    } 


    public void SendPlayerPosition(string pos)
    {

        Debug.Log("1");
#if !UNITY_EDITOR && UNITY_WEBGL
    SendPosToServer(pos);
#endif
    }

    public void ReceivePos(string pos) 
    {
        Debug.Log($"5:  {pos}");

        Instantiate(testObj, new Vector3(1,1,1), Quaternion.identity);

        
    }


}
