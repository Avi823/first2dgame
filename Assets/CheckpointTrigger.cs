using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    private bool hasTriggered = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            Checkpoints.instance.UpdateCheckpoint(transform.position);
            Debug.Log("Checkpoint triggered at: " + transform.position);
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                ColorUtility.TryParseHtmlString("#cba6ff", out Color lavender);
                sr.color = lavender;
            }
        }
    }
}
