using UnityEngine;

public class PlinkoBallPortal : MonoBehaviour
{
    [SerializeField] private PlinkoController plinkoController;
    [SerializeField] private Transform portalDestination;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only trigger when THIS ball enters an object tagged "Portal"
        if (other.CompareTag("Portal"))
        {
            // Reset ball physics so it stops moving/colliding in the background
            if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic; // Lock physics until the next drop
            }

            if (plinkoController != null && portalDestination != null)
            {
                plinkoController.EndPlinkoAndTeleport(portalDestination.position);
            }
        }
    }
}