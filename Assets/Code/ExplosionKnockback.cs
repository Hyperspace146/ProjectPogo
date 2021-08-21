using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionKnockback : MonoBehaviour
{
    [Tooltip("Minimum magnitude of explosion knockback. Occurs when at the edge of the explosion radius.")]
    public float MinExplosionKnockback;

    [Tooltip("Maximum magnitude of explosion knockback. Occurs when near the center of the explosion.")]
    public float MaxExplosionKnockback;

    [Tooltip("Time in seconds the explosion will last.")]
    public float ExplosionDuration;

    [Tooltip("The maximum damage the explosion can deal (which occurs when an object is near to its center).")]
    public float MaxDamage;

    private void Start()
    {
        StartCoroutine(DestroyAfterTime(ExplosionDuration));
    }

    IEnumerator DestroyAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }

    // "other" represents the object (e.g. player or enemy) colliding with this explosion hitbox
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody collidingRB = other.gameObject.GetComponent<Rigidbody>();
        if (collidingRB != null)
        {
            print("hi");
            Vector3 explosionContactPoint = other.ClosestPointOnBounds(transform.position);
            Vector3 explosionForceDirection = (explosionContactPoint - transform.position).normalized;

            // Set up a multiplier that inversely scales based on the object's distance from the center of the explosion
            float distanceFromCenter = Vector3.Distance(explosionContactPoint, transform.position);
            float explosionRadius = transform.lossyScale.x * GetComponent<SphereCollider>().radius;
            float distanceMultiplier = 1 - (distanceFromCenter / explosionRadius);

            // Using the multiplier calculate the final magnitude of the explosion force on the object
            float explosionKnockback = distanceMultiplier * (MaxExplosionKnockback - MinExplosionKnockback) + MinExplosionKnockback;
            Vector3 explosionForce = explosionForceDirection * explosionKnockback;

            print(explosionForce);
            collidingRB.AddForceAtPosition(explosionForce, explosionContactPoint, ForceMode.Impulse);
            //collidingRB.AddExplosionForce(ExplosionForce, transform.position, <SphereCollider>().radius * transform.lossyScale.x, 0f, ForceMode.Impulse);
        }
    }
}
