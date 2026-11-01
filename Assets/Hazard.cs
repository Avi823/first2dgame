using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (DeathFeedback.instance != null)
        {
            DeathFeedback.instance.Die();
        }
        else if (Checkpoints.instance != null)
        {
            Checkpoints.instance.RespawnPlayer();
        }
    }
}