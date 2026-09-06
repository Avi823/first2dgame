using UnityEngine;

public class GlitchedPortals : MonoBehaviour
{
   [Header("Color Glitch Settings")]
   [SerializeField] private float colorChangeInterval = 0.05f;
   [SerializeField] private bool useRandomHue = true;
   [Header("Jitter Settings")]
   [SerializeField] private float jitterIntensity = 0.08f;
   [SerializeField] private float jitterSpeed = 30f;
   private SpriteRenderer spriteRenderer;
   private Vector3 originalPosition;
   private float colorTimer;
   private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.localPosition;
    }
   private void Update()
    {
        HandleColorGlitch();
        HandleJitter();
    }
    private void HandleColorGlitch()
    {
        colorTimer += Time.deltaTime;
        if (colorTimer >= colorChangeInterval)
        {
            colorTimer = 0f;
            if (useRandomHue && spriteRenderer != null)
            {
                // Generates a bright random RGB color
                spriteRenderer.color = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);
            }
        }
    }
    private void HandleJitter()
    {
        // Uses Perlin noise for rapid, erratic vibration relative to the starting point
        float offsetX = (Mathf.PerlinNoise(Time.time * jitterSpeed, 0f) - 0.5f) * 2f * jitterIntensity;
        float offsetY = (Mathf.PerlinNoise(0f, Time.time * jitterSpeed) - 0.5f) * 2f * jitterIntensity;

        transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);
    }
}
