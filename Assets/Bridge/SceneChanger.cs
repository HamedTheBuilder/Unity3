using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Header("Scene Settings")]
    public string targetSceneName;
    public int targetSceneIndex = -1;

    [Header("Transition Settings")]
    public float transitionDelay = 0.5f;
    public bool useTrigger = true;
    public bool useCollision = false;

    [Header("Visual Effects")]
    public GameObject transitionEffect;
    public bool showDebugMessages = true;

    void Start()
    {
        //  Õ–Ì— ≈–« ·„ Ì „  ⁄ÌÌ‰ «·”Ì‰
        if (string.IsNullOrEmpty(targetSceneName) && targetSceneIndex == -1)
        {
            Debug.LogWarning("SceneChanger: No target scene specified!", gameObject);
        }
    }

    // «·«‰ ﬁ«· ≈·Ï «·”Ì‰ «·„Õœœ
    public void ChangeScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            LoadSceneByName();
        }
        else if (targetSceneIndex >= 0)
        {
            LoadSceneByIndex();
        }
        else
        {
            Debug.LogError("SceneChanger: No valid target scene specified!");
        }
    }

    void LoadSceneByName()
    {
        if (showDebugMessages)
            Debug.Log($"Changing to scene: {targetSceneName}");

        //  ‘€Ì·  √ÀÌ— «·«‰ ﬁ«· ≈–« „ÊÃÊœ
        if (transitionEffect != null)
        {
            Instantiate(transitionEffect, transform.position, Quaternion.identity);
        }

        //  Õ„Ì· «·”Ì‰ »⁄œ «· √ŒÌ—
        Invoke("LoadSceneByNameDelayed", transitionDelay);
    }

    void LoadSceneByNameDelayed()
    {
        SceneManager.LoadScene(targetSceneName);
    }

    void LoadSceneByIndex()
    {
        if (showDebugMessages)
            Debug.Log($"Changing to scene index: {targetSceneIndex}");

        //  ‘€Ì·  √ÀÌ— «·«‰ ﬁ«· ≈–« „ÊÃÊœ
        if (transitionEffect != null)
        {
            Instantiate(transitionEffect, transform.position, Quaternion.identity);
        }

        //  Õ„Ì· «·”Ì‰ »⁄œ «· √ŒÌ—
        Invoke("LoadSceneByIndexDelayed", transitionDelay);
    }

    void LoadSceneByIndexDelayed()
    {
        SceneManager.LoadScene(targetSceneIndex);
    }

    // «· ’«œ„ „⁄ «· —Ìﬁ—
    void OnTriggerEnter(Collider other)
    {
        if (useTrigger && other.CompareTag("Player"))
        {
            if (showDebugMessages)
                Debug.Log("Player entered trigger - changing scene");

            ChangeScene();
        }
    }

    // «· ’«œ„ «·⁄«œÌ
    void OnCollisionEnter(Collision collision)
    {
        if (useCollision && collision.gameObject.CompareTag("Player"))
        {
            if (showDebugMessages)
                Debug.Log("Player collided - changing scene");

            ChangeScene();
        }
    }

    // œ«·… Ì„ﬂ‰ «” œ⁄«ƒÂ« „‰ √“—«— √Ê √Õœ«À √Œ—Ï
    public void ChangeToScene(string sceneName)
    {
        targetSceneName = sceneName;
        ChangeScene();
    }

    public void ChangeToScene(int sceneIndex)
    {
        targetSceneIndex = sceneIndex;
        ChangeScene();
    }
}