using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator UIStartPanelanim;
    //public AudioClip startClip;
    AudioSource startSource;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
 
            Debug.Log("Start");
            UIStartPanelanim.SetTrigger("Start");
        }

        startSource = GetComponent<AudioSource>();
    }

    public void StartAudio()
    {
        //播放开门的音效
        startSource.Play();

    }

    public void StartGame()
    {
        //销毁当前场景并开启下一个场景
        Debug.Log("Level 1");
    }





}
