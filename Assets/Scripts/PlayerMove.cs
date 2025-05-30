
using UnityEngine;
public class PlayerMove: MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded;
    playerAttack playerAttack;
    GameObject SocketManager;
    SocketManager SocketManagerScript; // SocketManager ��ũ��Ʈ ����
    private Vector3 lastSentPosition;
    bool isAttack = false; // ���� ���� ����
    float rotationY = 0; // ���� Y�� ȸ�� ��
    bool attackSuccess = false; // ���� ���� ����
    float wedgieTime = 0;
    int score = 0; // ���� ����
    GameObject otherPlayer;
    string otherPlayerID; // �ٸ� �÷��̾��� ID
    string attacked;
    bool isAttacked = false; // ������ �޾Ҵ��� ����
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SocketManager = GameObject.Find("SocketManager");
        SocketManagerScript = SocketManager.GetComponent<SocketManager>();
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


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && isAttack == false)
        {

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;  
        }
        if (Input.GetMouseButtonDown(0)) 
        {
            if (isAttack == false && isGrounded && attackSuccess == false)// ���� ���°� �ƴϰ� ���� ���� ���� ���� ����
            {
                isAttack = true; // ���� �� �̵� ����
                Invoke("AttackEnd", 0.5f); // 0.5�� �Ŀ� ���� ���� �Լ� ȣ��
                if (playerAttack.attackTrigger == true)
                {
                    Debug.Log("���� ����");
                    attackSuccess = true; // ���� ���� ���·� ����
                    GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f);
                    attacked = $"{SocketManagerScript.GetMySocketID()},{otherPlayerID}"; // ������ ID�� ��� ID�� ��ǥ�� �����Ͽ� ����
                    SocketManagerScript.SendAttack(attacked); // ���� ����
                }
                else
                {

                    Debug.Log("���� ����");
                }
            }


        }
        if (isAttack == true)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }


        if (Vector3.Distance(transform.position, lastSentPosition) > 0.05f || Mathf.Abs(transform.rotation.y - rotationY) > 0.01f)
        {
            //Debug.Log("�÷��̾� ��ġ ���۵�: " + transform.position + ", ȸ��: " + transform.rotation.y);
            if (SocketManager != null)
            {
               
                string pos = $"{transform.position.x},{transform.position.y},{transform.position.z},{transform.eulerAngles.y}";
                SocketManagerScript.SendPlayerPosition(pos);
                lastSentPosition = transform.position;
                rotationY = transform.eulerAngles.y; // ���� Y�� ȸ�� �� ����
            }
        }

    }
    public void IsWedgied()
    {
        Debug.Log("���� ����");
        isAttacked = true; // ������ �޾����� ǥ��
        GetComponent<Renderer>().material.color = Color.blue; 
        
    }

    public void GetOtherPlayer(GameObject otherP)
    {
        otherPlayer = otherP; // �ٸ� �÷��̾� ������Ʈ ����
        otherPlayerID = otherP.GetComponent<OtherPlayer>().playerID; // �ٸ� �÷��̾��� ID ����
    }
    void AttackEnd()
    {
        isAttack = false; // ���� ���¸� false�� �����Ͽ� �̵� �����ϰ� ��
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
    private void OnTriggerEnter(Collider other)
    {

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("pantyDistance") && attackSuccess == true)
        {
            wedgieTime = wedgieTime + Time.deltaTime; // ��Ƽ ���� ���� �� �ð� ����
            //Debug.Log("��Ƽ ���� ���� �ð�: " + wedgieTime);
            if(isAttacked == false)
            {
                otherPlayer.GetComponent<OtherPlayer>().BeingAttacked();//�ѹ�������
                isAttacked = true;
            }
            // �ٸ� �÷��̾�� ���� �˸�
            if (wedgieTime >= 10f) // 10�� �̻� ���ӵǸ�
            {
                score += 1; // ���� ����
                Debug.Log("���� ����: " + score);
                wedgieTime = 0f; // �ð� �ʱ�ȭ
                attackSuccess = false; // ���� ���� ���� �ʱ�ȭ
                GetComponent<Renderer>().material.color = Color.white; // ���� ������� ����
                isAttacked = false; 
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("pantyDistance") && attackSuccess == true)
        {
            Debug.Log("��Ƽ ������");
            wedgieTime = 0f; // �ð� �ʱ�ȭ
            attackSuccess = false; // ���� ���� ���� �ʱ�ȭ
            otherPlayer = null; // �ٸ� �÷��̾� ������Ʈ �ʱ�ȭ
            GetComponent<Renderer>().material.color = Color.white;
            isAttacked = false; 
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
