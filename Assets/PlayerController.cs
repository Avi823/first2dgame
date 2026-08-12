using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    private float horizontalInput;
    [SerializeField] private float jumpForce = 5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    private Collider2D playerCollider;
    private bool isGrounded;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing;
    private bool canDash = true;

    public enum BodyState{ Normal, Tall, Flat }

    [Header("Morph Systems")]
    [SerializeField] private BodyState currentState = BodyState.Normal;
    private Vector3 normalScale = new Vector3(1f, 1f, 1f);
    private Vector3 tallScale = new Vector3(0.6f, 1.5f, 1f);
    private Vector3 flatScale = new Vector3(1.5f, 0.6f, 1f);
    private Vector3 targetScale;

    [Header("Sprite Morphing")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite tallSprite;
    [SerializeField] private Sprite flatSprite;

    [Header("Physics Settings")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float rollTorque = 15f;
    [SerializeField] private float maxAngularVelocity = 500f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        targetScale = normalScale;
        ResetToNormal();
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 10f);

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetToNormal();
        }

        if (isDashing)
        {
            return;
        }

        Vector2 bottomCenter = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.min.y);
        isGrounded = Physics2D.OverlapCircle(bottomCenter, groundCheckRadius, groundLayer);

        horizontalInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                horizontalInput = -1f;
            }
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                horizontalInput = 1f;
            }
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            Jump();
        }

        if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        if (currentState == BodyState.Normal)
        {
            if (horizontalInput != 0)
            {
                rb.AddTorque(-horizontalInput * rollTorque);
                rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
            }
            else
            {
                rb.angularVelocity = Mathf.Lerp(rb.angularVelocity, 0f, Time.fixedDeltaTime * 10f);
            }
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    public void MakeFlat()
    {
        SetState(BodyState.Flat);
    }

    public void MakeTall()
    {
        SetState(BodyState.Tall);
    }

    public void ResetToNormal()
    {
        SetState(BodyState.Normal);
    }

    private void SetState(BodyState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case BodyState.Normal: 
                targetScale = normalScale; 
                if (normalSprite != null) spriteRenderer.sprite = normalSprite;
                rb.constraints = RigidbodyConstraints2D.None;
                break;

            case BodyState.Tall: 
                targetScale = tallScale; 
                if (tallSprite != null) spriteRenderer.sprite = tallSprite;
                ResetRotationAndLock();
                break;

            case BodyState.Flat: 
                targetScale = flatScale; 
                if (flatSprite != null) spriteRenderer.sprite = flatSprite;
                ResetRotationAndLock();
                break;
        }
    }

    private void ResetRotationAndLock()
    {
        transform.rotation = Quaternion.identity;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.angularVelocity = 0f;
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = horizontalInput != 0 ? horizontalInput : transform.localScale.x;
        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}