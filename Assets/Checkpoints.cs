using UnityEngine;
using UnityEngine.InputSystem;

public class Checkpoints : MonoBehaviour
{
    public static Checkpoints instance;
    private Vector2 currentCheckpoint;
    private GameObject player;

    [SerializeField] private PlinkoController plinkoController;

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

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            RespawnPlayer();
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
                playerRb.linearVelocity = Vector2.zero;
                playerRb.angularVelocity = 0f;
            }
        }

        if (plinkoController != null)
        {
            plinkoController.ResetPlinko();
        }
    }
}