using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Stone : MonoBehaviour
{
    public bool belongsToDownWorld = true;
    public bool holdingPlayerBelongsToDownWorld = true;

    private Rigidbody2D _rigidbody2D;
    private float thrownHeight;

    AudioSource audio;

    public void Grabbed(PlayerController player)
    {
        holdingPlayerBelongsToDownWorld = player.belongsToDownWorld;

        transform.SetParent(player.holdingPos.transform);
        transform.localPosition = Vector3.zero;

        _rigidbody2D.gravityScale = belongsToDownWorld ? 1f : -1f;
        _rigidbody2D.simulated = false;

        thrownHeight = player.stoneThrowHeight;
    }

    public void Thrown()
    {
        transform.SetParent(null);

        _rigidbody2D.simulated = true;

        Vector2 velocity = _rigidbody2D.velocity;
        velocity.y += (holdingPlayerBelongsToDownWorld ? 1 : -1) *
                          Mathf.Sqrt(-2f * Physics.gravity.y * Mathf.Abs(_rigidbody2D.gravityScale) * thrownHeight);
        _rigidbody2D.velocity = velocity;

        if (!audio.isPlaying)
            audio.PlayOneShot(audio.clip);
    }

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

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
        
    }
}
