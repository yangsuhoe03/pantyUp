
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 15f;
    Vector3 currentVelocity = Vector3.zero;
    public float jumpForce = 5f;
    private bool jumpRequested = false;
    private float jumpDelay = 0.5f; // 딜레이 시간 (숙이는 시간)
    private Rigidbody rb;
    private bool isGrounded;
    playerAttack playerAttack;
    public GameObject attackPointFront, attackPointBack;
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
    Vector3 pantypos;
    bool dead = false;
    public GameObject cam;
    CameraMove cameraMove;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SocketManager = GameObject.Find("SocketManager");
        SocketManagerScript = SocketManager.GetComponent<SocketManager>();
        playerAttack = GetComponentInChildren<playerAttack>();

        lastSentPosition = transform.position;
        player_Anim = GetComponent<player_anim>();

        pantypos = new Vector3(0, -0.1237817f, -0.07895534f);
        cam = Camera.main.gameObject;
        cameraMove = cam.GetComponent<CameraMove>();
    }
    public string GetMYID()
    {

        string myID = SocketManagerScript.GetMySocketID();
        return myID;


    }

    void Update()
    {
        Debug.Log(rb.linearVelocity.y);
        Move();
        Jump();
        PredictLanding();

        if (Input.GetMouseButtonDown(0) && !dead && Cursor.lockState == CursorLockMode.Locked)
        {
            if (isAttack == false && isGrounded && attackSuccess == false)// 공격 상태가 아니고 땅에 있을 때만 공격 가능
            {
                isAttack = true; // 공격 시 이동 멈춤
                Invoke("AttackEnd", 0.5f); // 0.5초 후에 공격 종료 함수 호출
                player_Anim.GrabStart();
                Invoke("AttackCheck", 0.5f);
            }


        }
        if (player_Anim.grabsuccess == true && attackSuccess)
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
    public void Jump()
    {
        if (dead || Cursor.lockState != CursorLockMode.Locked) return;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isAttack && !jumpRequested)
        {
            Debug.Log("jump 했습니다~~~~~~~");
            jumpRequested = true;
            player_Anim.Jumpup();
            Invoke("PerformJump", jumpDelay);
        }
    }
    void PerformJump()
    {
        if (isGrounded) // 아직도 땅에 있을 때만 점프
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
        jumpRequested = false;
    }
    public void Move()
    {
        if (dead || Cursor.lockState != CursorLockMode.Locked) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 inputDir = transform.right * moveX + transform.forward * moveZ;
        float currentYVelocity = rb.linearVelocity.y;

        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        Vector3 targetHorizontalVelocity = new Vector3(inputDir.x * moveSpeed, 0, inputDir.z * moveSpeed);


        if (inputDir.magnitude > 0.01f)
        {
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetHorizontalVelocity, acceleration * Time.deltaTime);
        }
        else
        {
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        currentVelocity = new Vector3(horizontalVelocity.x, currentYVelocity, horizontalVelocity.z);
        if (isAttack)
        {
            currentVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            moveZ = 0;
        }
        rb.linearVelocity = currentVelocity;

        // 애니메이션 처리
        if (moveZ > 0 && !player_Anim.running)
        {
            player_Anim.Move(1);
        }
        else if (moveZ == 0 && (player_Anim.running || player_Anim.walkingbackward || player_Anim.walking) && moveX == 0)
        {
            player_Anim.Move(0);
        }
        else if ((moveZ < 0 && !player_Anim.walkingbackward) || (moveX != 0 && moveZ == 0 && !player_Anim.walking))
        {
            player_Anim.Move(-1);
        }

        //transform.Rotate(Vector3.up * cameraMove.mouseX);
    }
    public void AttackCheck()
    {
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
    }
    public void IsWedgied()
    {
        Debug.Log("공격 받음");
        isAttacked = true; // 공격을 받았음을 표시
        //GetComponent<Renderer>().material.color = Color.blue;
    }
    public void Death(GameObject otherp)
    {
        dead = true;
        transform.position = otherp.transform.position + Vector3.forward;
        transform.rotation = otherp.transform.rotation;
        player_Anim.Death();
        attackPointFront.SetActive(false);
        attackPointBack.SetActive(false);
        Invoke("Disappear", 3f);
        Invoke("RigidbodyFreezeOff", 1f);
        Invoke("Respawn", 7f);
    }
    public void Respawn()
    {
        dead = false;
        RigidbodyFreeze();
        player_Anim.Respawn();
        attackPointFront.SetActive(true);
        attackPointBack.SetActive(true);
    }
    void RigidbodyFreeze()
    {   
        transform.rotation = new Quaternion(0, 0, 0, 0);
        rb.freezeRotation = true;
    }
    void RigidbodyFreezeOff()
    {
        rb.freezeRotation = false;
    }
    void Disappear()
    {
        player_Anim.Disappear();
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
    public void SetPantypos()
    {
        playerPanty.transform.localPosition = pantypos;
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
            wedgieTime = wedgieTime + Time.deltaTime; // 팬티 공격 성공 시 시간 증가
            //Debug.Log("팬티 공격 성공 시간: " + wedgieTime);
            if (isAttacked == false)
            {
                //otherPlayer.GetComponent<OtherPlayer>().BeingAttacked();//한번만실행
                isAttacked = true;
            }
            // 다른 플레이어에게 공격 알림
            if (wedgieTime >= 30f) // 10초 이상 지속되면
            {
                wedgieTime = 0f; // 시간 초기화
                attackSuccess = false; // 공격 성공 상태 초기화

                isAttacked = false;

                player_Anim.Kill();

                isAttack = true; // 킬 도중 멈추기 위해 
                Invoke("AttackEnd", 0.5f); // 0.5초 후에 공격 종료 함수 호출

                otherPlayer.GetComponent<OtherPlayer>().SetPantypos();

                //GetComponent<Renderer>().material.color = Color.white; // 색상 원래대로 복원
                SocketManagerScript.AttackSuccess(attacked); // 공격 성공 전송
                otherPlayerPanty = null; // 다른 플레이어 오브젝트 초기화
                otherPlayerID = null;
                otherPlayer = null;
                
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("pantyDistance") && attackSuccess == true)
        {
            otherPlayer.GetComponent<OtherPlayer>().SetPantypos();
            Debug.Log("팬티 끊어짐");
            wedgieTime = 0f; // 시간 초기화
            attackSuccess = false; // 공격 성공 상태 초기화
            otherPlayerPanty = null; // 다른 플레이어 오브젝트 초기화
            otherPlayerID = null;
            otherPlayer = null;


            
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

            //player_Anim.Landing(isGrounded);

            //if (rb.linearVelocity.y < 0) player_Anim.Falling();
        }


    }
    void PredictLanding()
    {
        if (isGrounded || jumpRequested || rb.linearVelocity.y > -0.01f)
        {
            player_Anim.Landing(false);
            return;
        }

        float checkDistance = 0.5f; // 땅까지의 예측 거리 (조절 가능)
        Vector3 boxSize = new Vector3(0.5f, 0.1f, 0.5f); // 플레이어의 발 아래 영역 (플레이어 크기에 맞게 조절)
        Vector3 origin = transform.position + Vector3.up * 0.1f; // 조금 위에서부터 캐스트 시작
        Vector3 direction = Vector3.down;

        RaycastHit hit;

        if (Physics.BoxCast(origin, boxSize * 0.5f, direction, out hit, Quaternion.identity, checkDistance, LayerMask.GetMask("Ground")))
        {
            player_Anim.Landing(true); // 미리 착지 준비
            Debug.Log("landing 했습니다~~~~~~~");
        }
        else
        {
            //player_Anim.Falling(); // 아직 땅이 없으면 떨어지는 상태 유지
        }
    }
}
