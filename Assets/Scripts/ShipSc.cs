using UnityEngine;

public class SpaceshipMovement : MonoBehaviour
{
    public float speed = 5f; // ���� ���� �������
    public float limitX = 3f; // ���� ������ ������ ��� ������ X
    public float limitY = 3f; // ���� ������ ������ ��� ������ Y (������ �������)
    public float rotationSpeed = 5f; // ���� �������
    private float moveInputX; // �������� ��� ������ X
    private float moveInputY; // �������� ��� ������ Y

    private float currentRotationX = 0f; // ������� ������ ��� ������ X
    private float currentRotationZ = 0f; // ������� ������ ��� ������ Z
    private float targetRotationX = 0f; // ������� ����� ��� ������ X
    private float targetRotationZ = 0f; // ������� ����� ��� ������ Z
    private float rotationVelocityX = 0f; // ���� ������� ��� ������ X
    private float rotationVelocityZ = 0f; // ���� ������� ��� ������ Z

    void Update()
    {
        // ������ ��� ������ ������ �� �������� (������ �� A � D ������ ��� X)
        moveInputX = Input.GetAxis("Horizontal");
        moveInputY = Input.GetAxis("Vertical"); // ������ ��� �������� ������ �������

        // ����� �������
        MoveShip();

        // ���� ������� ��� ������ X ����� ��� ������ ��� ������ Y
        CalculateRotationX();

        // ���� ������� ��� ������ Z ����� ��� ������ ��� ������ X
        CalculateRotationZ();

        // ����� ������� ����� �������� SmoothDamp
        ApplyRotation();
    }

    // ����� ������� ��� ������� X � Y
    void MoveShip()
    {
        // ����� ������� ��� ������ X
        float newX = transform.position.x + moveInputX * speed * Time.deltaTime;

        // ����� ���� ������ ��� ������ X
        newX = Mathf.Clamp(newX, -limitX, limitX);

        // ����� ������� ��� ������ Y
        float newY = transform.position.y + moveInputY * speed * Time.deltaTime;

        // ����� ���� ������ ��� ������ Y
        newY = Mathf.Clamp(newY, -limitY, limitY);

        // ����� ���� �������
        transform.position = new Vector3(newX, newY, transform.position.z);
    }

    // ���� ������� ��� ������ X (������ �������)
    void CalculateRotationX()
    {
        if (moveInputY > 0) // ����� ����� ������
        {
            targetRotationX = -10f; // ����� ������
        }
        else if (moveInputY < 0) // ����� ����� ������
        {
            targetRotationX = 10f; // ����� ������
        }
        else // ��� ���� ������ ����� �� �� ���� ���� ��� ������ Y
        {
            targetRotationX = 0f; // ����� �������
        }
    }

    // ���� ������� ��� ������ Z (������ �������)
    void CalculateRotationZ()
    {
        targetRotationZ = Mathf.Clamp(moveInputX * -30f, -30f, 30f); // ������� ������ �� ������
    }

    // ����� ������� �������� SmoothDamp ���� ������� ���� �����
    void ApplyRotation()
    {
        // ����� ������� ��� ������ X (���� �����)
        currentRotationX = Mathf.SmoothDamp(currentRotationX, targetRotationX, ref rotationVelocityX, 0.1f);

        // ����� ������� ��� ������ Z (���� �����)
        currentRotationZ = Mathf.SmoothDamp(currentRotationZ, targetRotationZ, ref rotationVelocityZ, 0.1f);

        // ����� ������� ��� �������
        Quaternion targetRotation = Quaternion.Euler(currentRotationX, 0f, currentRotationZ);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
