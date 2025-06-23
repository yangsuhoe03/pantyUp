using UnityEngine;

public class ScoreUpItem : MonoBehaviour
{
    public GameObject spawnPoint; // SocketManager에서 할당
    GameObject socketManager;
    string mySocketID;

    private void Start()
    {
        socketManager = GameObject.Find("SocketManager");
        mySocketID = socketManager.GetComponent<SocketManager>().GetMySocketID();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject myPlayer = socketManager.GetComponent<SocketManager>().myPlayer;
            if (other.gameObject == myPlayer)
            {
                socketManager.GetComponent<SocketManager>().SendGetItem(mySocketID);
            }
            if (spawnPoint != null)
            {
                spawnPoint.GetComponent<isActive>().isItemActive = false;
            }
                
            Destroy(gameObject);
        }
    }
}