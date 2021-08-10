using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * When this object collides with the environment or an enemy, instantiate an explosion at the point of contact. 
 */
public class DetonateOnContact : MonoBehaviour
{
    public GameObject ExplosionPrefab;

    /*private void OnTriggerEnter(Collider other)
    {
        // Use a raycast to find the point of contact, then spawn a rocket at that point
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit);

        Instantiate(ExplosionPrefab, hit.point, Quaternion.identity);
        Destroy(this);
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(ExplosionPrefab, collision.GetContact(0).point, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
