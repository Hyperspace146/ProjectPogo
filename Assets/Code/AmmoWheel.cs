using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoWheel : MonoBehaviour
{
    [Tooltip("The player's rocket launcher. Provides information about current ammo.")]
    public RocketLauncher rocketLauncher;

    private Image img;
    
    void Start()
    {
        img = GetComponent<Image>();
        img.type = Image.Type.Filled;
        img.fillMethod = Image.FillMethod.Radial360;
        img.fillOrigin = 2;
        img.fillClockwise = false;
    }

    void Update()
    {
        img.fillAmount = rocketLauncher.CurrentAmmo / rocketLauncher.ClipSize;
    }
}
