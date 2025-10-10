using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 2f;
    public float distanceFromCenter = 10f;
    public Vector3 centerPoint = Vector3.zero; // äŞØÉ ÇáãÑßÒ ÇáÊí ÊÏæÑ ÍæáåÇ ÇáßÇãíÑÇ

    [Header("Rotation Limits")]
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;

    [Header("Auto Center Settings")]
    public bool autoSetCenterOnStart = true; // ÊÍÏíÏ ÇáãÑßÒ ÊáŞÇÆíÇğ ÚäÏ ÇáÈÏÁ
    public float autoCenterDistance = 10f; // ÇáãÓÇİÉ ÇáÊáŞÇÆíÉ ãä ÇáãÑßÒ

    private float currentXRotation = 0f;
    private float currentYRotation = 0f;
    private Vector3 initialPosition;

    void Start()
    {
        // ÍİÙ ÇáãæÖÚ ÇáÃæáí
        initialPosition = transform.position;

        // ÇÓÊÎÏÇã ÇáŞíã ÇáÍÇáíÉ ááßÇãíÑÇ ßÈÏÇíÉ
        Vector3 currentEuler = transform.eulerAngles;
        currentXRotation = currentEuler.y;
        currentYRotation = currentEuler.x;

        // ÊÍÏíÏ ÇáãÑßÒ ÊáŞÇÆíÇğ ÅĞÇ ßÇä ãİÚáÇğ
        if (autoSetCenterOnStart)
        {
            AutoSetCenterPoint();
        }
        else if (centerPoint == Vector3.zero)
        {
            // ÅĞÇ áã íÊã ÊÚííä ãÑßÒ æáã íßä ÇáÊáŞÇÆí ãİÚáÇğ¡ ÇÓÊÎÏã ãæÖÚ ÃãÇã ÇáßÇãíÑÇ
            centerPoint = transform.position + transform.forward * autoCenterDistance;
        }

        // ÊÍÏíË ãæÖÚ ÇáßÇãíÑÇ ÈäÇÁ Úáì ÇáÅÚÏÇÏÇÊ
        UpdateCameraPosition();
    }

    void Update()
    {
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        // ÇáÊÍßã ÈÇáÏæÑÇä İŞØ ÚäÏ ÇáÖÛØ Úáì ÒÑ ÇáãÇæÓ ÇáÃíãä
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            currentXRotation += mouseX;
            currentYRotation -= mouseY;

            // ÊÍÏíÏ ÇáÍÏæÏ ááÏæÑÇä ÇáÚãæÏí
            currentYRotation = Mathf.Clamp(currentYRotation, minVerticalAngle, maxVerticalAngle);
        }

        // ÊßÈíÑ/ÊÕÛíÑ ÈÚÌáÉ ÇáãÇæÓ
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            distanceFromCenter -= scroll * 5f;
            distanceFromCenter = Mathf.Clamp(distanceFromCenter, 2f, 50f);
        }

        // ÊÍÏíË ãæÖÚ ÇáßÇãíÑÇ
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        // ÍÓÇÈ ÇáÏæÑÇä
        Quaternion rotation = Quaternion.Euler(currentYRotation, currentXRotation, 0f);

        // ÍÓÇÈ ÇáãæÖÚ ÇáÌÏíÏ ÈäÇÁ Úáì ÇáãÓÇİÉ ãä ÇáãÑßÒ
        Vector3 direction = rotation * Vector3.forward;
        Vector3 newPosition = centerPoint - direction * distanceFromCenter;

        // ÊØÈíŞ ÇáÊÍæíáÇÊ
        transform.position = newPosition;
        transform.LookAt(centerPoint);
    }

    // ÏÇáÉ áÊÍÏíÏ äŞØÉ ÇáãÑßÒ ÊáŞÇÆíÇğ ÈäÇÁ Úáì ãæÖÚ ÇáßÇãíÑÇ ÇáÍÇáí
    public void AutoSetCenterPoint()
    {
        // ÍÓÇÈ ÇáãÑßÒ ÈäÇÁ Úáì ãæÖÚ ÇáßÇãíÑÇ ÇáÍÇáí æÇÊÌÇååÇ
        centerPoint = transform.position + transform.forward * distanceFromCenter;

        // ÊÍÏíË ÇáÏæÑÇä ÇáÍÇáí áíÊäÇÓÈ ãÚ ÇáãÑßÒ ÇáÌÏíÏ
        Vector3 lookDirection = (centerPoint - transform.position).normalized;
        currentXRotation = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
        currentYRotation = Mathf.Asin(lookDirection.y) * Mathf.Rad2Deg;

        Debug.Log($"Auto-set center point to: {centerPoint}");
    }

    // ÏÇáÉ áÊÍÏíÏ äŞØÉ ÇáãÑßÒ ÈäÇÁ Úáì ãÓÇİÉ ãÍÏÏÉ
    public void SetCenterPointWithDistance(float newDistance)
    {
        distanceFromCenter = Mathf.Clamp(newDistance, 2f, 50f);
        centerPoint = transform.position + transform.forward * distanceFromCenter;
        UpdateCameraPosition();
    }

    // ÏÇáÉ áÊÍÏíÏ äŞØÉ ÇáãÑßÒ (áÊÑßíÒ ÇáßÇãíÑÇ Úáì ÇáÓİíäÉ)
    public void SetCenterPoint(Vector3 newCenter)
    {
        centerPoint = newCenter;
        UpdateCameraPosition();
    }

    // ÏÇáÉ áÊÍÏíÏ äŞØÉ ÇáãÑßÒ ãÚ ÇáÍİÇÙ Úáì ÇáãÓÇİÉ ÇáÍÇáíÉ
    public void SetCenterPointKeepDistance(Vector3 newCenter)
    {
        // ÍÓÇÈ ÇáãÓÇİÉ ÇáÍÇáíÉ ãä ÇáãÑßÒ ÇáŞÏíã
        float currentDistance = Vector3.Distance(transform.position, centerPoint);
        centerPoint = newCenter;
        distanceFromCenter = currentDistance;
        UpdateCameraPosition();
    }

    // ÏÇáÉ áÅÚÇÏÉ ÊÚííä ÇáßÇãíÑÇ ááÅÚÏÇÏÇÊ ÇáÃæáíÉ
    public void ResetCamera()
    {
        currentXRotation = 0f;
        currentYRotation = 0f;
        distanceFromCenter = 10f;

        // ÅÚÇÏÉ ÊÚííä ÇáãÑßÒ ÅĞÇ ßÇä ÇáÊáŞÇÆí ãİÚáÇğ
        if (autoSetCenterOnStart)
        {
            AutoSetCenterPoint();
        }

        UpdateCameraPosition();
    }

    // ÏÇáÉ áÊÍÏíÏ ÍÓÇÓíÉ ÇáãÇæÓ
    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = Mathf.Clamp(sensitivity, 0.1f, 10f);
    }

    // ÏÇáÉ ááÊÈÏíá Èíä æÖÚ ÇáÊÍÏíÏ ÇáÊáŞÇÆí
    public void ToggleAutoCenter(bool enable)
    {
        autoSetCenterOnStart = enable;
        if (enable)
        {
            AutoSetCenterPoint();
        }
    }

    // İí ÇáãÍÑÑ¡ íãßääÇ ÅÖÇİÉ ÒÑ ááãÓÇÚÏÉ
    [ContextMenu("Auto Set Center Point")]
    void ContextAutoSetCenter()
    {
        AutoSetCenterPoint();
    }

    [ContextMenu("Reset Camera to Initial")]
    void ContextResetCamera()
    {
        ResetCamera();
    }
}