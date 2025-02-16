using UnityEngine;

public class GlowController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propertyBlock;

    [ColorUsage(true, true)]
    public Color glowColor = Color.white;
    public float glowAmount = 1f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        spriteRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetColor("_Color", glowColor);         
        propertyBlock.SetFloat("_GlowAmount", glowAmount);   

        spriteRenderer.SetPropertyBlock(propertyBlock);
    }
}