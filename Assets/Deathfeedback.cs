using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class DeathFeedback : MonoBehaviour
{
    public static DeathFeedback instance;

    [Header("Burst")]
    [SerializeField] private int burstCount = 28;
    [SerializeField] private float burstMinSpeed = 3f;
    [SerializeField] private float burstMaxSpeed = 7f;
    [SerializeField] private float burstMinSize = 0.08f;
    [SerializeField] private float burstMaxSize = 0.2f;
    [SerializeField] private Color burstColor = Color.white;

    [Header("Shake")]
    [SerializeField] private float shakeForce = 0.35f;

    [Header("Timing")]
    [SerializeField] private float hitPause = 0.12f;
    [SerializeField] private float fadeOutTime = 0.15f;
    [SerializeField] private float blackHold = 0.05f;
    [SerializeField] private float fadeInTime = 0.25f;

    private CinemachineImpulseSource impulseSource;
    private Image fadeImage;
    private Material burstMaterial;
    private GameObject player;
    private PlayerController playerController;
    private Rigidbody2D playerRb;
    private SpriteRenderer[] playerSprites;
    private bool isDying;

    private void Awake()
    {
        instance = this;
        impulseSource = GetComponent<CinemachineImpulseSource>();
        BuildFadeOverlay();

        burstMaterial = new Material(Shader.Find("Sprites/Default"));
        burstMaterial.mainTexture = MakeSoftDot();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            playerRb = player.GetComponent<Rigidbody2D>();
            playerSprites = player.GetComponentsInChildren<SpriteRenderer>();
        }
    }

    public void Die()
    {
        if (isDying)
        {
            return;
        }

        if (player == null || playerRb == null)
        {
            if (Checkpoints.instance != null)
            {
                Checkpoints.instance.RespawnPlayer();
            }
            return;
        }

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        isDying = true;
        Vector3 deathPosition = player.transform.position;

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        playerRb.linearVelocity = Vector2.zero;
        playerRb.angularVelocity = 0f;
        playerRb.simulated = false;
        SetPlayerVisible(false);

        SpawnBurst(deathPosition);
        impulseSource.GenerateImpulse(Random.insideUnitCircle.normalized * shakeForce);

        yield return new WaitForSeconds(hitPause);
        yield return Fade(0f, 1f, fadeOutTime);
        yield return new WaitForSeconds(blackHold);

        if (Checkpoints.instance != null)
        {
            Checkpoints.instance.RespawnPlayer();
        }

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        SetPlayerVisible(true);

        yield return Fade(1f, 0f, fadeInTime);

        playerRb.simulated = true;
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        isDying = false;
    }

    private void SetPlayerVisible(bool visible)
    {
        if (playerSprites == null)
        {
            return;
        }

        foreach (SpriteRenderer sr in playerSprites)
        {
            sr.enabled = visible;
        }
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            SetFadeAlpha(Mathf.Lerp(from, to, t / duration));
            yield return null;
        }

        SetFadeAlpha(to);
    }

    private void SetFadeAlpha(float alpha)
    {
        fadeImage.color = new Color(0f, 0f, 0f, alpha);
    }

    private void SpawnBurst(Vector3 position)
    {
        GameObject go = new GameObject("DeathBurst");
        go.transform.position = position;

        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.loop = false;
        main.playOnAwake = false;
        main.duration = 1f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.4f, 0.8f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(burstMinSpeed, burstMaxSpeed);
        main.startSize = new ParticleSystem.MinMaxCurve(burstMinSize, burstMaxSize);
        main.startColor = burstColor;
        main.gravityModifier = 1.2f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)burstCount) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.15f;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient fade = new Gradient();
        fade.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = fade;

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 90;
        renderer.sharedMaterial = burstMaterial;

        ps.Play();
        Destroy(go, 2f);
    }

    private void BuildFadeOverlay()
    {
        GameObject canvasGO = new GameObject("DeathFadeCanvas");
        canvasGO.transform.SetParent(transform, false);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        GameObject imageGO = new GameObject("Fade", typeof(RectTransform));
        imageGO.transform.SetParent(canvasGO.transform, false);

        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        fadeImage.raycastTarget = false;

        RectTransform rt = fadeImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
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