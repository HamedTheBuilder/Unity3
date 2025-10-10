using UnityEngine;

public class CheckPoint : MonoBehaviour
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
        SetCheckpointVisuals(false);
    }

    public void ActivateCheckpoint(PlayerMoveBridge player)
    {
        if (!isActivated)
        {
            isActivated = true;
            player.SetCheckpoint(transform.position);
            SetCheckpointVisuals(true);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound("Checkpoint");
            }

            Debug.Log("Checkpoint activated!");
        }
    }

    void SetCheckpointVisuals(bool activated)
    {
        if (checkpointLight != null)
        {
            checkpointLight.color = activated ? activatedColor : deactivatedColor;
        }

        if (flagRenderer != null && activatedMaterial != null && deactivatedMaterial != null)
        {
            flagRenderer.material = activated ? activatedMaterial : deactivatedMaterial;
        }

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
        Gizmos.color = isActivated ? Color.green : Color.gray;
        Gizmos.DrawWireCube(transform.position, new Vector3(2, 3, 2));
        Gizmos.color = isActivated ? new Color(0, 1, 0, 0.3f) : new Color(0.5f, 0.5f, 0.5f, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(2, 3, 2));
    }
}