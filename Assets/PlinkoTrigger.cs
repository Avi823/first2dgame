using UnityEngine;

public class PlinkoTrigger : MonoBehaviour
{
    [SerializeField] private PlinkoController plinkoController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && plinkoController != null)
        {
            plinkoController.StartPlinkoDrop();
        }
    }
}