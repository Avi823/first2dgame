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

    [Header("Ball Placement")]
    [SerializeField] private float dropHeightY = 33.3f;
    [SerializeField] private float minDropX = 305f;
    [SerializeField] private float maxDropX = 322f;

    private Rigidbody2D ballRb;
    private Vector3 ballInitialPosition;

    private void Start()
    {
        if (ballTransform != null)
        {
            ballTransform.TryGetComponent(out ballRb);
            ballInitialPosition = ballTransform.position;

            if (ballRb != null)
            {
                ballRb.bodyType = RigidbodyType2D.Kinematic;
                ballRb.linearVelocity = Vector2.zero;
            }
        }

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

        if (cinemachineCamera != null && ballTransform != null)
        {
            cinemachineCamera.Follow = ballTransform;
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        if (ballRb != null)
        {
            ballRb.bodyType = RigidbodyType2D.Kinematic;
            ballRb.linearVelocity = Vector2.zero;
            ballTransform.position = new Vector3(Mathf.Clamp(ballTransform.position.x, minDropX, maxDropX), dropHeightY, ballTransform.position.z);
        }
    }

    public void ResetPlinko()
    {
        state = PlinkoState.Inactive;

        if (cinemachineCamera != null && playerTransform != null)
        {
            cinemachineCamera.Follow = playerTransform;
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        if (ballRb != null)
        {
            ballRb.bodyType = RigidbodyType2D.Kinematic;
            ballRb.linearVelocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }

        if (ballTransform != null)
        {
            ballTransform.position = ballInitialPosition;
        }
    }

    public void EndPlinkoAndTeleport(Vector3 destination)
    {
        state = PlinkoState.Inactive;

        if (playerTransform != null)
        {
            playerTransform.position = destination;
        }

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