using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Camera References")]
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;

    [Header("UI References")]
    public RawImage thirdPersonRawImage; // ����� ���� ������ ��������� �������

    private bool isFirstPersonActive = true;

    void Start()
    {
        // ������ �� �� �������� ������ ���� �������� ����� �� �������
        SetCamerasAndUI(true);
    }

    void Update()
    {
        // ������ �� ��� �� Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchCameras();
        }
    }

    void SwitchCameras()
    {
        // ����� ���� ��������
        isFirstPersonActive = !isFirstPersonActive;

        // ����� ��������� ����� ��� ������
        SetCamerasAndUI(isFirstPersonActive);
    }

    void SetCamerasAndUI(bool firstPersonActive)
    {
        if (firstPersonActive)
        {
            // ����� �������� ������ ������ ����� ����
            firstPersonCamera.enabled = true;
            thirdPersonCamera.enabled = false;

            if (thirdPersonRawImage != null)
                thirdPersonRawImage.enabled = false;
        }
        else
        {
            // ����� �������� ������� ������ ����� ����
            firstPersonCamera.enabled = false;
            thirdPersonCamera.enabled = true;

            if (thirdPersonRawImage != null)
                thirdPersonRawImage.enabled = true;
        }
    }

    // ���� ������ ������ �������� ������ ������
    public bool IsFirstPersonActive()
    {
        return isFirstPersonActive;
    }
}