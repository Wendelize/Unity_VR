using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public float speed = 3;
    public float gravity = 18;
    public float jumpForce = 10;

    Camera cam;
    CharacterController controller;
    Vector3 velocity;
    Vector3 smoothV;
    public float yaw;
    public float pitch;
    float verticalVelocity;
    float lastGroundedTime;


    bool jumping = false;


    void Start()
    {
        cam = Camera.main;

        controller = GetComponent<CharacterController>();

        yaw = transform.eulerAngles.y;
        pitch = cam.transform.localEulerAngles.x;
        //smoothYaw = yaw;
        //smoothPitch = pitch;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Rotation();
    }

    void Movement()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 inputDir = new Vector3(input.x, 0, input.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection(inputDir);
        Vector3 targetVelocity = worldInputDir * speed;
        velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref smoothV, 0.1f);
        Jump();
    }

    void Rotation()
    {
        yaw = transform.eulerAngles.y;
        pitch = cam.transform.localEulerAngles.x;
        float mX = Input.GetAxisRaw("Mouse X");
        float mY = Input.GetAxisRaw("Mouse Y");

        yaw += mX; //* mouseSensitivity;
        pitch -= mY;
        pitch = Mathf.Clamp(pitch, -45, 80);
        //float smoothPitch = Mathf.SmoothDampAngle(smoothPitch, pitch, ref pitchSmoothV, rotationSmoothTime);
        //float smoothYaw = Mathf.SmoothDampAngle(smoothYaw, yaw, ref yawSmoothV, rotationSmoothTime);

        transform.eulerAngles = Vector3.up;//* smoothYaw;
        cam.transform.localEulerAngles = Vector3.right;// * smoothPitch;
    }

    void Jump()
    {
        verticalVelocity -= gravity * Time.deltaTime;
        velocity = new Vector3(velocity.x, verticalVelocity, velocity.z);

        var flags = controller.Move(velocity * Time.deltaTime);
        if (flags == CollisionFlags.Below)
        {
            jumping = false;
            lastGroundedTime = Time.time;
            verticalVelocity = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            float timeSinceLastTouchedGround = Time.time - lastGroundedTime;
            if (controller.isGrounded || (!jumping && timeSinceLastTouchedGround < 0.15f))
            {
                jumping = true;
                verticalVelocity = jumpForce;
            }
        }
    }

}
