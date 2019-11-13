using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Cmd {
    public float forwardMove;
    public float rightMove;
    public float upMove;
}

public class PlayerMovement : MonoBehaviour {

    public Transform playerView; // Camera
    public float playerViewYOffset = 0.6f; // The height at which the camera is bound to
    public float xMouseSensitivity = 30.0f;
    public float yMouseSensitivity = 30.0f;

    public float gravity = 20.0f;
    public float friction = 6;

    public float moveSpeed = 7.0f;
    public float runAcceleration = 14.0f;
    public float runDeacceleration = 10.0f;
    public float airAcceleration = 2.0f;
    public float airDecceleration = 2.0f;
    public float airControl = 0.3f;
    public float sideStrafeAcceleration = 50.0f;
    public float sideStrafeSpeed = 1.0f;
    public float jumpSpeed = 8.0f;
    public bool holdJumpToBhop = true;

    private CharacterController self;
    private Cmd command;

    private float rotX = 0.0f;
    private float rotY = 0.0f;

    private Vector3 moveDirectionNorm = Vector3.zero;
    private Vector3 playerVelocity = Vector3.zero;
    private float playerTopVelocity = 0.0f;

    private bool wishJump = false;
    private float playerFriction = 0.0f;

    void Start () 
    {
        // Hide the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerView == null)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera != null)
            {
                playerView = mainCamera.gameObject.transform;
            }
        }

        playerView.position = new Vector3(transform.position.x, transform.position.y + playerViewYOffset, transform.position.z);
        self = GetComponent<CharacterController>();
    }

    
    void Update () 
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Cursor.lockState = CursorLockMode.Locked;
            } 
        }

        rotX -= Input.GetAxisRaw("Mouse Y") * xMouseSensitivity * 0.02f;
        rotY += Input.GetAxisRaw("Mouse X") * yMouseSensitivity * 0.02f;


        if (rotX < -90)
        {
            rotX = -90;
        }
        else if (rotX > 90)
        {
            rotX = 90;
        }
            
        this.transform.rotation = Quaternion.Euler(0, rotY, 0); // Rotates the collider
        playerView.rotation = Quaternion.Euler(rotX, rotY, 0); // Rotates the camera
        QueueJump();

        if (self.isGrounded)
        {
            GroundMove();
        }
        else if (!self.isGrounded)
        {
            AirMove();
        }
            

        // Move the controller
        self.Move(playerVelocity * Time.deltaTime);

        /* Calculate top velocity */
        Vector3 udp = playerVelocity;
        udp.y = 0.0f;
        if (udp.magnitude > playerTopVelocity)
            playerTopVelocity = udp.magnitude;

        //Need to move the camera after the player has been moved because otherwise the camera will clip the player if going fast enough and will always be 1 frame behind.
        // Set the camera's position to the transform
        playerView.position = new Vector3(
            transform.position.x,
            transform.position.y + playerViewYOffset,
            transform.position.z);
    }

    private void QueueJump()
    {
        if (holdJumpToBhop)
        {
            wishJump = Input.GetButton("Jump");
            return;
        }

        if (Input.GetButtonDown("Jump") && !wishJump)
        {
            wishJump = true;
        }
            
        if (Input.GetButtonUp("Jump"))
        {
            wishJump = false;
        }       
    }

    private void GroundMove()
    {
        Vector3 wishDir;

        if (!wishJump)
        {
            ApplyFriction(1.0f);
        }
        else
        {
            ApplyFriction(0);
        }

        SetMovementDir();

        wishDir = new Vector3(command.rightMove, 0, command.forwardMove);
        wishDir = transform.TransformDirection(wishDir);
        wishDir.Normalize();
        moveDirectionNorm = wishDir;

        var wishSpeed = wishDir.magnitude;
        wishSpeed *= moveSpeed;

        Accelerate(wishDir, wishSpeed, runAcceleration);
        playerVelocity.y = -gravity * Time.deltaTime;

        if (wishJump)
        {
            playerVelocity.y = jumpSpeed;
            wishJump = false;
        }
    }

    private void ApplyFriction(float amount)
    {
        Vector3 ps = playerVelocity;
        float speed;
        float newSpeed;
        float control;
        float drop;

        ps.y = 0.0f;
        speed = ps.magnitude;
        drop = 0.0f;

        if (self.isGrounded)
        {
            if (speed < runAcceleration)
            {
                control = runAcceleration;
            }
            else
            {
                control = speed;
            }

            drop = control * friction * Time.deltaTime * amount;
        }

        newSpeed = speed - drop;
        playerFriction = newSpeed;

        if (newSpeed < 0)
        {
            newSpeed = 0;
        }
            
        if (speed > 0)
        {
            newSpeed /= speed;
        }
            
        playerVelocity.x *= newSpeed;
        playerVelocity.z *= newSpeed;
    }

    private void SetMovementDir()
    {
        command.forwardMove = Input.GetAxisRaw("Vertical");
        command.rightMove = Input.GetAxisRaw("Horizontal");
    }

    private void Accelerate(Vector3 wishDir, float whishSpeed, float accel)
    {
        float addSpeed;
        float accelSpeed;
        float currentSpeed;

        currentSpeed = Vector3.Dot(playerVelocity, wishDir);
        addSpeed = whishSpeed - currentSpeed;

        if (addSpeed <= 0)
        {
            return;
        }
            
        accelSpeed = accel * Time.deltaTime * whishSpeed;

        if (accelSpeed > addSpeed)
        {
            accelSpeed = addSpeed;
        }
            
        playerVelocity.x += accelSpeed * wishDir.x;
        playerVelocity.z += accelSpeed * wishDir.z;
    }

    private void AirMove()
    {
        Vector3 wishDir;
        float whisVel = airAcceleration;
        float accel;

        SetMovementDir();

        wishDir = new Vector3(command.rightMove, 0, command.forwardMove);
        wishDir = transform.TransformDirection(wishDir);

        float wishSpeed = wishDir.magnitude;
        wishSpeed *= moveSpeed;

        wishDir.Normalize();
        moveDirectionNorm = wishDir;

        float wishSpeed2 = wishSpeed;

        if (Vector3.Dot(playerVelocity, wishDir) < 0)
        {
            accel = airDecceleration;
        }
        else
        {
            accel = airAcceleration;
        }
            
        if (command.forwardMove == 0 && command.rightMove != 0)
        {
            if (wishSpeed > sideStrafeSpeed)
            {
                wishSpeed = sideStrafeSpeed;
            }
                
            accel = sideStrafeAcceleration;
        }

        Accelerate(wishDir, wishSpeed, accel);

        if (airControl > 0)
        {
            AirControl(wishDir, wishSpeed2);
        }
            
        playerVelocity.y -= gravity * Time.deltaTime;
    }

    private void AirControl(Vector3 wishDir, float wishSpeed)
    {
        float zSpeed;
        float speed;
        float dot;
        float k;

        if (Mathf.Abs(command.forwardMove) < 0.001 || Mathf.Abs(wishSpeed) < 0.001)
        {
            return;
        }
            
        //Normalize from IDTech
        zSpeed = playerVelocity.y;
        playerVelocity.y = 0;
        speed = playerVelocity.magnitude;
        playerVelocity.Normalize();

        dot = Vector3.Dot(playerVelocity, wishDir);
        k = 32;
        k *= airControl * dot * dot * Time.deltaTime;

        if (dot > 0)
        {
            playerVelocity.x = playerVelocity.x * speed + wishDir.x * k;
            playerVelocity.y = playerVelocity.y * speed + wishDir.y * k;
            playerVelocity.z = playerVelocity.z * speed + wishDir.z * k;

            playerVelocity.Normalize();
            moveDirectionNorm = playerVelocity;
        }

        playerVelocity.x *= speed;
        playerVelocity.y = zSpeed;
        playerVelocity.z *= speed;
    }
}