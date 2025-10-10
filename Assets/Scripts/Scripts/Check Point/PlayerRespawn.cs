using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    public static Vector3 lastCheckpointPos;
    private static bool hasCheckpoint = false;

    void Start()
    {
        // عند بداية المشهد، إذا فيه تشيك بوينت محفوظ نرجع له
        if (hasCheckpoint)
        {
            transform.position = lastCheckpointPos;
            Debug.Log("🚀 رجع اللاعب للتشيك بوينت: " + lastCheckpointPos);
        }
    }

    public void SetCheckpoint(Vector3 newPos)
    {
        lastCheckpointPos = newPos;
        hasCheckpoint = true;
        Debug.Log("✅ حفظ التشيك بوينت في: " + newPos);
    }

    public void Die()
    {
        Debug.Log("💀 اللاعب مات - إعادة تحميل السين");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // لإعادة ضبط عند الرجوع للقائمة أو بداية جديدة
    public static void ResetCheckpoint()
    {
        hasCheckpoint = false;
        lastCheckpointPos = Vector3.zero;
    }
}
