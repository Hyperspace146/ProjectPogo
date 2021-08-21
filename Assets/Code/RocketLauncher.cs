using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Allows the player to launch a rocket in the direction that they click.
 */
public class RocketLauncher : MonoBehaviour
{
    [Tooltip("The position on the character model where the rockets will spawn from.")]
    public Transform ShootPoint;
    public GameObject RocketPrefab;
    public float RocketSpeed;
    [Tooltip("Time after shooting that before the rocket despawns.")]
    public float TimeBeforeDespawn;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 lookDirection = Camera.main.ScreenPointToRay(Input.mousePosition).direction.normalized;
            GameObject rocket = Instantiate(RocketPrefab, ShootPoint.position, Quaternion.FromToRotation(Vector3.up, lookDirection)/*Quaternion.LookRotation(Vector3.up, lookDirection)*/);
            rocket.GetComponent<Rigidbody>().velocity = lookDirection * RocketSpeed;

            // have rocket despawn after a time
            StartCoroutine(DespawnRocket(rocket, TimeBeforeDespawn));
        }
    }

    IEnumerator DespawnRocket(GameObject rocket, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(rocket);
    }
}
