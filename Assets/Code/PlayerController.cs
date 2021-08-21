using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 12f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDist = 0.4f;
    public LayerMask groundMask;

    public PhysicMaterial FrictionMaterial;

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
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y));
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        rb.MovePosition(rb.position + move * speed * Time.deltaTime);

        // Apply friction only when the player is grounded
        collider.material = isGrounded ? FrictionMaterial : null;
    }
}
