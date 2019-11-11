using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0;
    [SerializeField] private Transform playerBody;
    private float ClampX = 0;

    void Awake()
    {

    }


    void Update()
    {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        MoveCamera();       

    }

    void MoveCamera()
    {

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float rotAmountX = mouseX * sensitivity;
        float rotAmountY = mouseY * sensitivity;
        ClampX -= rotAmountY;
        Vector3 targetRotCam = transform.rotation.eulerAngles;
        Vector3 targetRotBody = playerBody.rotation.eulerAngles;

        targetRotCam.x -= rotAmountY;
        targetRotBody.y += rotAmountX;

        if (ClampX > 90)
        {
            ClampX = 90;
            targetRotCam.x = 90;
        }
        else if (ClampX < -90)
        {
            ClampX = -90;
            targetRotCam.x = 270;
        }

        transform.rotation = Quaternion.Euler(targetRotCam);
        playerBody.rotation = Quaternion.Euler(targetRotBody);
    }
}
