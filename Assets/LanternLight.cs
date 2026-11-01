using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LanternLight : MonoBehaviour
{
    [Header("Base Look")]
    [SerializeField] private Color lanternColor = new Color(1f, 0.72f, 0.35f, 1f);
    [SerializeField] private float baseIntensity = 2.2f;
    [SerializeField] private float outerRadius = 4f;
    [SerializeField] private float innerRadius = 0.5f;

    [Header("Activation")]
    [SerializeField] private float activationRadius = 6f;
    [SerializeField] private float fadeInTime = 0.6f;
    [SerializeField] private bool stayLit = true;
    [SerializeField] private bool startLit = false;

    [Header("Flicker Settings")]
    [SerializeField] private bool flicker = true;
    [SerializeField] private float flickerSpeed = 1.8f;
    [SerializeField] private float flickerAmount = 0.35f;

    private Light2D light2D;
    private Transform player;
    private float noiseOffset;
    private float litAmount;
    private bool isLit;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        light2D.lightType = Light2D.LightType.Point;
        light2D.color = lanternColor;
        light2D.pointLightOuterRadius = outerRadius;
        light2D.pointLightInnerRadius = innerRadius;

        isLit = startLit;
        litAmount = startLit ? 1f : 0f;
        light2D.intensity = baseIntensity * litAmount;
        light2D.enabled = litAmount > 0f;

        noiseOffset = Random.Range(0f, 1000f);
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (player != null)
        {
            Vector2 offsetToPlayer = (Vector2)player.position - (Vector2)transform.position;
            bool inRange = offsetToPlayer.sqrMagnitude <= activationRadius * activationRadius;

            if (inRange)
            {
                isLit = true;
            }
            else if (!stayLit)
            {
                isLit = false;
            }
        }

        float target = isLit ? 1f : 0f;
        litAmount = Mathf.MoveTowards(litAmount, target, Time.deltaTime / Mathf.Max(fadeInTime, 0.01f));

        light2D.enabled = litAmount > 0f;
        if (!light2D.enabled)
        {
            return;
        }

        float intensity = baseIntensity;
        if (flicker)
        {
            float noise = Mathf.PerlinNoise(noiseOffset, Time.time * flickerSpeed);
            intensity += (noise - 0.5f) * 2f * flickerAmount;
        }

        light2D.intensity = intensity * litAmount;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.72f, 0.35f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}