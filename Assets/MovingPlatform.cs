using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum PlatformType { Horizontal, Vertical }
    [Header("Movement Settings")]
    [SerializeField] private PlatformType platformType = PlatformType.Horizontal;
    [SerializeField] private Transform PointA;
    [SerializeField] private Transform PointB;
    [SerializeField] private float speed = 2f;


    [Header("Squish Detection")]
    [SerializeField] private float checkRadius = 0.4f;
    [SerializeField] private LayerMask wallAndGroundLayer;
    private Vector3 targetPosition;
    void Start()
    {
        if (PointB != null)
        {
            targetPosition = PointB.position;
        }
    }
    void Update()
    {
        if (PointA == null || PointB == null) return;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetPosition = targetPosition == PointA.position ? PointB.position : PointA.position;
        }

    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player == null) return;
            if (platformType == PlatformType.Horizontal)
            {
                bool wallLeft = Physics2D.OverlapCircle(collision.transform.position + Vector3.left * 0.5f, checkRadius, wallAndGroundLayer);
                bool wallRight = Physics2D.OverlapCircle(collision.transform.position + Vector3.right * 0.5f, checkRadius, wallAndGroundLayer);
                if (wallLeft || wallRight)
                {
                    player.MakeTall();
                }
            } else if (platformType == PlatformType.Vertical)
            {
                bool wallAbove = Physics2D.OverlapCircle(collision.transform.position + Vector3.up * 0.5f, checkRadius, wallAndGroundLayer);
                bool wallBelow = Physics2D.OverlapCircle(collision.transform.position + Vector3.down * 0.5f, checkRadius, wallAndGroundLayer);
                if (wallAbove || wallBelow)
                {
                    player.MakeFlat();
                }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}