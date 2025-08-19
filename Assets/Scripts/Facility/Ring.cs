using UnityEngine;

public class Ring : MonoBehaviour
{
    [SerializeField] Color baseColor = Color.white;
    [SerializeField] float lightOffMultiplier = 0.2f;
    [Range(0, 1)] public float Alpha = 1f;

    static private readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    static private readonly int AlphaID = Shader.PropertyToID("_Alpha");

    private Animator animator;
    private Renderer r;
    private MaterialPropertyBlock mpb;

    private bool isLightingOn = true;

    private Color CurrentColor
    {
        get
        {
            if (isLightingOn)
            {
                return baseColor;
            }
            else
            {
                return new Color(baseColor.r * lightOffMultiplier, baseColor.g * lightOffMultiplier, baseColor.b * lightOffMultiplier, 1f);
            }
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        r = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
        Apply();
    }

    private void LateUpdate()
    {
        Apply();
    }

    public void SetBaseColor(Color color)
    {
        baseColor = color;
        Apply();
    }

    private void Apply()
    {
        r.GetPropertyBlock(mpb);
        mpb.SetColor(BaseColorID, CurrentColor);
        mpb.SetFloat(AlphaID, Alpha);
        r.SetPropertyBlock(mpb);
    }

    public void TurnOnBlink()
    {
        if (animator != null)
        {
            animator.SetTrigger("TurnOnBlink");
        }
    }

    public void TurnOffBlink()
    {
        if (animator != null)
        {
            animator.SetTrigger("TurnOffBlink");
        }
    }

    public void LightOn()
    {
        isLightingOn = true;
        Apply();   
    }

    public void LightOff()
    {
        isLightingOn = false;
        Apply();
        TurnOffBlink();
    }
}
