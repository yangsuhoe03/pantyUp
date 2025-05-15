using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 숨기기 + 고정
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 수직 회전 제한
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 카메라는 위아래만 회전
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // 플레이어는 좌우 회전
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
