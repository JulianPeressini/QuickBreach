using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
{
    private CharacterController self;

    [SerializeField] private float defaultSpeed = 0;
    [SerializeField] private float currentSpeed = 0;
    [SerializeField] private float maxSpeed = 0;
    [SerializeField] private float acceleration = 0;
    [SerializeField] private float jumpPower = 0;
    [SerializeField] private float gravity = 0;
    [SerializeField] private float bunnyHopLeaway = 0;
    private float bunnyHopDelay = 0;

    [SerializeField] private float axisY;
    [SerializeField] private float axisX;
    [SerializeField] private float axisZ;
    private Vector3 direction = new Vector3();

    void Awake() 
    {
        self = GetComponent<CharacterController>();
    }

    void Update() 
    {
        GetInputs ();
        Movement ();
    }

    private void GetInputs() 
    {
        axisX = Input.GetAxis("Horizontal");
        axisZ = Input.GetAxis("Vertical");

        if (Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetKey(KeyCode.Space)) 
        {
            Jump();
        }
    }

    private void Movement() 
    {
        direction.x = axisX;
        direction.z = axisZ;
        direction = transform.TransformDirection(direction);
        direction *= currentSpeed;

        if (!self.isGrounded) 
        {
            axisY -= gravity * Time.deltaTime;

            if (axisZ != 0) 
            {
                if (currentSpeed + acceleration > maxSpeed) 
                {
                    currentSpeed = maxSpeed;
                } 
                else 
                {
                    currentSpeed += acceleration;
                }
            } 
            else 
            {
                currentSpeed = defaultSpeed;
            }

            axisY -= acceleration;
            bunnyHopDelay = Time.time + bunnyHopLeaway;
        } 
        else 
        {
            if (Time.time > bunnyHopDelay) 
            {
                currentSpeed = defaultSpeed;
            }

            if (axisY < 0) 
            {
                axisY = -0.5f;
            }
        }

        direction.y = axisY;
        self.Move (direction * Time.deltaTime);
    }

    private void Jump() 
    {
        if (self.isGrounded) 
        {
            axisY = jumpPower;
        }
    }

    private void OpenBreach(GameObject breachToPlace)
    {
        Ray rayOrigin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hitInfo;

        if (Physics.Raycast(rayOrigin, out hitInfo))
        {
            Quaternion normal = Quaternion.LookRotation(hitInfo.normal);
            breachToPlace.transform.position = hitInfo.point;
            breachToPlace.transform.rotation = normal;
        }
    }

    public void Teleport(GameObject targetBreach)
    {
        self.enabled = false;
        gameObject.transform.position = targetBreach.transform.position;
        gameObject.transform.localEulerAngles = new Vector3(0, targetBreach.transform.localEulerAngles.y, 0);
        self.enabled = true;
    }
}