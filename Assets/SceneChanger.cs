using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerrr : MonoBehaviour
{
    public string sceneName; // ÇÓã ÇáÓíä ÇáĞí ÊÑíÏ ÇáĞåÇÈ Åáíå

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ÊÍãíá ÇáÓíä ÇáÌÏíÏ
            SceneManager.LoadScene(sceneName);
        }
    }
}