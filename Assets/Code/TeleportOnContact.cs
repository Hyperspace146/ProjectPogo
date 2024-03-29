﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportOnContact : MonoBehaviour
{
    public Transform TeleportDestination;

    private void OnCollisionEnter(Collision collision)
    {
        FindObjectOfType<AudioManager>().Play("Death");
        collision.transform.position = TeleportDestination.position;
        collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
