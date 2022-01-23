using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public KeyCode leftButton;
    public KeyCode rightButton;
    public KeyCode jumpButton;
    public KeyCode interactButton;

    
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    [SerializeField, Range(0f, 2f)]
    float jumpHeight = 1f;
    
    [ReadOnly] public Vector2 velocity;
    [ReadOnly] public float desiredHorizontalSpeed;
    [ReadOnly] public bool onGround;
    [ReadOnly] public bool desiredJump;

    private Rigidbody2D _rigidbody2D;
    
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        desiredHorizontalSpeed = ((Input.GetKey(leftButton) ? -1 : 0) + (Input.GetKey(rightButton) ? 1 : 0)) * maxSpeed;
        desiredJump |= Input.GetKey(jumpButton) ;
    }

    private void FixedUpdate()
    {
        velocity = _rigidbody2D.velocity;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x =
            Mathf.MoveTowards(velocity.x, desiredHorizontalSpeed, maxSpeedChange);
        
        if (desiredJump) {
            desiredJump = false;
            Jump();
        }
        _rigidbody2D.velocity = velocity;
        onGround = false;
    }
    
    void OnCollisionEnter2D (Collision2D collision) {
        EvaluateCollision(collision);
    }

    void OnCollisionStay2D (Collision2D collision) {
        EvaluateCollision(collision);
    }

    void EvaluateCollision (Collision2D collision) {
        for (int i = 0; i < collision.contactCount; i++) {
            Vector2 normal = collision.GetContact(i).normal;
            onGround |= (normal.y * (_rigidbody2D.gravityScale>0?1:-1)) >= 0.9f;
        }
    }
    
    void Jump () {
        if (onGround)
        {
            velocity.y += (_rigidbody2D.gravityScale > 0 ? 1 : -1) *
                          Mathf.Sqrt(-2f * Physics.gravity.y * Mathf.Abs(_rigidbody2D.gravityScale) * jumpHeight);
        }
    }
    
}
