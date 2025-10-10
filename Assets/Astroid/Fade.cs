using UnityEngine;
using System.Collections;

public class BackgroundFader : MonoBehaviour
{
    public static BackgroundFader Instance;

    private Camera fadeCamera;
    private float currentAlpha = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            CreateFadeCamera();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void CreateFadeCamera()
    {
        GameObject cameraObj = new GameObject("BackgroundFadeCamera");
        fadeCamera = cameraObj.AddComponent<Camera>();
        fadeCamera.clearFlags = CameraClearFlags.SolidColor;
        fadeCamera.backgroundColor = new Color(0, 0, 0, 0);
        fadeCamera.cullingMask = 0; // لا ترى أي شيء
        fadeCamera.depth = -100; // تحت كل الكاميرات
        fadeCamera.enabled = true;

        DontDestroyOnLoad(cameraObj);
    }

    public IEnumerator FadeOut(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            currentAlpha = timer / duration;

            // تغيير لون الخلفية من الشفاف إلى الأسود
            fadeCamera.backgroundColor = new Color(0, 0, 0, currentAlpha);
            fadeCamera.clearFlags = CameraClearFlags.SolidColor;

            yield return null;
        }

        // الخلفية سوداء بالكامل
        fadeCamera.backgroundColor = Color.black;
    }
}