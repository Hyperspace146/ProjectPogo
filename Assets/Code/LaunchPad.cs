using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    [Tooltip("The vector from the launch pad's position to this transform's position represents the " +
        "launch trajectory of this launch pad.")]
    public Transform LaunchTrajectory;

    public float LaunchStrength = 500;
    
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody otherRB = other.gameObject.GetComponent<Rigidbody>();
        if (otherRB != null)
        {
            otherRB.velocity = Vector3.zero;
            Vector3 launchForce = LaunchTrajectory.position - transform.position;
            launchForce *= LaunchStrength;
            otherRB.AddForce(launchForce);
        }
    }
}
