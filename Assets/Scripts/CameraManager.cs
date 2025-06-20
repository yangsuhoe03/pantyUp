using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;
    public Camera subCamera;
    public Camera subCamera2;
    public Camera subCamera3;
    public Transform subCamera3originalPos;
    public float subcamrotationSpeed = 5f;

    void Start()
    {
        subCamera3originalPos = subCamera3.transform;
    }
    void Update()
    {
        if(subCamera.enabled)
        {
            subCamera.transform.Rotate(0, subcamrotationSpeed * Time.deltaTime, 0);
        }
    }

    public void SwitchCamera(int mode)
    {
        if(mode == 0)
        {
            mainCamera.enabled = true;
            subCamera.enabled = false;
            subCamera2.enabled = false;
            subCamera3.enabled = false;
        }
        else if(mode == 1)
        {
            mainCamera.enabled = false;
            subCamera.enabled = true;
            subCamera2.enabled = false;
            subCamera3.enabled = false;
        }
        else if(mode == 2)
        {
            mainCamera.enabled = false;
            subCamera.enabled = false;
            subCamera2.enabled = true;
            subCamera3.enabled = false;
        }
        else if(mode == 3)
        {
            mainCamera.enabled = false;
            subCamera.enabled = false;
            subCamera2.enabled = false;
            subCamera3.enabled = true;
        }
    }
}
