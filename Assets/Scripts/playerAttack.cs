using UnityEngine;

public class playerAttack : MonoBehaviour
{
    public bool attackTrigger = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("attackPointBack"))
        {
            attackTrigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("attackPointBack"))
        {
            attackTrigger = false;
        }
    }


}
