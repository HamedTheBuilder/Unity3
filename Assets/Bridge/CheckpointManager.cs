using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Vector3 currentCheckpoint;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Set initial checkpoint to player start position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            currentCheckpoint = player.transform.position;
        }
    }

    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        currentCheckpoint = checkpointPosition;
        Debug.Log("Checkpoint saved at: " + checkpointPosition);
    }

    public void RespawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = currentCheckpoint;

            // «·»ÕÀ ⁄‰ „ﬂÊ‰ «· Õﬂ„ »«··«⁄» »√Ì «”„ ﬂ«‰
            MonoBehaviour playerController = player.GetComponent<MonoBehaviour>();
            if (playerController != null)
            {
                // «” œ⁄«¡ œ«·… «·≈ÕÌ«¡ ≈–« ﬂ«‰  „ÊÃÊœ…
                playerController.Invoke("Respawn", 0f);
            }

            Debug.Log("Player respawned at checkpoint");
        }
    }
}