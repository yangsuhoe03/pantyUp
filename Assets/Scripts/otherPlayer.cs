using UnityEngine;
using System.Collections;

public class OtherPlayer : MonoBehaviour
{
    public string playerID;
    public Animator animator1, animator2;
    public GameObject player1, player2, player3, playerdeadbody;
    public GameObject attackPointFront;
    public GameObject attackPointBack;
    private string currentmove;
    //animation Parameter List
    public bool walking, running, walkingbackward, landing, grabsuccess;

    public GameObject playerRightHand;
    public GameObject playerPanty;
    public GameObject otherPlayerPanty;
    Vector3 pantypos;
    Vector3 lastSentPosition;

    bool isAttacked = false;
    void Start()
    {
        // 팬티의 초기 위치를 저장
        if (playerPanty != null)
        {
            pantypos = playerPanty.transform.localPosition;
            Debug.Log($"팬티 초기 위치 저장: {pantypos}");
        }
        else
        {
            Debug.LogError("playerPanty가 할당되지 않았습니다!");
        }
    }
    void Update()
    {
        if (isAttacked && otherPlayerPanty != null)
        {
            Wedgie(otherPlayerPanty);
        }
        else if (!isAttacked && otherPlayerPanty != null)
        {
            otherPlayerPanty = null;
        }
    }

    public void SetPlayerID(string Id)
    {
        playerID = Id;
    }
    public void SetPosition(Vector3 pos, float rotationY)
    {
        //transform.position = pos;
        // 부드러운 움직임을 위한 보간 적용
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 10f);
        transform.rotation = Quaternion.Euler(0, rotationY, 0); // Y축 회전 적용
        lastSentPosition = pos;
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
        }
        else if (num == "4")
        {
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
        }
        else if (num == "10")
        {
            animator1.SetTrigger("kill");
        }
        else if (num == "11")
        {
            player1.SetActive(false);
            player2.SetActive(true);
            player3.SetActive(true);
            animator2.SetTrigger("death");
            attackPointFront.SetActive(false);
            attackPointBack.SetActive(false);
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
        if (playerPanty == null)
        {
            Debug.LogError("SetPantypos: playerPanty가 null입니다!");
            return;
        }

        isAttacked = false;
        StopAllCoroutines(); // 중복 호출 방지
        
        // 현재 팬티 위치와 목표 위치 로깅
        Debug.Log($"팬티 현재 위치: {playerPanty.transform.localPosition}, 목표 위치: {pantypos}");
        
        StartCoroutine(MovePantyToTarget(pantypos));
    }

    IEnumerator MovePantyToTarget(Vector3 targetPos)
    {
        if (playerPanty == null)
        {
            Debug.LogError("MovePantyToTarget: playerPanty가 null입니다!");
            yield break;
        }

        float speed = 20f; // 이동 속도
        float threshold = 0.01f; // 도달 판정 거리
        float startTime = Time.time;
        float maxDuration = 2f; // 최대 2초 동안만 시도

        while (Vector3.Distance(playerPanty.transform.localPosition, targetPos) > threshold)
        {
            if (Time.time - startTime > maxDuration)
            {
                Debug.LogWarning("팬티 이동 시간 초과!");
                break;
            }

            playerPanty.transform.localPosition = Vector3.MoveTowards(
                playerPanty.transform.localPosition,
                targetPos,
                speed * Time.deltaTime
            );

            // 현재 위치 로깅
            Debug.Log($"팬티 이동 중: {playerPanty.transform.localPosition}");
            
            yield return null;
        }

        // 정확히 위치 고정
        playerPanty.transform.localPosition = targetPos;
        Debug.Log($"팬티 이동 완료: {playerPanty.transform.localPosition}");
    }
    public void SetIsAttacked(bool isAttacked)
    {
        this.isAttacked = isAttacked;
    }
    public void Wedgie(GameObject otherPlayerPanty)
    {
        otherPlayerPanty.transform.position = playerRightHand.transform.position;
    }
}
