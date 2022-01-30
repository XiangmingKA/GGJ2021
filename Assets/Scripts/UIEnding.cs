using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEnding : MonoBehaviour
{
    // Start is called before the first frame update
    AudioSource successSource;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        successSource = GetComponent<AudioSource>();
    }

    public void SuccessAudio()
    {
        //播放通关的音效
        successSource.Play();

    }
}
