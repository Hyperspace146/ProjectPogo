using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDHealthText : MonoBehaviour
{
    public Health PlayerHealth;

    private Text healthText;

    private void Start()
    {
        healthText = GetComponent<Text>();
    }

    private void Update()
    {
        healthText.text = PlayerHealth.CurrentHealth.ToString();
    }
}
