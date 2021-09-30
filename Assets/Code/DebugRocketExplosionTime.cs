using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRocketExplosionTime : MonoBehaviour
{
    public float PreviousTimeToExplosion;

    public float AverageTimeToExplosion;

    private float lastShootTime;

    // Used in computing the average
    private float shootTimeSum;
    private int numTimesShot;

    public void RecordShootTime(float shootTime)
    {
        numTimesShot++;
        lastShootTime = shootTime;
    }

    public void RecordExplosionTime(float explosionTime)
    {
        float timeToExplosion = explosionTime - lastShootTime;

        PreviousTimeToExplosion = timeToExplosion;
        shootTimeSum += timeToExplosion;

        AverageTimeToExplosion = shootTimeSum / numTimesShot;
    }
}
