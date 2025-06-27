using UnityEngine;
using System.Collections;
using UnityEditor;
using TMPro;

public class OtherPlayer : MonoBehaviour
{
    public string playerID;
    public string mynickName = "";
    public TextMeshPro mynickNameText;
    public Animator animator1, animator2;
    public PlayerSoundManager playerSoundManager;
    public GameObject player1, player2, player3, playerdeadbody;
    public GameObject attackPointFront;
    public GameObject attackPointBack;
    public GameObject invincibleShield;
    private string currentmove;
    //animation Parameter List
    public bool walking, running, walkingbackward, landing, grabsuccess;
    public bool isdead = false;
    public bool pantymoving = false;

    public GameObject playerRightHand;
    public GameObject playerPanty;
    public GameObject otherPlayerPanty;
    Vector3 pantypos;
    Vector3 lastSentPosition;
    public float maxWedgieHealth = 10f;
    public float currentWedgieHealth = 10f;
    public bool isWedgied;
    public float lastWedgieTime = -999f;
    public float wedgieCooldown = 5f;
    public Rigidbody rb;

    Vector3 otherPos;
    float otherRotationY;
    bool isAttacked = false;
    void Start()
    {
        pantypos = new Vector3(0, -0.1237817f, -0.07895534f);
        StartCoroutine(InvincibleShield());
        rb = GetComponent<Rigidbody>();
        playerSoundManager = GetComponent<PlayerSoundManager>();
        //rb.freezeRotation = false;
        if (mynickNameText.text == "null")
        {

        }
    }
    void Update()
    {
        if (isAttacked && otherPlayerPanty != null && otherPlayerPanty.GetComponentInParent<OtherPlayer>().isdead == false && pantymoving == false)
        {
            Wedgie(otherPlayerPanty);
            Debug.Log("지금 잡고 있는 중이다.");
        }
        else if (!isAttacked && otherPlayerPanty != null)
        {
            otherPlayerPanty = null;
        }
        HandleWedgieHealth();

        SoundPlayer();
    }
    void FixedUpdate()
    {

        transform.position = Vector3.Lerp(transform.position, otherPos, Time.deltaTime * 3f);
        transform.rotation = Quaternion.Euler(0, otherRotationY, 0); // Y축 회전 적용
    }
    void LateUpdate()
    {
        LookPlayer();
    }

    public void SetPlayerID(string Id)
    {
        playerID = Id;
    }
    public void SetPosition(Vector3 pos, float rotationY)
    {
        //transform.position = pos;
        // 부드러운 움직임을 위한 보간 적용
        otherPos = pos;
        otherRotationY = rotationY;
        lastSentPosition = pos;
        
    }
    public void LookPlayer()
    {
        Vector3 camPos = Camera.main.transform.position;

        // 수평 방향으로만 바라보도록 Y축 고정
        Vector3 direction = transform.position - camPos;
        direction.y = 0f; // 위아래 각도 제거

        if (direction != Vector3.zero)
        {
            mynickNameText.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    public void SetNickname(string nickname)
    {
        mynickName = nickname;
        mynickNameText.text = mynickName;
    }
    public void SetAnimation(string num)
    {
        currentmove = num;
        if (num == "0")
        {
            walking = false;
            running = false;
            walkingbackward = false;
            SetAnimationParameter();
        }
        else if (num == "1")
        {
            walking = false;
            running = true;
            walkingbackward = false;
            SetAnimationParameter();
        }
        else if (num == "-1")
        {
            walking = false;
            running = false;
            walkingbackward = true;
            SetAnimationParameter();

        }
        else if (num == "2")
        {
            walking = true;
            running = false;
            walkingbackward = false;
            SetAnimationParameter();
        }
        else if (num == "3")
        {
            animator1.SetTrigger("jumpup");
            playerSoundManager.PlayJump();
        }
        else if (num == "4")
        {
            if(landing == false)
                playerSoundManager.PlayLand();
            landing = true;
            SetAnimationParameter();
        }
        else if (num == "5")
        {
            landing = false;
            SetAnimationParameter();
        }
        else if (num == "6")
        {
            animator1.SetTrigger("falling");
        }
        else if (num == "7")
        {
            animator1.SetTrigger("grabstart");
            playerSoundManager.PlayGrabstart();
        }
        else if (num == "8")
        {
            grabsuccess = true;
            SetAnimationParameter();
        }
        else if (num == "9")
        {
            grabsuccess = false;
            SetAnimationParameter();
            isAttacked = false;
        }
        else if (num == "10")
        {
            animator1.SetTrigger("kill");
            grabsuccess = false;
            SetAnimationParameter();
            isAttacked = false;
        }
        else if (num == "11")
        {
            player1.SetActive(false);
            player2.SetActive(true);
            player3.SetActive(true);
            animator2.SetTrigger("death");
            attackPointFront.SetActive(false);
            attackPointBack.SetActive(false);
            isdead = true;
            isWedgied = false;
        }
        else if (num == "12")
        {
            player1.SetActive(true);
            player2.SetActive(false);
            player3.SetActive(false);
            playerdeadbody.SetActive(true);
            attackPointFront.SetActive(true);
            attackPointBack.SetActive(true);
            animator2.SetTrigger("respawn");
            isdead = false;

            isWedgied = false;
            currentWedgieHealth = maxWedgieHealth;

            StartCoroutine(InvincibleShield());
        }
        else if (num == "13")
        {
            playerdeadbody.SetActive(false);
            //disappear
        }
    }
    void SetAnimationParameter()
    {
        animator1.SetBool("walking", walking);
        animator1.SetBool("running", running);
        animator1.SetBool("walkingbackward", walkingbackward);
        animator1.SetBool("landing", landing);
        animator1.SetBool("grabsuccess", grabsuccess);
    }
    public void BeingAttacked()
    {
        // 공격을 받았을 때의 로직
        // 예: 색상 변경, 애니메이션 재생 등
        //Debug.Log("공격 받음: " + playerID);
        //GetComponent<Renderer>().material.color = Color.black; 
    }
    public void SetPantypos()
    {
        Debug.Log("SetPantypos 실행");
        isAttacked = false;
        StopAllCoroutines(); // 중복 호출 방지
        StartCoroutine(MovePantyToTarget(pantypos));
        pantymoving = true;
        isWedgied = false;
    }
    public void HandleWedgieHealth()
    {
        if (isWedgied)
        {
            currentWedgieHealth -= 1f * Time.deltaTime;
            lastWedgieTime = Time.time;

            if (currentWedgieHealth <= 0f)
            {
                currentWedgieHealth = 0f;
                isWedgied = false;
            }
        }
        else if (!isWedgied)
        {
            if (Time.time - lastWedgieTime > wedgieCooldown)
            {
                if (currentWedgieHealth < maxWedgieHealth)
                {
                    currentWedgieHealth += 0.5f * Time.deltaTime;
                    currentWedgieHealth = Mathf.Min(currentWedgieHealth, maxWedgieHealth);
                }
            }
        }
    }

    IEnumerator MovePantyToTarget(Vector3 targetPos)
    {
        isAttacked = false;
        Debug.Log("MovePantyToTarget 실행");
        if (playerPanty == null)
        {
            Debug.LogError("MovePantyToTarget: playerPanty가 null입니다!");
            yield break;
        }

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
        playerSoundManager.PlaySetPos();
        playerPanty.transform.localPosition = targetPos;
        Debug.Log($"팬티 이동 완료: {playerPanty.transform.localPosition}");
        pantymoving = false;
    }
    public void SetIsAttacked(bool isAttacked)
    {
        this.isAttacked = isAttacked;
        if (isAttacked)
        {
            playerSoundManager.PlayStretching();
        }
    }
    public void Wedgie(GameObject otherPlayerPanty)
    {
        otherPlayerPanty.transform.position = playerRightHand.transform.position;
    }
    IEnumerator InvincibleShield()
    {
        invincibleShield.SetActive(true);
        attackPointBack.SetActive(false);

        float duration = 3f;
        float remaining = duration;

        while (remaining > 0f)
        {
            yield return null;
            remaining -= Time.deltaTime;
        }

        attackPointBack.SetActive(true);
        invincibleShield.SetActive(false);
    }
    void SoundPlayer()
    {
        if (animator1.GetCurrentAnimatorStateInfo(0).IsName("Player|Fastrun") || animator1.GetCurrentAnimatorStateInfo(0).IsName("Player|walk_backward"))
        {
            playerSoundManager.PlayFootstep();
        }
    }
    public void PlayStretching()
    {
        playerSoundManager.PlayStretching();
    }
}
