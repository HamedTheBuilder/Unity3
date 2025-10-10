using UnityEngine;

public class PlayerShadow3D : MonoBehaviour
{
    [Header("Shadow Settings")]
    public GameObject shadowPrefab;
    public LayerMask groundLayer = 1;
    public float maxShadowDistance = 50f;
    public float shadowOffset = 0.05f;

    [Header("Shadow Visual")]
    public Color shadowColor = new Color(0, 0, 0, 0.5f);
    public Vector3 shadowScale = new Vector3(2, 2, 2);

    [Header("Shadow Follow")]
    public bool followPlayer = true;
    public float followSmoothness = 5f;

    private GameObject shadowObject;
    private Renderer shadowRenderer;
    private bool isShadowEnabled = true;

    void Start()
    {
        CreateShadow();
    }

    void Update()
    {
        if (isShadowEnabled && shadowObject != null)
        {
            UpdateShadowPosition();
        }
    }

    void CreateShadow()
    {
        if (shadowPrefab == null)
        {
            // ≈‰‘«¡ Ÿ· «› —«÷Ì ≈–« ·„ Ì „  ⁄ÌÌ‰ »—Ì›«»
            shadowObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            shadowObject.name = "PlayerShadow";

            // ≈“«œ… «·‹ Collider ·√‰Â €Ì— needed
            Destroy(shadowObject.GetComponent<Collider>());

            // ≈⁄œ«œ «·„«œ… (Material)
            shadowRenderer = shadowObject.GetComponent<Renderer>();
            Material shadowMaterial = new Material(Shader.Find("Standard"));
            shadowMaterial.color = shadowColor;
            shadowMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            shadowMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            shadowMaterial.SetInt("_ZWrite", 0);
            shadowMaterial.DisableKeyword("_ALPHATEST_ON");
            shadowMaterial.EnableKeyword("_ALPHABLEND_ON");
            shadowMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            shadowMaterial.renderQueue = 3000;
            shadowRenderer.material = shadowMaterial;
        }
        else
        {
            // «” Œœ«„ «·»—Ì›«» «·„Œ’’
            shadowObject = Instantiate(shadowPrefab);
            shadowObject.name = "PlayerShadow";
            shadowRenderer = shadowObject.GetComponent<Renderer>();
        }

        shadowObject.transform.localScale = shadowScale;
        shadowObject.transform.rotation = Quaternion.Euler(90, 0, 0); //  ÊÃÌÂ ··√⁄·Ï
    }

    void UpdateShadowPosition()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position;

        // ≈ÿ·«ﬁ ‘⁄«⁄ ≈·Ï «·√”›· ·«ﬂ ‘«› «·√—÷
        if (Physics.Raycast(rayStart, Vector3.down, out hit, maxShadowDistance, groundLayer))
        {
            // Ê÷⁄ «·Ÿ· ›Êﬁ «·√—÷ »ﬁ·Ì·
            Vector3 targetPosition = hit.point + Vector3.up * shadowOffset;

            if (followPlayer)
            {
                //  Õ—Ìﬂ ”·” ··Ÿ·
                shadowObject.transform.position = Vector3.Lerp(
                    shadowObject.transform.position,
                    targetPosition,
                    Time.deltaTime * followSmoothness
                );
            }
            else
            {
                shadowObject.transform.position = targetPosition;
            }

            //  ⁄œÌ· œÊ—«‰ «·Ÿ· ·Ì ÿ«»ﬁ „⁄ ”ÿÕ «·√—÷
            shadowObject.transform.rotation = Quaternion.LookRotation(-hit.normal);

            //  ⁄œÌ· «·ÕÃ„ Õ”» «·«— ›«⁄ («Œ Ì«—Ì)
            float heightFactor = Mathf.Clamp01(1 - (hit.distance / maxShadowDistance));
            shadowObject.transform.localScale = shadowScale * (0.5f + heightFactor * 0.5f);

            SetShadowVisibility(true);
        }
        else
        {
            // ≈Œ›«¡ «·Ÿ· ≈–« ·„  ﬂ‰ Â‰«ﬂ √—÷
            SetShadowVisibility(false);
        }
    }

    void SetShadowVisibility(bool visible)
    {
        if (shadowRenderer != null)
        {
            shadowRenderer.enabled = visible;
        }
    }

    // œÊ«· ·· Õﬂ„ »«·Ÿ· „‰ ”ﬂ—» «  √Œ—Ï
    public void EnableShadow()
    {
        isShadowEnabled = true;
        SetShadowVisibility(true);
    }

    public void DisableShadow()
    {
        isShadowEnabled = false;
        SetShadowVisibility(false);
    }

    public void SetShadowColor(Color newColor)
    {
        shadowColor = newColor;
        if (shadowRenderer != null)
        {
            shadowRenderer.material.color = shadowColor;
        }
    }

    public void SetShadowScale(Vector3 newScale)
    {
        shadowScale = newScale;
        if (shadowObject != null)
        {
            shadowObject.transform.localScale = shadowScale;
        }
    }

    // —”„ «·‹ Gizmos ··„”«⁄œ… ›Ì «· ’ÕÌÕ
    void OnDrawGizmosSelected()
    {
        // —”„ ‘⁄«⁄ «·Ÿ· ›Ì «·„Õ——
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.down * maxShadowDistance);

        if (shadowObject != null && shadowRenderer.enabled)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(shadowObject.transform.position, 0.5f);
        }
    }
}