using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public bool belongsToDownWorld = true;

    private Rigidbody2D _rigidbody2D;
    private float thrownHeight;

    public void Grabbed(PlayerController player)
    {
        belongsToDownWorld = player.belongsToDownWorld;

        transform.SetParent(player.holdingPos.transform);
        transform.localPosition = Vector3.zero;

        //_rigidbody2D.gravityScale = belongsToDownWorld ? 1f : -1f;
        _rigidbody2D.simulated = false;

        thrownHeight = player.stoneThrowHeight;
    }

    public void Thrown()
    {
        transform.SetParent(null);

        _rigidbody2D.simulated = true;

        Vector2 velocity = _rigidbody2D.velocity;
        velocity.y += (_rigidbody2D.gravityScale > 0 ? 1 : -1) *
                          Mathf.Sqrt(-2f * Physics.gravity.y * Mathf.Abs(_rigidbody2D.gravityScale) * thrownHeight);
        _rigidbody2D.velocity = velocity;
    }

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
