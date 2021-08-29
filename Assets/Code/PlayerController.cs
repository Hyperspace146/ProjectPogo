using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float GroundAcceleration = 12f;
    public float MaxGroundedSpeed = 15;
    public float Friction = 1;
    public float JumpHeight = 5000f;

    public Transform GroundCheck;
    public float GroundDist = 0.4f;
    public LayerMask GroundMask;

    [Tooltip("Change in angle of velocity per second when strafing (in degrees).")]
    public float MaxDeltaTheta = 0.0005f;

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
            print("jump");
            rb.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y));
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDist, GroundMask);

        if (isGrounded)
        {
            //rb.drag
            GroundMove();
        }
        else
        {
            AirStrafe();
        }
    }

    void GroundMove()
    {
        // Move the player according to directional input
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection.Normalize();

        rb.velocity += moveDirection * GroundAcceleration * Time.fixedDeltaTime;

        // Cap grounded speed
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxGroundedSpeed);

        // Apply friction in opposite direction of current velocity
        // Option 1: rb.AddForce(rb.velocity.normalized * -1 * Friction);
        // Try rigidbody drag maybe?

        // the *= makes us slow down very slow when we're initially moving very slow (i think). try straight subtracting from velocity instead?
        // but then we'll slow down very slowly if we're moving really fast. maybe that's what we want though?
        float speed = rb.velocity.magnitude;
        float frictionMultiplier = speed - (Friction * Time.fixedDeltaTime);
        frictionMultiplier = Mathf.Max(0, frictionMultiplier);
        if (speed > 0) frictionMultiplier /= speed;
        float fricX = rb.velocity.x * frictionMultiplier;
        float fricZ = rb.velocity.z * frictionMultiplier;
        rb.velocity = new Vector3(fricX, rb.velocity.y, fricZ);
    }

    void AirMove()
    {

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
        //float deltaTheta = MaxDeltaTheta * Time.fixedDeltaTime;

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
