
using Unity.VisualScripting;
using UnityEngine;
public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded;
    playerAttack playerAttack;
    GameObject SocketManager;
    SocketManager SocketManagerScript; // SocketManager 스크립트 참조
    player_anim player_Anim; //플레이어 애니메이션 스크립트
    private Vector3 lastSentPosition;
    bool isAttack = false; // 공격 상태 변수
    float rotationY = 0; // 현재 Y축 회전 값
    bool attackSuccess = false; // 공격 성공 여부
    float wedgieTime = 0;
    GameObject otherPlayer;
    GameObject otherPlayerPanty;
    string otherPlayerID; // 다른 플레이어의 ID
    string attacked;
    bool isAttacked = false; // 공격을 받았는지 여부
    public GameObject playerRightHand;
    public GameObject playerPanty;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SocketManager = GameObject.Find("SocketManager");
        SocketManagerScript = SocketManager.GetComponent<SocketManager>();
        playerAttack = GetComponentInChildren<playerAttack>();

        lastSentPosition = transform.position;
        player_Anim = GetComponent<player_anim>();


    }
    public string GetMYID(){

        string myID = SocketManagerScript.GetMySocketID();
        return myID;
        

    }

    void Update()
    {

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 velocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);
        rb.linearVelocity = velocity;

        //뛰기 애니메이션
        if (moveZ > 0 && !player_Anim.running)
        {
            player_Anim.Move(moveZ);
        }
        else if (moveZ == 0 && (player_Anim.running || player_Anim.walkingbackward))
        {
            player_Anim.Move(moveZ);
        }
        else if (moveZ < 0)
        {
            player_Anim.Move(moveZ);
        }


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && isAttack == false)
        {

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            player_Anim.Jumpup();
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (isAttack == false && isGrounded && attackSuccess == false)// 공격 상태가 아니고 땅에 있을 때만 공격 가능
            {
                isAttack = true; // 공격 시 이동 멈춤
                Invoke("AttackEnd", 0.5f); // 0.5초 후에 공격 종료 함수 호출
                if (playerAttack.attackTrigger == true)
                {
                    Debug.Log("공격 성공");
                    attackSuccess = true; // 공격 성공 상태로 변경
                    //GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f);
                    attacked = $"{SocketManagerScript.GetMySocketID()},{otherPlayerID}"; // 공격자 ID와 대상 ID를 쉼표로 구분하여 저장
                    SocketManagerScript.SendAttack(attacked); // 공격 전송

                    player_Anim.GrabSuccess(true);
                    

                    
                }
                else
                {
                    Debug.Log("공격 실패");
                    player_Anim.GrabSuccess(false);
                }

                player_Anim.GrabStart();
            }


        }
        if (isAttack == true)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

        }
        if ( player_Anim.grabsuccess == true )
        {
            if (otherPlayerPanty != null)
            {
                otherPlayerPanty.transform.position = playerRightHand.transform.position;                   
            }

        }


        if (Vector3.Distance(transform.position, lastSentPosition) > 0.05f || Mathf.Abs(transform.rotation.y - rotationY) > 0.01f)
        {
            //Debug.Log("플레이어 위치 전송됨: " + transform.position + ", 회전: " + transform.rotation.y);
            if (SocketManager != null)
            {

                string pos = $"{transform.position.x},{transform.position.y},{transform.position.z},{transform.eulerAngles.y}";
                SocketManagerScript.SendPlayerPosition(pos);
                lastSentPosition = transform.position;
                rotationY = transform.eulerAngles.y; // 현재 Y축 회전 값 저장
            }
        }

    }
    public void IsWedgied()
    {
        Debug.Log("공격 받음");
        isAttacked = true; // 공격을 받았음을 표시
        //GetComponent<Renderer>().material.color = Color.blue;

    }

    public void GetOtherPlayer(GameObject otherP)
    {
        otherPlayer = otherP; // 다른 플레이어 오브젝트 저장
        otherPlayerPanty = otherPlayer.GetComponent<OtherPlayer>().playerPanty;
        otherPlayerID = otherP.GetComponent<OtherPlayer>().playerID; // 다른 플레이어의 ID 저장
        
    }
    void AttackEnd()
    {
        isAttack = false; // 공격 상태를 false로 변경하여 이동 가능하게 함
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            player_Anim.Landing(isGrounded);
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
            wedgieTime = wedgieTime + Time.deltaTime; // 팬티 공격 성공 시 시간 증가
            //Debug.Log("팬티 공격 성공 시간: " + wedgieTime);
            if (isAttacked == false)
            {
                otherPlayer.GetComponent<OtherPlayer>().BeingAttacked();//한번만실행
                isAttacked = true;
            }
            // 다른 플레이어에게 공격 알림
            if (wedgieTime >= 10f) // 10초 이상 지속되면
            {
                wedgieTime = 0f; // 시간 초기화
                attackSuccess = false; // 공격 성공 상태 초기화

                isAttacked = false;

                player_Anim.Kill();
                //GetComponent<Renderer>().material.color = Color.white; // 색상 원래대로 복원
                SocketManagerScript.AttackSuccess(attacked); // 공격 성공 전송
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("pantyDistance") && attackSuccess == true)
        {
            Debug.Log("팬티 끊어짐");
            wedgieTime = 0f; // 시간 초기화
            attackSuccess = false; // 공격 성공 상태 초기화
            otherPlayer = null;
            otherPlayerPanty = null; // 다른 플레이어 오브젝트 초기화

            
            player_Anim.GrabSuccess(false);
            isAttacked = false;
            SocketManagerScript.AttackFaild(attacked); // 공격 실패 전송 
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;

            player_Anim.Landing(isGrounded);

            if(rb.linearVelocity.y < 0)player_Anim.Falling();
        }


    }
}
