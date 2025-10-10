using UnityEngine;

public class CheckPoint11 : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    public bool isActivated = false;
    public ParticleSystem activationEffect;
    public Light checkpointLight;
    public Color activatedColor = Color.green;
    public Color deactivatedColor = Color.gray;

    [Header("Visual Feedback")]
    public MeshRenderer flagRenderer;
    public Material activatedMaterial;
    public Material deactivatedMaterial;

    void Start()
    {
        //  ⁄ÌÌ‰ «·„ŸÂ— «·«› —«÷Ì
        SetCheckpointVisuals(false);
    }

    public void ActivateCheckpoint(PlayerMoveBridge player)
    {
        if (!isActivated)
        {
            isActivated = true;

            // Õ›Ÿ „Êﬁ⁄ «· ‘Ìﬂ »ÊÌ‰  (»œÊ‰ œÊ—«‰)
            player.SetCheckpoint(transform.position);

            //  ›⁄Ì· «·„ƒÀ—«  «·»’—Ì…
            SetCheckpointVisuals(true);

            //  ‘€Ì· ’Ê  «· ‰‘Ìÿ
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound("Checkpoint");
            }

            Debug.Log("Checkpoint activated!");
        }
    }

    void SetCheckpointVisuals(bool activated)
    {
        //  €ÌÌ— ·Ê‰ «·÷Ê¡
        if (checkpointLight != null)
        {
            checkpointLight.color = activated ? activatedColor : deactivatedColor;
        }

        //  €ÌÌ— „«œ… «·⁄·„
        if (flagRenderer != null && activatedMaterial != null && deactivatedMaterial != null)
        {
            flagRenderer.material = activated ? activatedMaterial : deactivatedMaterial;
        }

        //  ‘€Ì·/≈Ìﬁ«› «·„ƒÀ—« 
        if (activationEffect != null)
        {
            if (activated)
                activationEffect.Play();
            else
                activationEffect.Stop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMoveBridge player = other.GetComponent<PlayerMoveBridge>();
            if (player != null)
            {
                ActivateCheckpoint(player);
            }
        }
    }

    void OnDrawGizmos()
    {
        // —”„ √ÌﬁÊ‰… «· ‘Ìﬂ »ÊÌ‰  ›Ì «·„Õ——
        Gizmos.color = isActivated ? Color.green : Color.gray;
        Gizmos.DrawWireCube(transform.position, new Vector3(2, 3, 2));
        Gizmos.color = isActivated ? new Color(0, 1, 0, 0.3f) : new Color(0.5f, 0.5f, 0.5f, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(2, 3, 2));
    }
}