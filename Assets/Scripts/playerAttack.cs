using UnityEngine;

public class playerAttack : MonoBehaviour
{
    public bool attackTrigger = false;
    private GameObject targetPlayer;
    PlayerMove playerMove;
    public GameObject Player; // 플레이어 오브젝트를 에디터에서 할당
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMove = Player.GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("attackPointBack"))
        {
            attackTrigger = true;
            targetPlayer = other.transform.root.gameObject;
            playerMove.GetOtherPlayer(targetPlayer);

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("attackPointBack"))
        {
            attackTrigger = false;
        }
    }


}
