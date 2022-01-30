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
        _originPos  = this.transform.localPosition;
        _sunkPos    = _originPos - Vector3.up * sinkLength;
    }

    // Update is called once per frame
    void Update()
    {
        //_originPos = this.transform.localPosition;
        //_sunkPos = _originPos - Vector3.up * sinkLength;

        if (_objOnTop > 0)
        {
            if ( (Vector3.Distance(this.transform.localPosition, _sunkPos) > 0.001f))
            {
                this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, _sunkPos, sinkSpeed * Time.deltaTime);
            }
        }
        else
        {
            if ((Vector3.Distance(this.transform.localPosition, _originPos) > 0.001f))
            {
                this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, _originPos, sinkSpeed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" || other.tag == "Stone")
        {
            onTriggerOn?.Invoke();
            _objOnTop++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" || other.tag == "Stone")
        {
            onTriggerOff?.Invoke();
            _objOnTop--;
        }
    }
}
