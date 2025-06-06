using JetBrains.Annotations;
using UnityEngine;
/*
idle = 0
running = 1
walkback = -1
jumpup = 3
landingon = 4
landingoff = 5
falling = 6
grabstart = 7
grabsuccess = 8
grabfailed = 9
kill = 10
death = 11
*/
public class player_anim : MonoBehaviour
{
    public SocketManager socketManager;
    public Animator animator1, animator2;
    public GameObject player1, player2, player3;

    //서버로 보낼 스트링들
    private string currentmove;

    //animation Parameter List
    public bool walking, running, walkingbackward, landing, grabsuccess;

    void Start()
    {
        if (socketManager == null)
        {
            socketManager = GameObject.Find("SocketManager").GetComponent<SocketManager>();
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
    public void Move(float dir)
    {
        if (dir > 0)
        {
            walking = false;
            running = true;
            walkingbackward = false;
            currentmove = "1";
        }
        else if (dir == 0)
        {
            walking = false;
            running = false;
            walkingbackward = false;
            currentmove = "0";
        }
        else
        {
            walking = false;
            running = false;
            walkingbackward = true;
            currentmove = "-1";
        }
        SetAnimationParameter();
        //socketManager.SendPlayerMove(currentmove);
    }
    public void Landing(bool grounded)
    {
        landing = grounded;
        if (grounded)
            currentmove = "4";
        else
            currentmove = "5";

        SetAnimationParameter();
        //socketManager.SendPlayerMove(currentmove);
    }
    public void Jumpup()
    {
        currentmove = "3";
        animator1.SetTrigger("jumpup");
        //socketManager.SendPlayerMove(currentmove);
    }
    public void Falling()
    {
        
        animator1.SetTrigger("falling");
        currentmove = "6";
        //socketManager.SendPlayerMove(currentmove);
    }
    public void Kill()
    {
        animator1.SetTrigger("kill");
        currentmove = "10";
        //socketManager.SendPlayerMove(currentmove);
    }
    public void Death()
    {
        player1.SetActive(false);
        player2.SetActive(true);
        player3.SetActive(true);
        animator2.SetTrigger("death");

        currentmove = "11";
        //socketManager.SendPlayerMove(currentmove);
    }
    public void GrabStart()
    {
        animator1.SetTrigger("grabstart");
        currentmove = "7";
        //socketManager.SendPlayerMove(currentmove);
    }
    public void GrabSuccess(bool success)
    {
        grabsuccess = success;
        SetAnimationParameter();
        if(success)
            currentmove = "8";
        else
            currentmove = "9";
        //socketManager.SendPlayerMove(currentmove);
    }
}
