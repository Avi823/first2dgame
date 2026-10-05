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
    [Header("Flicker Settings")]
    [SerializeField] private bool flicker = true;
    [SerializeField] private float flickerSpeed = 1.8f;
    [SerializeField] private float flickerAmount = 0.35f;
    private Light2D light2D;
    private float noiseOffset;
    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        light2D.lightType = Light2D.LightType.Point;
        light2D.color = lanternColor;
        light2D.intensity = baseIntensity;
        light2D.pointLightOuterRadius = outerRadius;
        light2D.pointLightInnerRadius = innerRadius;
        noiseOffset = Random.Range(0f, 1000f);
    }
    private void Update()
    {
        if (!flicker) return;
        float noise = Mathf.PerlinNoise(noiseOffset, Time.time * flickerSpeed);
        float offset = (noise - 0.5f) * 2f * flickerAmount;
        light2D.intensity = baseIntensity + offset;
    }
}
