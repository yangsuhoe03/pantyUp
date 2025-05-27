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
    public void BeingAttacked()
    {
        // 공격을 받았을 때의 로직
        // 예: 색상 변경, 애니메이션 재생 등
        Debug.Log("공격 받음: " + playerID);
        GetComponent<Renderer>().material.color = Color.black; // 
    }

}
