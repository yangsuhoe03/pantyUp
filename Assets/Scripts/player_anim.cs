using UnityEngine;

public class player_anim : MonoBehaviour
{
    public SocketManager SocketManager;
    public Animator animator1, animator2;
    public GameObject player1, player2, player3;

    //animation Parameter List
    public bool walking, running, walkingbackward, landing, grabsuccess;

    void Start()
    {
        if (SocketManager == null)
        {
            GameObject.Find("SocketManager").GetComponent<SocketManager>();
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
    public void Jumpup()
    {
        animator1.SetTrigger("jumpup");
    }
    public void Falling()
    {
        animator1.SetTrigger("falling");
    }
    public void Kill()
    {
        animator1.SetTrigger("kill");
    }
    public void Death()
    {
        animator2.SetTrigger("death");
        player1.SetActive(false);
        player2.SetActive(true);
        player3.SetActive(true);
    }
    public void GrabStart()
    {
        animator1.SetTrigger("grabstart");
    }
}
