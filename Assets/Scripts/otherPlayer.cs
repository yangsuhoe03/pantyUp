using UnityEngine;

public class OtherPlayer : MonoBehaviour
{
    string playerID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

}
