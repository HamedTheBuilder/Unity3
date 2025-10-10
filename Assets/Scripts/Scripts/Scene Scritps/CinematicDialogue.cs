using UnityEngine;

public class SideViewCamera3D : MonoBehaviour
{
    [Header("ÅÚÏÇÏÇÊ ÇáßÇãíÑÇ")]
    public Transform target;         // ÇááÇÚÈ
    public float smoothSpeed = 5f;   // ÓÑÚÉ ÊÊÈÚ ÇáßÇãíÑÇ
    public Vector3 offset = new Vector3(0f, 2f, -10f); // ÇáãÓÇİÉ ãä ÇááÇÚÈ

    private void LateUpdate()
    {
        if (target == null) return;

        // İŞØ ÊÊÈÚ ÇáãÍæÑ X æ Y — ÇáßÇãíÑÇ ËÇÈÊÉ İí Z
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, target.position.z) + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // äÎáí ÇáßÇãíÑÇ ÊäÙÑ ÏÇÆãğÇ ááÃãÇã (ÈÏæä ÏæÑÇä ÛÑíÈ)
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}