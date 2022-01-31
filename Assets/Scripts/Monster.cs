using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class Monster : MonoBehaviour
{
    public Transform[] checkPoints;
    int _curVisitingIndex = 0;

    [Range(.1f, 10f)]
    public float movingSpeed = 1.0f;

    public static UnityEvent OnPlayerTouchedMonster = new UnityEvent();

    AudioSource audio;

    private bool facingLeft = true;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (checkPoints.Length > 0)
        {
            Transform targetTrans = checkPoints[_curVisitingIndex];
            if (Vector3.Distance(this.transform.position, targetTrans.position) > 0.01f)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, targetTrans.position, movingSpeed * Time.deltaTime);
            }
            else
            {
                _curVisitingIndex = (_curVisitingIndex + 1) % checkPoints.Length;
            }

            if ( transform.position.x - targetTrans.position.x < -0.001)
            {
                if (facingLeft)
                {
                    facingLeft = false;
                    GetComponent<SpriteRenderer>().flipX = true;
                }

            }
            else
            {
                if (!facingLeft)
                {
                    facingLeft = true;
                    GetComponent<SpriteRenderer>().flipX = false;
                }

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            OnPlayerTouchedMonster?.Invoke();
        }
        else if (collision.tag == "Stone")
        {
            Dead();
        }
    }

    bool isScheduledToDead = false;

    void Dead()
    {
        if (!isScheduledToDead)
        {
            isScheduledToDead = true;

            audio.PlayOneShot(audio.clip);

            StartCoroutine(WaitAndDestory());
        }
    }

    IEnumerator WaitAndDestory()
    {
        yield return new WaitForSeconds(2f);

        this.gameObject.SetActive(false);
    }

}
