using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

public class ReadOnlyAttribute : PropertyAttribute
{
 
}
 
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
        GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
 
    public override void OnGUI(Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public KeyCode leftButton;
    public KeyCode rightButton;
    public KeyCode jumpButton;
    public KeyCode interactButton;

    public PlayerController otherPlayerController;
    
    public Vector2 distanceThreshold = new Vector2(0.1f, 0.2f);
    
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
    [ReadOnly] public bool canHook=false;
    [ReadOnly] public float distToGround = 0.0f;
    [ReadOnly] public bool distanceCheck = false;
    [ReadOnly] public bool hooked = false;
    

    private Rigidbody2D _rigidbody2D;
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        distToGround = GetComponent<Collider2D>().bounds.extents.y;
    }

    void Update()
    {
        desiredHorizontalSpeed = ((Input.GetKey(leftButton) ? -1 : 0) + (Input.GetKey(rightButton) ? 1 : 0)) * maxSpeed;
        desiredJump |= Input.GetKey(jumpButton);
        canHook = Input.GetKey(interactButton);
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, -Vector2.up * (_rigidbody2D.gravityScale > 0 ? 1 : -1),
            distToGround + 0.1f);
        return hit.collider != null;
    }

    private bool DistanceCheck()
    {
        return Mathf.Abs(transform.position.y - otherPlayerController.transform.position.y +
                         distToGround * 2 * (_rigidbody2D.gravityScale > 0 ? 1 : -1)) < distanceThreshold.y &&
               Mathf.Abs(transform.position.x - otherPlayerController.transform.position.x) < distanceThreshold.x;
    }
    private void FixedUpdate()
    {
        velocity = _rigidbody2D.velocity;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        if (onGround)
        {
            velocity.x =
                Mathf.MoveTowards(velocity.x, desiredHorizontalSpeed, maxSpeedChange);
        }
        
        if (desiredJump) {
            desiredJump = false;
            Jump();
        }

        distanceCheck = DistanceCheck();
        if (canHook && otherPlayerController.canHook && distanceCheck)
        {
            if (!hooked)
            {
                hooked = true;
                // _velocityBackup = velocity;
            }

            velocity.y = 0;
            //Hook!
            _rigidbody2D.constraints |= RigidbodyConstraints2D.FreezePositionY;
        }
        else
        {
            if (hooked)
            {
                hooked = false;
                
            }
            _rigidbody2D.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
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
