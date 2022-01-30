using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DeadZone : MonoBehaviour
{
    [FormerlySerializedAs("rebirthPosition")] public Transform defaultRebirthPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.GetComponent<Rebirth>())
        {
            other.transform.GetComponent<Rebirth>().DoRebirth();
        }
        else
        {
            other.transform.position = defaultRebirthPosition.position;
        }
    }
}
