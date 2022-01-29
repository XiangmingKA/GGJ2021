using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public Transform rebirthPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.transform.position = rebirthPosition.position;
    }
}
