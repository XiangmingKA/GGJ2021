using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rebirth : MonoBehaviour
{
    public Transform rebirthPosition;

    public void DoRebirth()
    {
        transform.position = rebirthPosition.position;
    }
}
