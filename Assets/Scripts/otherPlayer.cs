using UnityEngine;

public class OtherPlayer : MonoBehaviour
{
    public string playerID;
    public Animator animator1, animator2;
    public GameObject player1, player2, player3;
    private string currentmove;
    //animation Parameter List
    public bool walking, running, walkingbackward, landing, grabsuccess;

    public GameObject playerRightHand;
    public GameObject playerPanty;
    public GameObject otherPlayerPanty;
    Vector3 pantypos;

    bool isAttacked = false;
    void Start()
    {
        pantypos = new Vector3(0, -0.1237817f, -0.07895534f);
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
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, rotationY, 0); // Y축 회전 적용
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
        }
        else if (num == "12")
        {
            player1.SetActive(true);
            player2.SetActive(false);
            player3.SetActive(false);
            animator2.SetTrigger("respawn");
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
        playerPanty.transform.localPosition = pantypos;
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
