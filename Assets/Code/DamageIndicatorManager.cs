using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicatorManager : MonoBehaviour
{
    [Tooltip("The UI element that be parent to all the damage indicators.")]
    public GameObject DamageIndicatorHolder;
    public GameObject DamageIndicatorPrefab;
    public float IndicatorDespawnTime = 0.7f;

    // Create a damage indicator and rotate it to the direction that the damage came from
    public void CreateDamageIndicator(int damage, Vector3 damageContactPoint)
    {
        Vector2 directionOfDamage = Camera.main.WorldToViewportPoint(damageContactPoint) - new Vector3(0.5f, 0.5f);
        print(directionOfDamage);
        GameObject di = Instantiate(DamageIndicatorPrefab, DamageIndicatorHolder.transform);
        di.transform.Rotate(Vector3.forward, Vector2.SignedAngle(directionOfDamage, Vector2.right));
        StartCoroutine(WaitToDespawnDamageIndicator(di));
    }

    IEnumerator WaitToDespawnDamageIndicator(GameObject di)
    {
        yield return new WaitForSeconds(IndicatorDespawnTime);
        Destroy(di);
    }
}
