using UnityEngine;

public class AsteroidRotation : MonoBehaviour
{
    public float rotationSpeed = 50f; // ���� ������ѡ ���� ������� �� ��� Inspector

    void Update()
    {
        // ����� ������� ��� ������ Z
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
