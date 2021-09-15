using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDSpeedText : MonoBehaviour
{
    public Rigidbody PlayerRigidbody;

    private TMPro.TMP_Text speedText;

    private void Start()
    {
        speedText = GetComponent<TMPro.TMP_Text>();
    }

    private void Update()
    {
        speedText.text = new Vector2(PlayerRigidbody.velocity.x, PlayerRigidbody.velocity.z).magnitude.ToString("f2");  // f2 limits the number to 2 decimal points
    }
}
