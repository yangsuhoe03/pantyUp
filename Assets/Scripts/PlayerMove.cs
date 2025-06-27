
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float acceleration = 10f;
    public float deceleration = 15f;
    Vector3 currentVelocity = Vector3.zero;
    public float jumpForce = 5f;
    private bool jumpRequested = false;
    private float jumpDelay = 0.0f; // 딜레이 시간 (숙이는 시간)
    private Rigidbody rb;
    public bool isGrounded;
    playerAttack playerAttack;
    public GameObject attackPointFront, attackPointBack;
    GameObject SocketManager;
    SocketManager SocketManagerScript; // SocketManager 스크립트 참조
    UIManager UIManagerScript;
    player_Ground player_ground;
    player_anim player_Anim; //플레이어 애니메이션 스크립트
    private Vector3 lastSentPosition;
    bool isAttack = false; // 공격 상태 변수
    float rotationY = 0; // 현재 Y축 회전 값
    bool attackSuccess = false; // 공격 성공 여부
    float wedgieTime = 0;
    GameObject otherPlayer;
    GameObject otherPlayerPanty;
    GameObject otherPlayerRighthand;
    string otherPlayerID; // 다른 플레이어의 ID
    string attacked;
    bool isAttacked = false; // 공격을 받았는지 여부
    public GameObject playerRightHand;
    public GameObject playerPanty;
    Vector3 pantypos;
    public bool dead = false;
    public GameObject cam;
    CameraMove cameraMove;
    public float respawnTime = 5f;
    public GameObject spawnPoint;
    public Transform[] randomRespawnPoint;

    public bool isWedging = false;
    public bool isWedgied = false;
    public float lastWedgieTime = -999f;
    public float wedgieCooldown = 5f;
    public float maxWedgieHealth = 10f;
    public float currentWedgieHealth = 10f;

    public GameObject invincibleShield;
    PlayerSoundManager playerSoundManager;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SocketManager = GameObject.Find("SocketManager");
        SocketManagerScript = SocketManager.GetComponent<SocketManager>();
        playerAttack = GetComponentInChildren<playerAttack>();
        playerSoundManager = GetComponent<PlayerSoundManager>();

        lastSentPosition = transform.position;
        player_Anim = GetComponent<player_anim>();

        pantypos = new Vector3(0, -0.1237817f, -0.07895534f);
        cam = Camera.main.gameObject;
        cameraMove = cam.GetComponent<CameraMove>();
        UIManagerScript = GameObject.Find("UIManager").GetComponent<UIManager>();
        player_ground = GetComponentInChildren<player_Ground>();

        UIManagerScript.Invincible();
        SetRandomRespawnPoint();
        SetStartPosition();
    }
    public string GetMYID()
    {

        string myID = SocketManagerScript.GetMySocketID();
        return myID;


    }
    void FixedUpdate()
    {
        if (!UIManagerScript.rewarding)
        {
            Move();
        }
    }
    void Update()
    {
        //디버깅용 키
        if (Input.GetKeyDown(KeyCode.F))
        {
            Death(GameObject.Find("Player"));
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            UIManagerScript.GameEnd();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }

        if (isAttacked && otherPlayerRighthand != null)
        {
            Wedgied(otherPlayerRighthand);
        }
        else if (!isAttacked && otherPlayerRighthand != null)
        {
            otherPlayerRighthand = null;
        }

        GroundCheck();
        Jump();
        PredictLanding();
        HandleOtherPlayerHealth();
        HandleWedgieHealth();

        if (Input.GetMouseButtonDown(0) && !dead && Cursor.lockState == CursorLockMode.Locked)
        {
            if (isAttack == false && isGrounded && attackSuccess == false)// 공격 상태가 아니고 땅에 있을 때만 공격 가능
            {
                isAttack = true; // 공격 시 이동 멈춤
                Invoke("AttackEnd", 0.5f); // 0.5초 후에 공격 종료 함수 호출
                player_Anim.GrabStart();
                Invoke("AttackCheck", 0.5f);
                playerSoundManager.PlayGrabstart();
            }


        }
        if (player_Anim.grabsuccess == true && attackSuccess)
        {
            if (otherPlayerPanty != null)
            {
                otherPlayerPanty.transform.position = playerRightHand.transform.position;
            }

        }


        if (Vector3.Distance(transform.position, lastSentPosition) > 0.5f || Mathf.Abs(transform.rotation.y - rotationY) > 0.08f)
        {
            Debug.Log("플레이어 위치 전송됨: " + transform.position + ", 회전: " + transform.rotation.y);
            //Debug.Log("플레이어 위치 전송됨: " + transform.position + ", 회전: " + transform.rotation.y);
            if (SocketManager != null)
            {

                string pos = $"{transform.position.x},{transform.position.y},{transform.position.z},{transform.eulerAngles.y}";
                SocketManagerScript.SendPlayerPosition(pos);
                lastSentPosition = transform.position;
                rotationY = transform.rotation.y; // 현재 Y축 회전 값 저장
            }
        }

        SoundPlayer();
    }
    public void Jump()
    {
        if (dead || Cursor.lockState != CursorLockMode.Locked || UIManagerScript.rewarding) return;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isAttack && !jumpRequested)
        {
            jumpRequested = true;
            player_Anim.Jumpup();
            Invoke("PerformJump", jumpDelay);

            playerSoundManager.PlayJump();
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
        if (dead || Cursor.lockState != CursorLockMode.Locked)
        {
            player_Anim.Move(0);
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        /*
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
        */
        Vector3 inputDir = new Vector3(moveX, 0, moveZ).normalized;

        if (isAttack || inputDir == Vector3.zero)
        {
            player_Anim.Move(0);
            return;
        }

        // 방향 기준을 월드가 아닌 "캐릭터 기준"으로 변환
        Vector3 worldDir = transform.TransformDirection(inputDir);
        transform.position += worldDir * moveSpeed * Time.fixedDeltaTime;

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
        if (playerAttack.attackTrigger == true && otherPlayer.GetComponent<OtherPlayer>().isWedgied == false)
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
        isWedgied = true;
        playerSoundManager.PlayStretching();
        //GetComponent<Renderer>().material.color = Color.blue;
    }
    public void Death(GameObject otherp)
    {
        dead = true;
        isAttacked = false;
        transform.position = otherp.transform.position + otherp.transform.forward;
        transform.rotation = otherp.transform.rotation;
        player_Anim.Death();
        attackPointFront.SetActive(false);
        attackPointBack.SetActive(false);
        Invoke("Disappear", respawnTime);
        Invoke("RigidbodyFreezeOff", 1f);
        Invoke("Respawn", respawnTime);
        UIManagerScript.StartCoroutine(UIManagerScript.YouDied());
        currentWedgieHealth = 0f;
        SetPantypos();
        playerSoundManager.PlayKill();
        Invoke("PlayDeathSound", 2f);
    }
    public void Respawn()
    {
        if (randomRespawnPoint.Length > 0)
        {
            int randomIndex = Random.Range(0, randomRespawnPoint.Length);
            transform.position = randomRespawnPoint[randomIndex].position;
            transform.rotation = randomRespawnPoint[randomIndex].rotation;
        }
        dead = false;
        RigidbodyFreeze();
        player_Anim.Respawn();
        attackPointFront.SetActive(true);
        attackPointBack.SetActive(true);
        Debug.Log("리스폰됨!!!~~~!~!~");
        currentWedgieHealth = maxWedgieHealth;
        UIManagerScript.UpdateWedgieHealthBar(currentWedgieHealth / maxWedgieHealth);
        UIManagerScript.Invincible();
        UIManagerScript.Respawn();
    }
    void SetRandomRespawnPoint()
    {
        if (spawnPoint.transform.childCount == 0)
        {
            Debug.LogWarning("자식 오브젝트가 없습니다.");
            return;
        }
        List<Transform> tempList = new List<Transform>();

        foreach (Transform child in spawnPoint.transform)
        {
            tempList.Add(child);
        }
        randomRespawnPoint = tempList.ToArray();
    }
    void SetStartPosition()
    {
        if (randomRespawnPoint.Length > 0)
        {
            int randomIndex = Random.Range(0, randomRespawnPoint.Length);
            transform.position = randomRespawnPoint[randomIndex].position;
            transform.rotation = randomRespawnPoint[randomIndex].rotation;
        }
    }
    void RigidbodyFreeze()
    {
        transform.rotation = new Quaternion(0, 0, 0, 0);
        rb.freezeRotation = true;
        Debug.Log("리지드 바디 설정!!!!!!!!!!!!@!#!#");

    }
    void RigidbodyFreezeOff()
    {
        rb.freezeRotation = false;
    }
    void Disappear()
    {
        player_Anim.Disappear();

        Debug.Log("사라짐 6^^^^^^^^^^^^^^^^^^^^^^^!!!~~~!~!~");
    }

    public void GetOtherPlayer(GameObject otherP)
    {

        if (otherPlayer == null)
        {
            otherPlayer = otherP; // 다른 플레이어 오브젝트 저장
            otherPlayerPanty = otherPlayer.GetComponent<OtherPlayer>().playerPanty;
            otherPlayerID = otherP.GetComponent<OtherPlayer>().playerID; // 다른 플레이어의 ID 저장
        }

    }
    void AttackEnd()
    {
        isAttack = false; // 공격 상태를 false로 변경하여 이동 가능하게 함
    }
    public void Wedgied(GameObject otherPlayerRighthand)
    {
        this.otherPlayerRighthand = otherPlayerRighthand;
        playerPanty.transform.position = this.otherPlayerRighthand.transform.position;
    }
    public void HandleWedgieHealth()
    {
        if (isWedgied)
        {
            currentWedgieHealth -= 1f * Time.deltaTime;
            lastWedgieTime = Time.time;
            UIManagerScript.UpdateWedgieHealthBar(currentWedgieHealth / maxWedgieHealth);

            if (currentWedgieHealth <= 0f)
            {
                currentWedgieHealth = 0f;
                isWedgied = false;
            }
        }
        else if (!isWedgied && !dead)
        {
            if (Time.time - lastWedgieTime > wedgieCooldown)
            {
                if (currentWedgieHealth < maxWedgieHealth)
                {
                    currentWedgieHealth += 0.5f * Time.deltaTime;
                    currentWedgieHealth = Mathf.Min(currentWedgieHealth, maxWedgieHealth);
                    UIManagerScript.UpdateWedgieHealthBar(currentWedgieHealth / maxWedgieHealth);
                }
            }
        }
    }
    void HandleOtherPlayerHealth()
    {
        if (isWedging)
        {
            if (otherPlayer.GetComponent<OtherPlayer>().isWedgied == false)
            {
                otherPlayer.GetComponent<OtherPlayer>().isWedgied = true;
            }

            if (otherPlayer.GetComponent<OtherPlayer>().currentWedgieHealth <= 0f)
            {
                otherPlayer.GetComponent<OtherPlayer>().currentWedgieHealth = 0f;
                isWedging = false;

                // 죽음 처리
                attackSuccess = false; // 공격 성공 상태 초기화

                isAttacked = false;

                player_Anim.Kill();
                playerSoundManager.PlayKill();

                isAttack = true; // 킬 도중 멈추기 위해 w
                Invoke("AttackEnd", 0.5f); // 0.5초 후에 공격 종료 함수 호출

                otherPlayer.GetComponent<OtherPlayer>().SetPantypos();

                //GetComponent<Renderer>().material.color = Color.white; // 색상 원래대로 복원
                SocketManagerScript.AttackSuccess(attacked); // 공격 성공 전송
                //otherPlayer.GetComponent<OtherPlayer>().rb.freezeRotation = false; // 리지드바디 회전 해제
                otherPlayerPanty = null; // 다른 플레이어 오브젝트 초기화
                otherPlayerID = null;
                otherPlayer = null;
            }
        }
        else
        {

        }

        // UI 같은 거 있다면 여기서 체력 반영 가능
        // UIManagerScript.UpdateWedgieHealthBar(currentWedgieHealth / maxWedgieHealth);
    }

    public void SetPantypos()
    {
        isAttacked = false;
        isWedgied = false;
        StopAllCoroutines(); // 중복 호출 방지
        StartCoroutine(MovePantyToTarget(pantypos));
    }

    IEnumerator MovePantyToTarget(Vector3 targetPos)
    {
        float speed = 20f; // 이동 속도
        float threshold = 0.01f; // 도달 판정 거리

        while (Vector3.Distance(playerPanty.transform.localPosition, targetPos) > threshold)
        {
            playerPanty.transform.localPosition = Vector3.MoveTowards(
                playerPanty.transform.localPosition,
                targetPos,
                speed * Time.deltaTime
            );
            yield return null; // 다음 프레임까지 대기
        }

        // 정확히 위치 고정
        playerPanty.transform.localPosition = targetPos;
        if (!dead)
        {
            playerSoundManager.PlaySetPos();
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("pantyDistance") && attackSuccess == true)
        {
            isWedging = true;

            wedgieTime = wedgieTime + Time.deltaTime; // 팬티 공격 성공 시 시간 증가
            //Debug.Log("팬티 공격 성공 시간: " + wedgieTime);
            if (isAttacked == false)
            {
                //otherPlayer.GetComponent<OtherPlayer>().BeingAttacked();//한번만실행
                isAttacked = true;
            }
            // 다른 플레이어에게 공격 알림
            /*
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
            */
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("pantyDistance"))
        {
            isWedging = false;
            if (otherPlayer != null)
            {
                otherPlayer.GetComponent<OtherPlayer>().isWedgied = false;
            }
        }
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
    void GroundCheck()
    {
        if (isGrounded && !player_Anim.landing && isGrounded != player_ground.isGrounded)
        {
            player_Anim.Landing(false);
        }
        else if ((!isGrounded || !player_Anim.landing) && isGrounded != player_ground.isGrounded)
        {
            player_Anim.Landing(true);
            playerSoundManager.PlayLand();
        }
        isGrounded = player_ground.isGrounded;
    }
    void PredictLanding()
    {
        if ((isGrounded && !player_Anim.landing) || jumpRequested || rb.linearVelocity.y > -0.01f)
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
        }
        else
        {
            //player_Anim.Falling(); // 아직 땅이 없으면 떨어지는 상태 유지
        }
    }
    void SoundPlayer()
    {
        if (player_Anim.animator1.GetCurrentAnimatorStateInfo(0).IsName("Player|Fastrun") || player_Anim.animator1.GetCurrentAnimatorStateInfo(0).IsName("Player|walk_backward"))
        {
            playerSoundManager.PlayFootstep();
        }
    }
    void PlayDeathSound()
    {
        playerSoundManager.PlayDeath();
    }
}
