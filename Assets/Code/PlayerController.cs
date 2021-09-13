using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Choose which type of air movement implementation to use.")]
    public int AirMoveType = 0;
    [Tooltip("How fast the player will accelerate/decelerate in the air.")]
    public float AirAcceleration = 12;
    [Tooltip("The percentage of the current air speed that will be added to base air acceleration.")]
    [Range(0, 1)]
    public float AirAccelerationScaling = 0.5f;

    public float GroundAcceleration = 12;
    public float MaxGroundedSpeed = 15;
    public float Friction = 1;
    [Tooltip("The length of time in seconds after landing during which friction is temporarily not applied.")]
    public float FrictionBuffer = 0.1f;
    public float JumpHeight = 5000f;

    public Transform GroundCheck;
    public float GroundDist = 0.4f;
    public LayerMask GroundMask;

    [Tooltip("Affects how sharp a turn the player will make when strafing.")]
    public float StrafeSpeed = 50;

    [Tooltip("Maximum angle in degrees that you can strafe in one second.")]
    public float MaxStrafeAnglePerSecond = 180;

    private bool isGrounded;
    public bool applyFriction = false;
    private Vector2 lastLookDirection;
    private float frictionBufferTimer = 0;

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
        frictionBufferTimer += Time.fixedDeltaTime;
        bool groundCheck = Physics.CheckSphere(GroundCheck.position, GroundDist, GroundMask);

        // If we're not grounded, don't apply friction
        if (!groundCheck)
        {
            applyFriction = false;
        }
        // Or if we ARE grounded but was airborne in the last physics step (meaning isGrounded was false),
        // reset the friction timer because we've just landed on the ground. The timer will wait a certain time before applying friction
        else if (!isGrounded)
        {
            frictionBufferTimer = 0;
        }
        // Or if we were grounded for at least the length of time of the friction buffer, apply friction
        else if (frictionBufferTimer >= FrictionBuffer)
        {
            applyFriction = true;
        }

        isGrounded = groundCheck;

        if (applyFriction)
        {
            GroundMove();
        }
        if (!isGrounded)
        {
            switch (AirMoveType)
            {
                case 0:
                    AirStrafe();
                    break;
                case 1:
                    AirMove1();
                    break;
                case 2:
                    AirMove2();
                    break;
            }
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
     * Strafe based on the change in angle of look direction.
     */ 
    void AirStrafe()
    {
        // Using the dot product equation, calculate the change in the angle of the current look direction in the x-z plane.
        // The change in angle will be negative if turning clockwise, positive if counter-clockwise.
        Vector2 currentLookDirection = new Vector2(transform.forward.x, transform.forward.z);
        float changeInLookAngle = Vector2.Dot(currentLookDirection, lastLookDirection);
        changeInLookAngle /= (lastLookDirection.magnitude * currentLookDirection.magnitude);
        changeInLookAngle = Mathf.Clamp01(changeInLookAngle);  // Prevents NaN errors when doing arccos below.
        changeInLookAngle = Mathf.Acos(changeInLookAngle);

        lastLookDirection = currentLookDirection;

        // Collect horizontal "a" and "d" key inputs
        float x = Input.GetAxis("Horizontal");

        // Delta theta is the change in angle that the player's velocity will undergo for this physics step as a
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

    /*
     * In TF2, when in the air and pressing WASD seems a force is applied in the direction of
       the WASD input relative to the player's current look direction.

       When strafing by holding A or D with the look direction aligned with the current velocity,
       the air movement force will be perpendicular with the current velocity, and the resulting velocity
       will have been rotated to the left or right. (Note that the magnitude of the resulting velocity may 
       be clamped as to not surpass the previous velocity).
     */
    void AirMove1()
    {
        // Find the direction of the air movement force based on WASD player input
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);  // Make the input vector relative to current look direction
        moveDirection.Normalize();

        // Add this horizontal air movement force to the player's horizontal velocity
        Vector3 airMovementForce = moveDirection * Time.fixedDeltaTime * AirAcceleration 
            * (rb.velocity.magnitude * AirAccelerationScaling);
        Vector3 prevHoriVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 newHoriVelocity = prevHoriVelocity + airMovementForce;

        // Clamp the magnitude of the horizontal velocity to not be greater than it was before
        newHoriVelocity = Vector3.ClampMagnitude(newHoriVelocity, prevHoriVelocity.magnitude);

        rb.velocity = new Vector3(newHoriVelocity.x, rb.velocity.y, newHoriVelocity.z);
    }

    /*
     * Same as above, applying an air movement force in the direction of WASD input, but now the force's magnitude
     * scales with the change in angle of look direction.
     */ 
    void AirMove2()
    {
        // Find the direction of the air movement force based on WASD player input
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);  // Make the input vector relative to current look direction
        moveDirection.Normalize();

        // Using the dot product equation, calculate the change in the angle of the current look direction in the x-z plane.
        // The change in angle will be negative if turning clockwise, positive if counter-clockwise.
        Vector2 currentLookDirection = new Vector2(transform.forward.x, transform.forward.z);
        float changeInLookAngle = Vector2.Dot(currentLookDirection, lastLookDirection);
        changeInLookAngle /= (lastLookDirection.magnitude * currentLookDirection.magnitude);
        changeInLookAngle = Mathf.Clamp01(changeInLookAngle);  // Prevents NaN errors when doing arccos below.
        changeInLookAngle = Mathf.Acos(changeInLookAngle);

        lastLookDirection = currentLookDirection;

        // Add this horizontal air movement force to the player's horizontal velocity. Scales with change in look direction.
        Vector3 airMovementForce = moveDirection * Time.fixedDeltaTime * AirAcceleration
            * (rb.velocity.magnitude * AirAccelerationScaling) * changeInLookAngle;
        Vector3 prevHoriVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 newHoriVelocity = prevHoriVelocity + airMovementForce;

        // Clamp the magnitude of the horizontal velocity to not be greater than it was before
        newHoriVelocity = Vector3.ClampMagnitude(newHoriVelocity, prevHoriVelocity.magnitude);

        rb.velocity = new Vector3(newHoriVelocity.x, rb.velocity.y, newHoriVelocity.z);
    }
}
