using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Door : Interactable
{
    public enum DoorStatus
    {
        Open,
        Close
    }

    public DoorStatus doorStatus = DoorStatus.Close;

    public Transform openedPositon;
    public Transform closedPositon;

    [Range(.1f, 10f)]
    public float movingSpeed = 1.0f;
    [Range(.1f, 10f)]
    public float rotationSpeed = 1.0f;

    AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = doorStatus == DoorStatus.Open ? openedPositon.position : closedPositon.position;
        this.transform.rotation = doorStatus == DoorStatus.Open ? openedPositon.rotation : closedPositon.rotation;

        audio = GetComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckTransform(doorStatus == DoorStatus.Open ? openedPositon : closedPositon);
    }

    public void DoorOpen()
    {
        doorStatus = DoorStatus.Open;
    }

    public void DoorClose()
    {
        doorStatus = DoorStatus.Close;
    }

    void CheckTransform(Transform target)
    {
        if (Vector3.Distance(this.transform.position, target.position) > 0.001f)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, target.position, movingSpeed * Time.deltaTime);

            if (!audio.isPlaying)
                audio.PlayOneShot(audio.clip);
        }
        if (Quaternion.Angle(this.transform.rotation, target.rotation) > 0.001f)
        {
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, target.rotation, rotationSpeed * Time.deltaTime);
        }
    }
}
