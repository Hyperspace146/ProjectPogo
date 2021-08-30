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
    public int CurrentAmmo;
    [Tooltip("The time in seconds it takes to reload a rocket into the clip. Will reload passively," +
        "regardless of whether the player is currently shooting or not.")]
    public float ReloadTime = 0.5f;

    private float lastTimeShot;
    private bool currentlyReloading = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && CurrentAmmo > 0 && lastTimeShot + DelayBetweenShots < Time.time)
        {
            Shoot();
        }

        if (CurrentAmmo < ClipSize && !currentlyReloading)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }
    
    public void Shoot()
    {
        // Shoot if able to (not reloading, has ammo, and the time delay between shots has passed)
        if (lastTimeShot + DelayBetweenShots < Time.time)
        {
            // Decrease ammo by one shot
            CurrentAmmo -= 1;

            Vector3 lookDirection = Camera.main.ScreenPointToRay(Input.mousePosition).direction.normalized;
            GameObject rocket = Instantiate(RocketPrefab, ShootPoint.position, Quaternion.FromToRotation(Vector3.up, lookDirection));
            rocket.GetComponent<Rigidbody>().velocity = lookDirection * RocketSpeed;

            // have rocket despawn after a time
            StartCoroutine(DespawnRocket(rocket, TimeBeforeDespawn));

            // Update last time shot to now
            lastTimeShot = Time.time;
        }
    }

    public IEnumerator ReloadCoroutine()
    {
        currentlyReloading = true;
        // Wait for the specified reload time before refilling the clip
        yield return new WaitForSeconds(ReloadTime);
        CurrentAmmo++;
        currentlyReloading = false;
    }

    IEnumerator DespawnRocket(GameObject rocket, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(rocket);
    }
}
