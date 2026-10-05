using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Snow : MonoBehaviour
{
    [SerializeField] private float width = 30f;
    [SerializeField] private float height = 20f;
    [SerializeField] private float speed = 1.2f;
    [SerializeField] private float angle = 190f;
    [SerializeField] private float minSize = 0.04f;
    [SerializeField] private float maxSize = 0.09f;
    [SerializeField] private float emissionRate = 45f;
    [SerializeField] private Color color = new Color(1f, 0.93f, 0.78f, 0.6f);
    [SerializeField] private LayerMask collidesWith = ~0;

    private void Awake()
    {
        var ps = GetComponent<ParticleSystem>();

        float rad = angle * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        float lifetime = (Mathf.Max(width, height) * 1.5f) / speed;

        var main = ps.main;
        main.loop = true;
        main.prewarm = true;
        main.duration = lifetime * 1.2f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(lifetime * 0.8f, lifetime * 1.2f);
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(minSize, maxSize);
        main.startColor = color;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.maxParticles = 500;

        var emission = ps.emission;
        emission.rateOverTime = emissionRate;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(width, height, 1f);

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = dir.x * speed;
        velocity.y = dir.y * speed;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient fade = new Gradient();
        fade.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(1f, 0.2f),
                new GradientAlphaKey(1f, 0.8f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = fade;

        var collision = ps.collision;
        collision.enabled = true;
        collision.type = ParticleSystemCollisionType.World;
        collision.mode = ParticleSystemCollisionMode.Collision2D;
        collision.collidesWith = collidesWith;
        collision.lifetimeLoss = 1f;
        collision.enableDynamicColliders = true;

        var renderer = GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 100;
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.material.mainTexture = MakeSoftDot();
    }

    private Texture2D MakeSoftDot()
    {
        int res = 32;
        Texture2D tex = new Texture2D(res, res);
        Vector2 center = new Vector2(res / 2f, res / 2f);

        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center) / (res / 2f);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(1f - dist)));
            }
        }

        tex.Apply();
        return tex;
    }
}