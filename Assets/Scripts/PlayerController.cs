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
    public bool belongsToDownWorld = true;

    public KeyCode leftButton;
    public KeyCode rightButton;
    public KeyCode jumpButton;
    public KeyCode interactButton;

    public Animator animator;
    public PlayerController otherPlayerController;

    public GameObject holdingPos;

    public Vector2 distanceThreshold = new Vector2(0.1f, 0.2f);
    
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    [SerializeField, Range(0f, 2f)]
    float jumpHeight = 1f;

    [SerializeField, Range(0f, 100f)]
    public float stoneThrowHeight = 3f;

    [SerializeField, Range(0f, 90f)]
    float maxGroundAngle = 25f;
    
    [SerializeField, Range(0f, 5f)]
    float forceUnhookTime = 1.0f;
    
    [ReadOnly] public Vector2 velocity;
    [ReadOnly] public float desiredHorizontalSpeed;
    [ReadOnly] public bool onGround;
    [ReadOnly] public bool desiredJump;
    [ReadOnly] public bool canHook=false;
    [ReadOnly] public bool forceUnhook=false;
    [ReadOnly] public float distToGround = 0.0f;
    [ReadOnly] public bool distanceCheck = false;
    [ReadOnly] public bool hooked = false;
    [ReadOnly] public Vector2 contactNormal;

    private Rigidbody2D _rigidbody2D;
    private float minGroundDotProduct;
    private GameObject stoneObj;
    private bool canThrow = false;
    
    void OnValidate () {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }
    void Awake () {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        OnValidate();
    }
    private void Start()
    {
        distToGround = GetComponent<Collider2D>().bounds.extents.y;
    }

    void Update()
    {
        desiredHorizontalSpeed = ((Input.GetKey(leftButton) ? -1 : 0) + (Input.GetKey(rightButton) ? 1 : 0)) * maxSpeed;
        desiredJump |= Input.GetKey(jumpButton);
        canHook = Input.GetKey(interactButton);

        HandleThrowStone();
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
        UpdateState();
        AdjustVelocity();
        
        if (desiredJump) {
            desiredJump = false;
            Jump();
        }

        UpdateHook();

        _rigidbody2D.velocity = velocity;
        UpdateAnimation();
        onGround = false;
    }
    private void UpdateAnimation()
    {
        if(!animator) return;
        var scale = animator.gameObject.transform.localScale;
        if (velocity.x > 1e-3 || velocity.x<-1e-3)
        {
            animator.gameObject.transform.localScale = new Vector3(Mathf.Abs(scale.x)*(velocity.x > 0?1:-1)*(belongsToDownWorld?1:-1), scale.y, scale.z);
        }
        
        if (!onGround)
        {
            if (hooked)
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isRunning",false);
                animator.SetBool("isFlying",true);
            }
            else
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isRunning",false);
                animator.SetBool("isFlying",false);
            }
        }
        else
        {
            if (velocity.magnitude > 0.1f)
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isRunning",true);
                animator.SetBool("isFlying",false);
            }
            else
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isRunning",false);
                animator.SetBool("isFlying",false);
            }
        }
    }
    private void UpdateHook()
    {
        distanceCheck = DistanceCheck();
        if (canHook && otherPlayerController.canHook && distanceCheck && !forceUnhook)
        {
                if (!hooked)
                {
                    hooked = true;
                    StartCoroutine(Hook(forceUnhookTime));
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
        
        if (!(canHook && otherPlayerController.canHook && distanceCheck) && forceUnhook)
        {
            forceUnhook = false;
        }
    }

    void OnCollisionEnter2D (Collision2D collision) {
        EvaluateCollision(collision);
        if (collision.gameObject.tag == "Stone")
        {
            HandleGrabStone(collision.gameObject);
        }
    }

    void OnCollisionStay2D (Collision2D collision) {
        EvaluateCollision(collision);
        if (collision.gameObject.tag == "Stone")
        {
            HandleGrabStone(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Stone")
        {
            Debug.LogWarning("Called HandleGrabStone");
            HandleGrabStone(collision.gameObject);
        }
    }


    void EvaluateCollision (Collision2D collision) {
        for (int i = 0; i < collision.contactCount; i++) {
            Vector2 normal = collision.GetContact(i).normal;
            if (normal.y* (_rigidbody2D.gravityScale>0?1:-1) >= minGroundDotProduct) {
                onGround = true;
                contactNormal = normal;
            }
        }
    }

    void HandleGrabStone(GameObject stone)
    {
        if (canHook && !hooked)
        {
            GrabStone(stone);
        }
    }

    void HandleThrowStone()
    {
        if (canHook && !hooked && canThrow)
        {
            if (stoneObj != null)
            {
                stoneObj.GetComponent<Stone>().Thrown();

                stoneObj = null;
                canHook = false;
                canThrow = false;
            }
        }
    }

    void GrabStone(GameObject stone)
    {
        if (stoneObj == null)
        {
            stoneObj = stone;
            stoneObj.GetComponent<Stone>().Grabbed(this);

            canHook = false;
            StartCoroutine(HoldCanThrow());
        }
    }
    
    void Jump () {
        if (onGround)
        {
            velocity.y += (_rigidbody2D.gravityScale > 0 ? 1 : -1) *
                          Mathf.Sqrt(-2f * Physics.gravity.y * Mathf.Abs(_rigidbody2D.gravityScale) * jumpHeight);
        }
    }
    Vector2 ProjectOnContactPlane (Vector2 vector) {
        return vector - contactNormal * Vector2.Dot(vector, contactNormal);
    }
    void UpdateState ()
    {
        velocity = _rigidbody2D.velocity;
        if (!onGround) {
            contactNormal = Vector2.up;
        }
    }
    void AdjustVelocity () {
        if (!hooked)
        {
            Vector2 xAxis = ProjectOnContactPlane(Vector2.right).normalized;

            float currentX = Vector2.Dot(velocity, xAxis);

            float acceleration = maxAcceleration;
            float maxSpeedChange = acceleration * Time.deltaTime;
            var rawVel = Mathf.MoveTowards(velocity.x, desiredHorizontalSpeed, maxSpeedChange);

            float newX =
                Mathf.MoveTowards(currentX, desiredHorizontalSpeed, maxSpeedChange);

            if (onGround)
            {
                velocity += xAxis * (newX - currentX);
            }
            else
            {
                velocity.x = newX;
            }
        }
    }

    private IEnumerator Hook(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        forceUnhook = true;
        hooked = false;
    }

    private IEnumerator HoldCanThrow()
    {
        yield return new WaitForSeconds(1f);
        canThrow = true;
    }
}
