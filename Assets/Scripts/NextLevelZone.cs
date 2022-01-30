using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelZone : MonoBehaviour
{
    public string nextLevelName = "";
    public Collider2D player1;
    public Collider2D player2;

    private HashSet<Collider2D> curColliders = new HashSet<Collider2D>();

    private void Update()
    {
        Debug.Log(curColliders.Count);
        if (curColliders.Contains(player1) && curColliders.Contains(player2))
        {
            SceneManager.LoadScene(nextLevelName);
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        curColliders.Add(other);
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        curColliders.Remove(other);
    }
}
