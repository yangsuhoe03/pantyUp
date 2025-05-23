using UnityEngine;
public class PlayerMove: MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    bool atkTrigger;
    private Rigidbody rb;
    private bool isGrounded;
    playerAttack playerAttack;
    GameObject SocketManager;
    private Vector3 lastSentPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SocketManager = GameObject.Find("SocketManager");
        playerAttack = GetComponentInChildren<playerAttack>();

        lastSentPosition = transform.position;
    }

    void Update()
    {

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 velocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);
        rb.linearVelocity = velocity;


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;  
        }
        if (Input.GetMouseButtonDown(0)) 
        {
            if (playerAttack.attackTrigger == true)
            {
                Debug.Log("공격 성공");
            }
            else 
            {

                Debug.Log("공격 실패"); 
            }

        }


        if (Vector3.Distance(transform.position, lastSentPosition) > 0.05f)
        {
            if (SocketManager != null)
            {

                string pos = $"{transform.position.x},{transform.position.y},{transform.position.z}";
                SocketManager.GetComponent<SocketManager>().SendPlayerPosition(pos);
                lastSentPosition = transform.position;
            }
        }

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        
         
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }

    }
}
