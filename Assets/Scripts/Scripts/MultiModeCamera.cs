using UnityEngine;

public class MultiModeCamera : MonoBehaviour
{
    public enum CameraMode
    {
        FirstPerson,
        ThirdPerson,
        TopDown,
        Free
    }

    [Header("Camera Mode")]
    public CameraMode currentMode = CameraMode.ThirdPerson;
    public KeyCode switchModeKey = KeyCode.C;

    [Header("Target")]
    public Transform playerTarget;

    [Header("First Person Settings")]
    public float fpMouseSensitivity = 2f;

    [Header("Third Person Settings")]
    public float tpDistance = 5f;
    public float tpHeight = 2f;

    [Header("Top Down Settings")]
    public float tdHeight = 10f;

    private float mouseX, mouseY;
    private Vector3 fpOffset = new Vector3(0, 0.5f, 0);

    void Start()
    {
        if (playerTarget == null)
        {
            playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //  »œÌ· Ê÷⁄ «·ﬂ«„Ì—«
        if (Input.GetKeyDown(switchModeKey))
        {
            SwitchCameraMode();
        }
    }

    void LateUpdate()
    {
        if (playerTarget == null) return;

        switch (currentMode)
        {
            case CameraMode.FirstPerson:
                UpdateFirstPerson();
                break;
            case CameraMode.ThirdPerson:
                UpdateThirdPerson();
                break;
            case CameraMode.TopDown:
                UpdateTopDown();
                break;
            case CameraMode.Free:
                UpdateFreeCamera();
                break;
        }
    }

    void UpdateFirstPerson()
    {
        //  ÕœÌÀ „œŒ·«  «·„«Ê”
        mouseX += Input.GetAxis("Mouse X") * fpMouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * fpMouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, -90f, 90f);

        //  ÿ»Ìﬁ «·„Ê÷⁄ Ê«·œÊ—«‰
        transform.position = playerTarget.position + fpOffset;
        transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
    }

    void UpdateThirdPerson()
    {
        mouseX += Input.GetAxis("Mouse X") * fpMouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * fpMouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, -30f, 70f);

        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        Vector3 position = playerTarget.position - (rotation * Vector3.forward * tpDistance) + Vector3.up * tpHeight;

        transform.position = position;
        transform.LookAt(playerTarget.position + Vector3.up * 1f);
    }

    void UpdateTopDown()
    {
        Vector3 position = playerTarget.position + Vector3.up * tdHeight;
        transform.position = position;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    void UpdateFreeCamera()
    {
        // ﬂ«„Ì—« Õ—… Ì„ﬂ‰  Õ—ÌﬂÂ«
        float moveSpeed = 5f;
        float horizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        transform.Translate(horizontal, 0, vertical);

        mouseX += Input.GetAxis("Mouse X") * fpMouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * fpMouseSensitivity;
        transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
    }

    void SwitchCameraMode()
    {
        currentMode = (CameraMode)(((int)currentMode + 1) % System.Enum.GetValues(typeof(CameraMode)).Length);
        Debug.Log("Camera mode switched to: " + currentMode);
    }

    // œ«·… · ⁄ÌÌ‰ Ê÷⁄ «·ﬂ«„Ì—«
    public void SetCameraMode(CameraMode newMode)
    {
        currentMode = newMode;
    }
}