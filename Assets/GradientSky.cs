using UnityEngine;
 
public class GradientSky : MonoBehaviour
{
    [SerializeField] private Gradient skyGradient;
    [SerializeField] private int sortingOrder = -100;
    [SerializeField] private float sizeMultiplier = 1.3f;
 
    private SpriteRenderer sr;
    private Camera cam;
 
    private void Reset()
    {
        skyGradient = new Gradient();
        skyGradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.03f, 0.04f, 0.09f), 0f),
                new GradientColorKey(new Color(0.07f, 0.08f, 0.18f), 0.55f),
                new GradientColorKey(new Color(0.16f, 0.1f, 0.2f), 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );
    }
 
    private void Awake()
    {
        cam = Camera.main;
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = MakeGradientSprite();
        sr.sortingOrder = sortingOrder;
    }
 
    private void LateUpdate()
    {
        if (cam == null)
        {
            return;
        }
 
        float height = cam.orthographicSize * 2f * sizeMultiplier;
        float width = height * cam.aspect;
        transform.localScale = new Vector3(width, height, 1f);
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, transform.position.z);
    }
 
    private Sprite MakeGradientSprite()
    {
        int res = 128;
        Texture2D tex = new Texture2D(1, res);
        tex.wrapMode = TextureWrapMode.Clamp;
 
        for (int y = 0; y < res; y++)
        {
            float t = y / (float)(res - 1);
            tex.SetPixel(0, y, skyGradient.Evaluate(t));
        }
 
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, res), new Vector2(0.5f, 0.5f), 1f);
    }
}