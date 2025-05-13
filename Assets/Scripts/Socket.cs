using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class SocketManager : MonoBehaviour
{

    // JavaScript와 통신 (JS 함수 정의 필요)
    [DllImport("__Internal")]
    private static extern void ConnectToSocket();

    [DllImport("__Internal")]
    private static extern void SendMessageToServer(string message);

    void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        ConnectToSocket();
#endif
    }

    public void OnSendButtonClick()
    {
        string msg = "inputField.text";
#if !UNITY_EDITOR && UNITY_WEBGL
        SendMessageToServer(msg);
#endif
        Log($" 보냄: {msg}");
    }

    // JavaScript에서 호출할 함수
    public void ReceiveMessage(string msg)
    {
        Log($"받음: {msg}");
    }

    void Log(string message)
    {
        Debug.Log(message);
        // UI에 메시지 표시 (예: Text 컴포넌트 사용)
        // textComponent.text += message + "\n";
    }
}
