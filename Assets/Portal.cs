using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField] private Transform teleportDestination;
    private static bool isTransitioning = false;
    [SerializeField] private float cooldownTime = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("SOMETHING TOUCHED THE PORTAL: " + other.gameObject.name);

        if (isTransitioning)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            if (teleportDestination != null)
            {
                isTransitioning = true;
                other.transform.position = teleportDestination.position;
                Invoke(nameof(ResetCooldown), cooldownTime);
            }
        }
    }

    private void ResetCooldown()
    {
        isTransitioning = false;
    }
}
