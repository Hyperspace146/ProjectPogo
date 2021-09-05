using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerJoe : MonoBehaviour
{
    public float GroundAcceleration = 12;
    public float AirAcceleration = 15;
    public float MaxGroundedSpeed = 15;
    public float MaxAerialSpeed = 15;
    public float Friction = 1;
    public float JumpHeight = 5000f;

    public Transform GroundCheck;
    public float GroundDist = 0.4f;
    public LayerMask GroundMask;

    [Tooltip("Change in angle of velocity per second when strafing (in degrees).")]
    public float MaxDeltaTheta = 4;

    [Tooltip("Maximum allowed vel angle from look direction (in degrees).")]
    public float MaxAngRelativeToLook = 85;


    private bool isGrounded;

    private Rigidbody rb;
    private Collider collider;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y));
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDist, GroundMask);

        if (isGrounded)
        {
            GroundMove();
        }
        else
        {
            AirMove();
            AirStrafe();
        }
    }

    void GroundMove()
    {
        // Move the player according to directional input
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection.Normalize();

        // Calculate new horizontal grounded speed
        Vector3 move = moveDirection * GroundAcceleration * Time.fixedDeltaTime;
        move = new Vector3(rb.velocity.x, 0, rb.velocity.z) + move;

        // Cap horizontal grounded speed
        move = Vector3.ClampMagnitude(move, MaxGroundedSpeed);

        move.y = rb.velocity.y;
        rb.velocity = move;

        // the *= makes us slow down very slow when we're initially moving very slow (i think). try straight subtracting from velocity instead?
        // but then we'll slow down very slowly if we're moving really fast. maybe that's what we want though?
        float speed = rb.velocity.magnitude;
        float frictionMultiplier = speed - (Friction * Time.fixedDeltaTime);
        frictionMultiplier = Mathf.Max(0, frictionMultiplier);
        if (speed > 0)
        {
            frictionMultiplier /= speed;
        }
        float fricX = rb.velocity.x * frictionMultiplier;
        float fricZ = rb.velocity.z * frictionMultiplier;
        rb.velocity = new Vector3(fricX, rb.velocity.y, fricZ);
    }

    void AirMove()
    {
        //Movement Vector
        Vector3 moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection.Normalize();

        Vector3 move = moveDirection * AirAcceleration * Time.fixedDeltaTime;
        move = new Vector3(rb.velocity.x, 0, rb.velocity.z) + move;
        move = Vector3.ClampMagnitude(move, MaxAerialSpeed);
        move.y = rb.velocity.y;
       rb.velocity = move;
    }

    void AirStrafe()
    {
        // Calculate the angle between the current look direction and current velocity (in ONLY the xz plane)
        Vector2 lookDirection = new Vector2(transform.forward.x, transform.forward.z);
        Vector2 currentVelocity = new Vector2(rb.velocity.x, rb.velocity.z);
        float angle = Vector3.Dot(lookDirection, currentVelocity);
        angle /= lookDirection.magnitude;
        angle /= currentVelocity.magnitude;
        angle = Mathf.Acos(angle);
        angle *= Mathf.Rad2Deg;

        // Collect horizontal "a" and "d" key inputs
        float x = Input.GetAxis("Horizontal");

        if (Mathf.Abs(angle) > MaxAngRelativeToLook || x == 0)
        {
            return;
        }

        float deltaTheta = ((MaxAngRelativeToLook - angle) / MaxAngRelativeToLook) * MaxDeltaTheta * Time.fixedDeltaTime;

        // Determines is Left or Right direction
        if (x > 0)
        {
            deltaTheta *= -1;
        }

        print(deltaTheta);

        // Instantiation of empty, to be adjusted, velocity vector
        Vector3 newVelocity = new Vector3(0, rb.velocity.y, 0);

        // Use matrix multiplication using the rotation matrix to rotate our current velocity vector 
        // by theta degrees (the magnitude of our velocity vector still stays the same)
        newVelocity.x = Mathf.Cos(deltaTheta) * rb.velocity.x + -Mathf.Sin(deltaTheta) * rb.velocity.z;
        newVelocity.z = Mathf.Sin(deltaTheta) * rb.velocity.x + Mathf.Cos(deltaTheta) * rb.velocity.z;

        // Apply changed directional velocity to rigidbody
        rb.velocity = newVelocity;
    }
}
