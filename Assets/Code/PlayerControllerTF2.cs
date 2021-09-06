using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerTF2 : MonoBehaviour
{
    public float AirAcceleration = 12;
    public float GroundAcceleration = 12;
    public float MaxGroundedSpeed = 15;
    public float Friction = 1;
    public float JumpHeight = 5000f;

    public Transform GroundCheck;
    public float GroundDist = 0.4f;
    public LayerMask GroundMask;

    [Tooltip("Affects how sharp a turn the player will make when strafing.")]
    public float StrafeSpeed = 50;

    [Tooltip("Maximum angle in degrees that you can strafe in one second.")]
    public float MaxStrafeAnglePerSecond = 180;

    private bool isGrounded;
    private Vector2 lastLookDirection;

    private Rigidbody rb;
    private Collider collider;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        lastLookDirection = new Vector2(transform.forward.x, transform.forward.z);
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

    /*
     * In TF2, when in the air and pressing WASD seems a force is applied in the direction of
       the WASD input relative to the player's current look direction.

       When strafing by holding A or D with the look direction aligned with the current velocity,
       the air movement force will be perpendicular with the current velocity, and the resulting velocity
       will have been rotated to the left or right. (Note that the magnitude of the resulting velocity may 
       be clamped as to not surpass the previous velocity).
     */
    void AirMove()
    {
        // Find the direction of the air movement force based on WASD player input
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);  // Make the input vector relative to current look direction
        moveDirection.Normalize();

        // Add this horizontal air movement force to the player's horizontal velocity
        Vector3 airMovementForce = moveDirection * Time.fixedDeltaTime * AirAcceleration;
        Vector3 prevHoriVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 newHoriVelocity = prevHoriVelocity + airMovementForce;

        // Clamp the magnitude of the horizontal velocity to not be greater than it was before
        newHoriVelocity = Vector3.ClampMagnitude(newHoriVelocity, prevHoriVelocity.magnitude);

        rb.velocity = new Vector3(newHoriVelocity.x, rb.velocity.y, newHoriVelocity.z);
    }

    void AirStrafe()
    {
        // Using the dot product equation, calculate the change in the angle of the current look direction in the x-z plane.
        // The change in angle will be negative if turning clockwise, positive if counter-clockwise.
        Vector2 currentLookDirection = new Vector2(transform.forward.x, transform.forward.z);
        float changeInLookAngle = Vector2.Dot(currentLookDirection, lastLookDirection);
        changeInLookAngle /= (lastLookDirection.magnitude * currentLookDirection.magnitude);
        changeInLookAngle = Mathf.Acos(changeInLookAngle);

        //print("last: " + lastLookDirection);
        //print("current: " + currentLookDirection);

        //print(changeInLookAngle);

        lastLookDirection = currentLookDirection;

        // Collect horizontal "a" and "d" key inputs
        float x = Input.GetAxis("Horizontal");

        // Delta theta is the change in angle that the player's velocity will undergo this physics step as a 
        // result of strafing
        float deltaTheta = changeInLookAngle * StrafeSpeed * Time.fixedDeltaTime;

        if (x > 0)
        {
            deltaTheta *= -1;
        }

        // Prevent strafing if they've strafed at too harsh an angle, or if they didn't press any strafe buttons
        if (Mathf.Abs(changeInLookAngle) * Mathf.Rad2Deg > MaxStrafeAnglePerSecond || x == 0)
        {
            return;
        }

        print(deltaTheta);

        // Instantiation of empty, to be adjusted, velocity vector
        Vector3 newVelocity = new Vector3(0, rb.velocity.y, 0);

        // Rotate our current velocity vector (using matrix multiplication with the rotation matrix)
        // by theta degrees. The magnitude of our velocity vector stays the same after rotation
        newVelocity.x = Mathf.Cos(deltaTheta) * rb.velocity.x + -Mathf.Sin(deltaTheta) * rb.velocity.z;
        newVelocity.z = Mathf.Sin(deltaTheta) * rb.velocity.x + Mathf.Cos(deltaTheta) * rb.velocity.z;

        // Update rigidbody with newly rotated velocity vector
        rb.velocity = newVelocity;
    }
}
