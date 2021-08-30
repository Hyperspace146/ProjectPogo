using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDCurrentAmmoText : MonoBehaviour
{
    public RocketLauncher rocketLauncher;

    private Text currentAmmoText;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmoText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        currentAmmoText.text = rocketLauncher.CurrentAmmo.ToString();
    }
}
