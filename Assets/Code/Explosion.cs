using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Applies knockback and damage based on the distance of the colliding object from the center of this object.
 * Assumes this object's collider is spherical.
 */
public class Explosion : MonoBehaviour
{
    public bool DebugMode = false;

    [Tooltip("Maximum magnitude of explosion knockback, which occurs near the center of the explosion.")]
    public float MaxExplosionKnockback;

    [Tooltip("The percentage of the maximum explosion knockback that will occur at the edge of the explosion.")]
    [Range(0, 1)]
    public float MinExplosionKnockbackRatio;

    [Tooltip("Magnitude of the extra upward lift caused by the explosion.")]
    public float UpwardLiftModifier;

    [Tooltip("Time in seconds the explosion will last.")]
    public float ExplosionDuration;

    [Tooltip("The maximum damage (as a positive value) that the explosion can deal. Max damage occurs when " +
        "an object is near to its center).")]
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
            // manual implementation of explosion kb. has issues when player collider is inside the center of the explosion

            /*Vector3 explosionContactPoint = other.ClosestPointOnBounds(transform.position);
            Vector3 explosionForceDirection = (explosionContactPoint - transform.position).normalized;

            // Set up a multiplier that inversely scales based on the object's distance from the center of the explosion
            float distanceFromCenter = Vector3.Distance(explosionContactPoint, transform.position);
            float explosionRadius = transform.lossyScale.x * GetComponent<SphereCollider>().radius;
            float distanceMultiplier = 1 - (distanceFromCenter / explosionRadius);

            // Using the multiplier calculate the final magnitude of the explosion force on the object
            float explosionKnockback = distanceMultiplier * (MaxExplosionKnockback - MinExplosionKnockback) + MinExplosionKnockback;
            Vector3 explosionForce = explosionForceDirection * explosionKnockback;

            print(explosionForce);
            collidingRB.AddForceAtPosition(explosionForce, explosionContactPoint, ForceMode.Impulse);*/
            

            // We need the scale of the gameobject so we can calculate the radius of the explosion's collider.
            // In finding the scale, we assume the scale in each axis is the same (since the explosion is spherical), which
            // is what we're checking below
            if (transform.localScale.x != transform.localScale.y || transform.localScale.y != transform.localScale.z)
            {
                Debug.LogError("Explosion object does not have the same scale value for the x, y, and z axes.");
            }

            // Apply explosion knockback. We make the explosion radius bigger than the collider radius so that we
            // guarantee a minimum amount of knockback when colliding with the very edge of the collider
            float colliderRadius = (GetComponent<SphereCollider>().radius * transform.localScale.x);
            float explosionRadius = colliderRadius / (1 - MinExplosionKnockbackRatio);
            collidingRB.AddExplosionForce(MaxExplosionKnockback, transform.position, explosionRadius, UpwardLiftModifier, ForceMode.Impulse);

            FindObjectOfType<DebugRocketExplosionTime>().RecordExplosionTime(Time.time);

            // Apply damage to non-player objects that decreases based on distance from the center of the explosion
            if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
                return;
            }

            Vector3 explosionContactPoint = other.ClosestPointOnBounds(transform.position);
            float distance = Vector3.Distance(explosionContactPoint, transform.position);
            float damageRatio = 1.0F - Mathf.Clamp01(distance / colliderRadius);
            int damage = (int) (MaxDamage * damageRatio);

            Health victimHealth = other.GetComponent<Health>();
            if (victimHealth != null)
            {
                victimHealth.ChangeHealth(-damage);
            }
        }
    }
}
