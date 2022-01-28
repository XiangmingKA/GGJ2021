using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorTrigger : Interactable
{
    [Range(-1f, 1f)]
    public float sinkLength = 1f;

    [Range(0f, 1f)]
    public float sinkSpeed = 1f;

    public UnityEvent onTriggerOn;
    public UnityEvent onTriggerOff;

    Vector3 _originPos;
    Vector3 _sunkPos;
    int     _objOnTop = 0;

    // Start is called before the first frame update
    void Start()
    {
        _originPos  = this.transform.position;
        _sunkPos    = _originPos - Vector3.up * sinkLength;
    }

    // Update is called once per frame
    void Update()
    {
        if (_objOnTop > 0)
        {
            if ( (Vector3.Distance(this.transform.position, _sunkPos) > 0.001f))
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, _sunkPos, sinkSpeed * Time.deltaTime);
            }
        }
        else
        {
            if ((Vector3.Distance(this.transform.position, _originPos) > 0.001f))
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, _originPos, sinkSpeed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            onTriggerOn?.Invoke();
            _objOnTop++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            onTriggerOff?.Invoke();
            _objOnTop--;
        }
    }
}
