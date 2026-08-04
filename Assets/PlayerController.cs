using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    private float horizontalInput;
    [SerializeField] private float jumpForce = 5f;
    private Rigidbody2D rb;
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
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
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetScale = normalScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 10f);
        if (isDashing)
        {
            return;
        }
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
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
    }
    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
    //Morph Logic
    public void TriggerSmoosh(bool squishedVertically)
    {
        switch (currentState)
        {
            case BodyState.Normal:
              if (squishedVertically) SetState(BodyState.Flat);
              else SetState(BodyState.Tall);
              break;
            case BodyState.Tall:
                SetState(BodyState.Flat);
                break;
            case BodyState.Flat:
                SetState(BodyState.Tall);
                break;
        }
    }
    public void ResettoNormal()
    {
        SetState(BodyState.Normal);
    }
    private void SetState(BodyState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case BodyState.Normal: targetScale = normalScale; break;
            case BodyState.Tall: targetScale = tallScale; break;
            case BodyState.Flat: targetScale = flatScale; break;
        }
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
        if (groundCheckPoint != null){
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
    
}
