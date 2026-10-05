using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlinkoController : MonoBehaviour
{
    private enum PlinkoState { Inactive, Aiming, Dropping }
    private PlinkoState state = PlinkoState.Inactive;

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform ballTransform;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private PlayerController playerMovementScript;

    [Header("Ball Placement (tune these in the Inspector to match your board)")]
    [Tooltip("World Y position of the drop rail - where the ball sits before you click to drop it.")]
    [SerializeField] private float dropHeightY = 33.3f;
    [Tooltip("Leftmost world X the ball can be placed at.")]
    [SerializeField] private float minDropX = 305f;
    [Tooltip("Rightmost world X the ball can be placed at.")]
    [SerializeField] private float maxDropX = 322f;

    private Rigidbody2D ballRb;

    private void Start()
    {
        if (ballTransform != null)
        {
            ballTransform.TryGetComponent(out ballRb);
        }

        // Make sure Cinemachine starts out following the player
        if (cinemachineCamera != null && playerTransform != null)
        {
            cinemachineCamera.Follow = playerTransform;
        }
    }

    private void Update()
    {
        if (state == PlinkoState.Aiming)
        {
            HandleAimingInput();
        }
    }

    private void HandleAimingInput()
    {
        if (Mouse.current == null || Camera.main == null || ballTransform == null)
        {
            return;
        }

        // Convert the mouse's screen position to world space at the drop rail's depth,
        // then slide the (still-kinematic) ball along X to follow it.
        Vector3 screenPos = Mouse.current.position.ReadValue();
        screenPos.z = Mathf.Abs(Camera.main.transform.position.z - dropHeightY);
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPos);

        float clampedX = Mathf.Clamp(worldPoint.x, minDropX, maxDropX);
        ballTransform.position = new Vector3(clampedX, dropHeightY, ballTransform.position.z);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            DropBall();
        }
    }

    private void DropBall()
    {
        state = PlinkoState.Dropping;

        if (ballRb != null)
        {
            ballRb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public void StartPlinkoDrop()
    {
        state = PlinkoState.Aiming;

        // Retarget Cinemachine to follow the ball instead of the player.
        // Cinemachine's own damping handles the smooth pan - no manual transform code needed.
        if (cinemachineCamera != null && ballTransform != null)
        {
            cinemachineCamera.Follow = ballTransform;
        }

        // Disable player movement controls during the Plinko phase
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        // Keep the ball frozen (kinematic) so it can be positioned before the drop
        if (ballRb != null)
        {
            ballRb.bodyType = RigidbodyType2D.Kinematic;
            ballRb.linearVelocity = Vector2.zero;
            ballTransform.position = new Vector3(Mathf.Clamp(ballTransform.position.x, minDropX, maxDropX), dropHeightY, ballTransform.position.z);
        }
    }

    public void EndPlinkoAndTeleport(Vector3 destination)
    {
        state = PlinkoState.Inactive;

        // Teleport player to destination
        if (playerTransform != null)
        {
            playerTransform.position = destination;
        }

        // Switch Cinemachine and controls back to the player
        if (cinemachineCamera != null && playerTransform != null)
        {
            cinemachineCamera.Follow = playerTransform;
        }
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
    }
}