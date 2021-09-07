using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastRocketCollision : MonoBehaviour
{
    [Tooltip("Determines what layers the rockets can collide with.")]
    public LayerMask RocketCollisionLayerMask;

    public GameObject ExplosionPrefab;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Raycast forward in the direction the rocket is moving every physics step.
        // The raycast will only be as long as the distance the rocket will move every physics step.
        Vector3 rocketDirection = rb.velocity.normalized;
        float rocketTravelDistance = rb.velocity.magnitude * Time.fixedDeltaTime;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, rocketDirection, out hit, rocketTravelDistance, RocketCollisionLayerMask))
        {
            print("Raycast hit time: " + Time.time);

            // Detonate if the raycast predicts the rocket will collide with something
            Instantiate(ExplosionPrefab, hit.point, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
