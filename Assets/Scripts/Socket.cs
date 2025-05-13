using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class SocketManager : MonoBehaviour
{

    // JavaScript�� ��� (JS �Լ� ���� �ʿ�)
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
        Log($" ����: {msg}");
    }

    // JavaScript���� ȣ���� �Լ�
    public void ReceiveMessage(string msg)
    {
        Log($"����: {msg}");
    }

    void Log(string message)
    {
        Debug.Log(message);
        // UI�� �޽��� ǥ�� (��: Text ������Ʈ ���)
        // textComponent.text += message + "\n";
    }
}
