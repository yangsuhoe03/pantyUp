using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // ???²J ????? + ???? ¾È³ç
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ???? ??? ????
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // ????? ??????? ???
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // ?¡À????? ?¢¯? ???
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
