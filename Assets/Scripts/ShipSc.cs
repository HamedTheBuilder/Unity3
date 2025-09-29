using UnityEngine;

public class SpaceshipMovement : MonoBehaviour
{
    public float speed = 5f; // ÓÑÚÉ ÍÑßÉ ÇáÓİíäÉ
    public float limitX = 3f; // ÇáÍÏ ÇáÃŞÕì ááãæŞÚ Úáì ÇáãÍæÑ X
    public float limitY = 3f; // ÇáÍÏ ÇáÃŞÕì ááãæŞÚ Úáì ÇáãÍæÑ Y (ÇáÃÚáì æÇáÃÓİá)
    public float rotationSpeed = 5f; // ÓÑÚÉ ÇáÊÏæíÑ
    private float moveInputX; // ÇáãÏÎáÇÊ Úáì ÇáãÍæÑ X
    private float moveInputY; // ÇáãÏÎáÇÊ Úáì ÇáãÍæÑ Y

    private float currentRotationX = 0f; // ÇáÊÏæíÑ ÇáÍÇáí Íæá ÇáãÍæÑ X
    private float currentRotationZ = 0f; // ÇáÊÏæíÑ ÇáÍÇáí Íæá ÇáãÍæÑ Z
    private float targetRotationX = 0f; // ÇáÊÏæíÑ ÇáåÏİ Íæá ÇáãÍæÑ X
    private float targetRotationZ = 0f; // ÇáÊÏæíÑ ÇáåÏİ Íæá ÇáãÍæÑ Z
    private float rotationVelocityX = 0f; // ÓÑÚÉ ÇáÊÏæíÑ Úáì ÇáãÍæÑ X
    private float rotationVelocityZ = 0f; // ÓÑÚÉ ÇáÊÏæíÑ Úáì ÇáãÍæÑ Z

    void Update()
    {
        // ÇáÍÕæá Úáì ãÏÎáÇÊ ÇáÍÑßÉ ãä ÇáãÓÊÎÏã (ÇáÃÓåã Ãæ A æ D ááÊÍÑß Úáì X)
        moveInputX = Input.GetAxis("Horizontal");
        moveInputY = Input.GetAxis("Vertical"); // ÇáÍÕæá Úáì ÇáãÏÎáÇÊ ááÃÚáì æÇáÃÓİá

        // ÊÍÑíß ÇáÓİíäÉ
        MoveShip();

        // ÍÓÇÈ ÇáÊÏæíÑ Íæá ÇáãÍæÑ X ÈäÇÁğ Úáì ÇáÍÑßÉ Úáì ÇáãÍæÑ Y
        CalculateRotationX();

        // ÍÓÇÈ ÇáÊÏæíÑ Íæá ÇáãÍæÑ Z ÈäÇÁğ Úáì ÇáÍÑßÉ Úáì ÇáãÍæÑ X
        CalculateRotationZ();

        // ÊØÈíŞ ÇáÊÏæíÑ ÇáÓáÓ ÈÇÓÊÎÏÇã SmoothDamp
        ApplyRotation();
    }

    // ÊÍÑíß ÇáÓİíäÉ Úáì ÇáãÍÇæÑ X æ Y
    void MoveShip()
    {
        // ÊÍÑíß ÇáÓİíäÉ Úáì ÇáãÍæÑ X
        float newX = transform.position.x + moveInputX * speed * Time.deltaTime;

        // ÊÍÏíÏ ÍÏæÏ ÇáÍÑßÉ Úáì ÇáãÍæÑ X
        newX = Mathf.Clamp(newX, -limitX, limitX);

        // ÊÍÑíß ÇáÓİíäÉ Úáì ÇáãÍæÑ Y
        float newY = transform.position.y + moveInputY * speed * Time.deltaTime;

        // ÊÍÏíÏ ÍÏæÏ ÇáÍÑßÉ Úáì ÇáãÍæÑ Y
        newY = Mathf.Clamp(newY, -limitY, limitY);

        // ÊÍÏíË ãæŞÚ ÇáÓİíäÉ
        transform.position = new Vector3(newX, newY, transform.position.z);
    }

    // ÍÓÇÈ ÇáÊÏæíÑ Úáì ÇáãÍæÑ X (ááÃÚáì æÇáÃÓİá)
    void CalculateRotationX()
    {
        if (moveInputY > 0) // ÚäÏãÇ ÊÊÍÑß ááÃÚáì
        {
            targetRotationX = -10f; // Çáãíá ááÃÚáì
        }
        else if (moveInputY < 0) // ÚäÏãÇ ÊÊÍÑß ááÃÓİá
        {
            targetRotationX = 10f; // Çáãíá ááÃÓİá
        }
        else // ÅĞÇ ßÇäÊ ÇáÍÑßÉ ËÇÈÊÉ Ãæ áÇ íæÌÏ ÍÑßÉ Úáì ÇáãÍæÑ Y
        {
            targetRotationX = 0f; // ÅÚÇÏÉ ÇáÊæÇÒä
        }
    }

    // ÍÓÇÈ ÇáÊÏæíÑ Úáì ÇáãÍæÑ Z (ááíãíä æÇáíÓÇÑ)
    void CalculateRotationZ()
    {
        targetRotationZ = Mathf.Clamp(moveInputX * -30f, -30f, 30f); // ÇáÊÏæíÑ ááíãíä Ãæ ááíÓÇÑ
    }

    // ÊØÈíŞ ÇáÊÏæíÑ ÈÇÓÊÎÏÇã SmoothDamp áÌÚá ÇáÊÏæíÑ ÃßËÑ ÓáÇÓÉ
    void ApplyRotation()
    {
        // ÊÏæíÑ ÇáÓİíäÉ Íæá ÇáãÍæÑ X (ÃÚáì æÃÓİá)
        currentRotationX = Mathf.SmoothDamp(currentRotationX, targetRotationX, ref rotationVelocityX, 0.1f);

        // ÊÏæíÑ ÇáÓİíäÉ Íæá ÇáãÍæÑ Z (íãíä æíÓÇÑ)
        currentRotationZ = Mathf.SmoothDamp(currentRotationZ, targetRotationZ, ref rotationVelocityZ, 0.1f);

        // ÊØÈíŞ ÇáÊÏæíÑ Úáì ÇáÓİíäÉ
        Quaternion targetRotation = Quaternion.Euler(currentRotationX, 0f, currentRotationZ);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
