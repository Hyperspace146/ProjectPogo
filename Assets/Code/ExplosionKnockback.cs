using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionKnockback : MonoBehaviour
{
    [Tooltip("Magnitude of the explosion knockback.")]
    public float ExplosionForce;

    [Tooltip("Time the explosion will last.")]
    public float ExplosionDuration;

    private void Start()
    {
        StartCoroutine(DestroyAfterTime(ExplosionDuration));
    }

    IEnumerator DestroyAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        print("hi");
        Rigidbody collidingRB = other.gameObject.GetComponent<Rigidbody>();
        if (collidingRB != null)
        {
            collidingRB.AddExplosionForce(ExplosionForce, transform.position, 
                GetComponent<SphereCollider>().radius * transform.lossyScale.x, 0f, ForceMode.Impulse);
        }
    }
}
