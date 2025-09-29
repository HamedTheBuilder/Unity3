using UnityEngine;
using UnityEngine.UI;

public class CanvasHalfCircle : MonoBehaviour
{
    public Transform spaceship; // �������
    public RawImage rawImage; // ������ ���� ������� ��� ��� ��� �����
    public float radius = 5f; // ��� ��� ��� ������� (������� ��� ������ ��������)
    public float angleStart = -90f; // ������� ���� ���� ���� ��� �������
    public float angleEnd = 90f; // ������� ���� ����� ����� ��� �������

    private RectTransform rawImageRectTransform; // ������� ��� RectTransform ����� �� RawImage

    void Start()
    {
        // ������ ��� RectTransform ����� �� RawImage
        rawImageRectTransform = rawImage.GetComponent<RectTransform>();

        // ��� ������ ���� ���� �� ��� �������
        PositionRawImage();
    }

    // ��� ������ ���� ��� ����� ��� �������
    void PositionRawImage()
    {
        // ������ ����� ����� ��� ��� �������
        float angle = Random.Range(angleStart, angleEnd); // ������ ����� ������� ���� ��� �������

        // ����� ������� ��� ������
        float radian = angle * Mathf.Deg2Rad;

        // ���� ���� ������ ����� ��� �������
        Vector3 position = new Vector3(Mathf.Cos(radian) * radius, Mathf.Sin(radian) * radius, 0f);

        // ��� ������ �� ������ ������ ������� �������
        rawImageRectTransform.position = spaceship.position + position;

        // ��� ������ ����� �������� ������
        rawImageRectTransform.LookAt(Camera.main.transform);
    }

    // ��� ����� ������ɡ �� ������ ������
    void Update()
    {
        // ��� ���� ������� ����ߡ ���� ����� ������ ���� ����
        PositionRawImage();
    }
}
