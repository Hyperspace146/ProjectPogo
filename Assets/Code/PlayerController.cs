using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 12f;
    public float jumpHeight = 5000f;

    public Transform groundCheck;
    public float groundDist = 0.4f;
    public LayerMask groundMask;

    [Tooltip("Change in angle of velocity per physics step when strafing (in degrees).")]
    public float deltaTheta = 1;

    public PhysicMaterial FrictionMaterial;

    public bool isGrounded;

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
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y));
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask);

        if (isGrounded)
            GroundMove();
        else
            AirMove();
    }

    void GroundMove()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        rb.MovePosition(rb.position + move * speed * Time.deltaTime);

        // Apply friction only when the player is grounded
        collider.material = isGrounded ? FrictionMaterial : null;
    }

    void AirMove()
    {
        float x = Input.GetAxis("Horizontal");
        float theta;
        if (x > 0)
        {
            theta = deltaTheta * -1;
        }
        else if (x < 0)
        {
            theta = deltaTheta;
        }
        else
        {
            return;
        }
        theta *= Mathf.Rad2Deg;

        Vector3 newVelocity = new Vector3(0, rb.velocity.y, 0);

        newVelocity.x = Mathf.Cos(theta) * rb.velocity.x + -Mathf.Sin(theta) * rb.velocity.z;
        newVelocity.z = Mathf.Sin(theta) * rb.velocity.x + Mathf.Cos(theta) * rb.velocity.z;

        rb.velocity = newVelocity;
    }
}
