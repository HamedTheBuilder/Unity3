using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Camera References")]
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;

    [Header("UI References")]
    public RawImage thirdPersonRawImage; // «·—«Ê ≈Ì„Ã «·Œ«’… »«·ﬂ«„Ì—« «·À«‰Ì…

    private bool isFirstPersonActive = true;

    void Start()
    {
        // «· √ﬂœ „‰ √‰ «·ﬂ«„Ì—« «·√Ê·Ï ‰‘ÿ… Ê«·À«‰Ì… „⁄ÿ·… ›Ì «·»œ«Ì…
        SetCamerasAndUI(true);
    }

    void Update()
    {
        // «· Õﬁﬁ „‰ ÷€ÿ “— Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchCameras();
        }
    }

    void SwitchCameras()
    {
        //  »œÌ· Õ«·… «·ﬂ«„Ì—«
        isFirstPersonActive = !isFirstPersonActive;

        //  ÿ»Ìﬁ «·≈⁄œ«œ«  »‰«¡ ⁄·Ï «·Õ«·…
        SetCamerasAndUI(isFirstPersonActive);
    }

    void SetCamerasAndUI(bool firstPersonActive)
    {
        if (firstPersonActive)
        {
            //  ›⁄Ì· «·ﬂ«„Ì—« «·√Ê·Ï Ê≈Œ›«¡ «·—«Ê ≈Ì„Ã
            firstPersonCamera.enabled = true;
            thirdPersonCamera.enabled = false;

            if (thirdPersonRawImage != null)
                thirdPersonRawImage.enabled = false;
        }
        else
        {
            //  ›⁄Ì· «·ﬂ«„Ì—« «·À«‰Ì… Ê≈ŸÂ«— «·—«Ê ≈Ì„Ã
            firstPersonCamera.enabled = false;
            thirdPersonCamera.enabled = true;

            if (thirdPersonRawImage != null)
                thirdPersonRawImage.enabled = true;
        }
    }

    // œ«·… „”«⁄œ… ·„⁄—›… «·ﬂ«„Ì—« «·‰‘ÿ… Õ«·Ì«
    public bool IsFirstPersonActive()
    {
        return isFirstPersonActive;
    }
}