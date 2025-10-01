using UnityEngine;

public class CameraSwitcher11 : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;
    public Canvas[] canvasesForCamera1;  // ��������� ������ ��������� ������
    public Canvas[] canvasesForCamera2;  // ��������� ������ ��������� �������

    private bool isCamera1Active = true;

    void Start()
    {
        // ���� �� �������� ������ ���� �������� �����
        camera1.enabled = true;
        camera2.enabled = false;

        // ����� ������� �������� ������ ������ �������
        SetCanvasesState(canvasesForCamera1, true);
        SetCanvasesState(canvasesForCamera2, false);
    }

    // ��� ������ ������ �����
    public void SwitchCamera()
    {
        isCamera1Active = !isCamera1Active;

        camera1.enabled = isCamera1Active;
        camera2.enabled = !isCamera1Active;

        // ����� ���� ���������
        SetCanvasesState(canvasesForCamera1, isCamera1Active);
        SetCanvasesState(canvasesForCamera2, !isCamera1Active);

        Debug.Log("�������� ������ ����: " + (isCamera1Active ? "�������� 1" : "�������� 2"));
    }

    // ���� ������ ������ ���� ������ �� ���������
    private void SetCanvasesState(Canvas[] canvases, bool state)
    {
        if (canvases != null)
        {
            foreach (Canvas canvas in canvases)
            {
                if (canvas != null)
                {
                    canvas.enabled = state;
                }
            }
        }
    }
}