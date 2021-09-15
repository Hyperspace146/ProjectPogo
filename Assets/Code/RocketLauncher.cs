using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Allows the player to launch a rocket in the direction that they click.
 */
public class RocketLauncher : MonoBehaviour
{
    [Tooltip("The position on the character model where the rockets will spawn from.")]
    public Transform ShootPoint;
    public GameObject RocketPrefab;
    public float RocketSpeed = 40f;
    [Tooltip("Time after shooting that before the rocket despawns.")]
    public float TimeBeforeDespawn = 10f;
    [Tooltip("The time in seconds that you must wait in before you can shoot again.")]
    public float DelayBetweenShots = 0.2f;
    [Tooltip("Max number of rockets that can be stored in the clip.")]
    public int ClipSize = 8;
    public float CurrentAmmo;
    [Tooltip("The time in seconds it takes to reload a rocket into the clip. Will reload passively," +
        "regardless of whether the player is currently shooting or not.")]
    public float ReloadTime = 0.5f;
    [Tooltip("Layermask that controls what objects will be detected when raycasting for a rocket target under" +
        "the crosshair.")]
    public LayerMask RocketRaycastTargetMask;
    [Tooltip("The max distance in world space units for the rocket raycast to find a target.")]
    public float MaxRocketTargetDistance = 1000;

    private float lastTimeShot;

    void Update()
    {
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.L)) && CurrentAmmo >= 1 && lastTimeShot + DelayBetweenShots < Time.time)
        {
            Shoot();
        }

        CurrentAmmo = Mathf.Min(ClipSize, CurrentAmmo + (Time.deltaTime / ReloadTime));
    }
    
    public void Shoot()
    {
        FindObjectOfType<AudioManager>().Play("Shoot");

        print("shoot time: " + Time.time);

        // Decrease ammo by one shot
        CurrentAmmo -= 1;

        // Find the point in world space where the player is aiming at with their crosshair
        RaycastHit hit;
        Vector3 rocketTargetPoint;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).direction, out hit, MaxRocketTargetDistance, RocketRaycastTargetMask))
        {
            rocketTargetPoint = hit.point;
        } 
        else
        {
            rocketTargetPoint = Camera.main.transform.position + Camera.main.ScreenPointToRay(Input.mousePosition).direction * MaxRocketTargetDistance;
        }

        // Now we find the vector pointing from the SHOOT POINT to where the player's pointing in world space
        Vector3 shootDirection = rocketTargetPoint - ShootPoint.position;
        shootDirection.Normalize();

        // Now spawn the rocket with velocity in that direction. The rotation calc makes sure it points its head in the direction of shooting.
        GameObject rocket = Instantiate(RocketPrefab, ShootPoint.position, Quaternion.FromToRotation(Vector3.up, shootDirection));
        rocket.GetComponent<Rigidbody>().velocity = shootDirection * RocketSpeed;

        // have rocket despawn after a time
        StartCoroutine(DespawnRocket(rocket, TimeBeforeDespawn));

        // Update last time shot to now
        lastTimeShot = Time.time;
    }

    IEnumerator DespawnRocket(GameObject rocket, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(rocket);
    }
}
