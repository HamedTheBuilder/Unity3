using UnityEngine;

public class ProximityLights : MonoBehaviour
{
    [Header("إعدادات منطقة الكشف")]
    [Tooltip("طول منطقة الكشف")]
    public float detectionLength = 8f;
    [Tooltip("عرض منطقة الكشف")]
    public float detectionWidth = 3f;
    [Tooltip("ارتفاع منطقة الكشف")]
    public float detectionHeight = 3f;

    private Light myLight;
    private bool latched = false;
    private Transform player;

    void Start()
    {
        // الحصول على اللمبة المرفقة
        myLight = GetComponent<Light>();

        if (myLight == null)
        {
            Debug.LogError("مافيه لايت!");
            return;
        }

        myLight.enabled = false;

        // البحث عن اللاعب
        FindPlayer();
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        // إذا اللمبة شغالة خلاص، ما نكمل
        if (latched || myLight == null) return;

        // إذا مافيه لاعب، نحاول نلقاه
        if (player == null)
        {
            FindPlayer();
            return;
        }

        // التحقق إذا اللاعب داخل المنطقة المستطيلة
        if (IsPlayerInDetectionArea())
        {
            myLight.enabled = true;
            latched = true;
            Debug.Log("💡 اللمبة اشتغلت!");
        }
    }

    bool IsPlayerInDetectionArea()
    {
        if (player == null) return false;

        // حساب المسافات المحلية بالنسبة لللمبة
        Vector3 localPlayerPos = transform.InverseTransformPoint(player.position);

        // التحقق إذا اللاعب داخل المستطيل
        bool inLength = Mathf.Abs(localPlayerPos.z) <= detectionLength / 2f;
        bool inWidth = Mathf.Abs(localPlayerPos.x) <= detectionWidth / 2f;
        bool inHeight = Mathf.Abs(localPlayerPos.y) <= detectionHeight / 2f;

        return inLength && inWidth && inHeight;
    }

    // إذا تبي تعيد الضوء
    public void ResetTheLight()
    {
        latched = false;
        if (myLight != null)
            myLight.enabled = false;
    }

    // لعرض منطقة الكشف في المحرر
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(detectionWidth, detectionHeight, detectionLength));

        // رسم خطوط مساعدة
        Gizmos.color = Color.green * 0.5f;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(detectionWidth, detectionHeight, detectionLength));
    }
}