using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public static Checkpoints instance;
    private Vector2 currentCheckpoint;
    private GameObject player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            currentCheckpoint = player.transform.position;
        }
    }

    public void UpdateCheckpoint(Vector2 newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
        Debug.Log("Checkpoint updated to: " + currentCheckpoint);
    }

    public void RespawnPlayer()
    {
        if (player != null)
        {
            player.transform.position = currentCheckpoint;
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero; // Reset velocity to prevent unwanted movement
            }
        }
    }
}