using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public GameObject player;
    PlayerMove playerMove;
    public float mouseX;
    float xRotation = 0f;

    void Start()
    {
        //scoreManager = FindObjectOfType<ScoreManager>();
        if (player == null)
        {
            player = GameObject.Find("Player");
            playerMove = player.GetComponent<PlayerMove>();
        }
    }

    void Update()
    {
        // 마우스 커서가 잠겨있을 때만 카메라 회전
        if (Cursor.lockState == CursorLockMode.Locked && !playerMove.dead)
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);


            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
        else
        {
            //카메라 위로 올라감
        }
    }

}
