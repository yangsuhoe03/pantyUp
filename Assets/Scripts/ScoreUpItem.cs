using UnityEngine;

public class ScoreUpItem : MonoBehaviour
{
    public GameObject spawnPoint; // SocketManager에서 할당
    GameObject socketManager;
    string mySocketID;
    
    // 회전 속도 설정
    public float rotationSpeed = 100f;
    // 위아래 이동 관련 변수
    public float floatAmplitude = 0.2f; // 이동 높이
    public float floatFrequency = 2f;   // 이동 속도
    private float startY;

    private void Start()
    {
        socketManager = GameObject.Find("SocketManager");
        mySocketID = socketManager.GetComponent<SocketManager>().GetMySocketID();
        startY = transform.position.y;
    }

    private void Update()
    {
        // 오브젝트를 Y축을 중심으로 계속 회전
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        // 위아래로 부드럽게 이동
        Vector3 pos = transform.position;
        pos.y = startY + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject myPlayer = socketManager.GetComponent<SocketManager>().myPlayer;
            if (other.gameObject == myPlayer)
            {
                socketManager.GetComponent<SocketManager>().SendGetItem(mySocketID);
                GameObject.Find("Player").GetComponent<PlayerSoundManager>().PlayGetPoint();
            }
            if (spawnPoint != null)
            {
                spawnPoint.GetComponent<isActive>().isItemActive = false;
            }
                
            Destroy(gameObject);
        }
    }
}