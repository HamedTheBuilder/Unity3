using UnityEngine;
using System.Collections;

public class PowerUpSystem : MonoBehaviour
{
    [Header("Power Up Manager")]
    public PowerUpManager powerUpManager;

    [Header("Power Up Duration")]
    public float powerUpDuration = 6f;

    [Header("Ship Light Settings")]
    public Light shipLight;

    [Header("Shield Settings")]
    public GameObject shieldPrefab;
    private GameObject currentShield;

    [Header("Laser Settings")]
    public GameObject laserPrefab; // بريفاب الليزر الأساسي
    public Material blueLaserMaterial; // الماتيرال الأزرق من الإنسبكتور
    private Material originalLaserMaterial;
    public SimpleLaserGun laserGun;

    private SpaceshipMovement spaceshipMovement;
    private Coroutine currentPowerUpCoroutine;
    private bool isPowerUpActive = false;

    void Start()
    {
        spaceshipMovement = GetComponent<SpaceshipMovement>();
        laserGun = GetComponent<SimpleLaserGun>();

        // حفظ الماتيرال الأصلي للليزر
        if (laserPrefab != null)
        {
            Renderer laserRenderer = laserPrefab.GetComponent<Renderer>();
            if (laserRenderer != null)
            {
                originalLaserMaterial = laserRenderer.sharedMaterial;
            }
        }

        if (shipLight != null)
        {
            shipLight.enabled = false;
        }

        Debug.Log("✅ نظام القدرات جاهز");
    }

    void Update()
    {
        HandlePowerUpInput();
    }

    void HandlePowerUpInput()
    {
        // لا تتحقق إذا كانت قدرة نشطة - يسمح باستخدام القدرات مباشرة
        if (Input.GetKeyDown(KeyCode.Alpha1)) // زر 1 - ليزر أزرق
        {
            TryUsePowerUp(PowerUpType.BlueLaser);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // زر 2 - سرعة
        {
            TryUsePowerUp(PowerUpType.SpeedBoost);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // زر 3 - درع
        {
            TryUsePowerUp(PowerUpType.Shield);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) // زر 4 - إطلاق متعدد
        {
            TryUsePowerUp(PowerUpType.MultiShot);
        }
    }

    void TryUsePowerUp(PowerUpType type)
    {
        if (powerUpManager != null && powerUpManager.UsePowerUp(type))
        {
            Debug.Log($"🎯 استخدام قدرة: {type}");
            StartCoroutine(ActivatePowerUp(type));
        }
        else
        {
            Debug.LogWarning($"❌ لا يمكن استخدام {type} - لا يوجد في المخزون");
        }
    }

    public void CollectPowerUp(PowerUpType type)
    {
        Debug.Log($"🎁 جمع قدرة: {type}");

        if (powerUpManager != null)
        {
            powerUpManager.AddPowerUp(type);
        }
    }

    IEnumerator ActivatePowerUp(PowerUpType type)
    {
        // لا نمنع استخدام قدرات جديدة - يسمح باستخدام أكثر من قدرة

        // إيقاف القدرة السابقة إذا كانت نشطة
        if (currentPowerUpCoroutine != null)
        {
            StopCoroutine(currentPowerUpCoroutine);
            ResetToNormal();
        }

        // تفعيل القدرة الجديدة
        switch (type)
        {
            case PowerUpType.BlueLaser:
                currentPowerUpCoroutine = StartCoroutine(BlueLaserRoutine());
                break;
            case PowerUpType.SpeedBoost:
                currentPowerUpCoroutine = StartCoroutine(SpeedBoostRoutine());
                break;
            case PowerUpType.Shield:
                currentPowerUpCoroutine = StartCoroutine(ShieldRoutine());
                break;
            case PowerUpType.MultiShot:
                currentPowerUpCoroutine = StartCoroutine(MultiShotRoutine());
                break;
        }

        yield return new WaitForSeconds(powerUpDuration);

        // انتهاء القدرة
        ResetToNormal();
        currentPowerUpCoroutine = null;

        Debug.Log($"⏰ انتهت قدرة: {type}");
    }

    IEnumerator BlueLaserRoutine()
    {
        Debug.Log("🔵 ليزر أزرق مفعل!");
        ChangeShipLight(Color.blue);

        // تغيير ماتيرال الليزر إلى أزرق
        if (laserPrefab != null && blueLaserMaterial != null)
        {
            Renderer laserRenderer = laserPrefab.GetComponent<Renderer>();
            if (laserRenderer != null)
            {
                laserRenderer.material = blueLaserMaterial;
                Debug.Log("✅ تم تغيير لون الليزر إلى أزرق");
            }
        }
        else
        {
            Debug.LogWarning("❌ بريفاب الليزر أو الماتيرال الأزرق غير موصول!");
        }

        yield return new WaitForSeconds(powerUpDuration);

        // إعادة الماتيرال الأصلي للليزر
        if (laserPrefab != null && originalLaserMaterial != null)
        {
            Renderer laserRenderer = laserPrefab.GetComponent<Renderer>();
            if (laserRenderer != null)
            {
                laserRenderer.material = originalLaserMaterial;
                Debug.Log("🔄 تم إعادة لون الليزر الأصلي");
            }
        }
    }

    IEnumerator SpeedBoostRoutine()
    {
        Debug.Log("⚡ سرعة الحركة مفعلة!");
        ChangeShipLight(Color.yellow);

        float originalSpeed = spaceshipMovement.speed;

        // زيادة السرعة
        if (spaceshipMovement != null)
        {
            spaceshipMovement.speed *= 2f;
            Debug.Log($"🚀 السرعة: {originalSpeed} → {spaceshipMovement.speed}");
        }

        yield return new WaitForSeconds(powerUpDuration);

        // إعادة السرعة الأصلية
        if (spaceshipMovement != null)
        {
            spaceshipMovement.speed = originalSpeed;
            Debug.Log($"🔄 العودة للسرعة: {spaceshipMovement.speed}");
        }
    }

    IEnumerator ShieldRoutine()
    {
        Debug.Log("🛡️ الدرع مفعل!");
        ChangeShipLight(Color.red);

        // إنشاء الدرع
        if (shieldPrefab != null)
        {
            currentShield = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
            currentShield.transform.SetParent(transform);
            currentShield.transform.localPosition = Vector3.zero;

            Debug.Log("✅ تم إنشاء الدرع");
        }
        else
        {
            Debug.LogWarning("❌ بريفاب الدرع غير موصول!");
        }

        yield return new WaitForSeconds(powerUpDuration);

        // إزالة الدرع بعد انتهاء المدة
        if (currentShield != null)
        {
            Destroy(currentShield);
            currentShield = null;
            Debug.Log("🗑️ تم إزالة الدرع");
        }
    }

    IEnumerator MultiShotRoutine()
    {
        Debug.Log("💜 Multi-Shot مفعل!");
        ChangeShipLight(new Color(0.8f, 0.2f, 0.8f));

        // تفعيل نظام الإطلاق المتعدد
        if (laserGun != null)
        {
            var originalFirePoints = laserGun.firePoints;
            var originalFiringMode = laserGun.firingMode;

            laserGun.firePoints = CreateMultiShotFirePoints();
            laserGun.firingMode = SimpleLaserGun.FiringMode.Simultaneous;

            Debug.Log("🔫 تفعيل الإطلاق المتعدد - 5 مسارات");

            yield return new WaitForSeconds(powerUpDuration);

            laserGun.firePoints = originalFirePoints;
            laserGun.firingMode = originalFiringMode;
            CleanupMultiShotFirePoints();

            Debug.Log("🔄 العودة للإطلاق العادي");
        }
    }

    Transform[] CreateMultiShotFirePoints()
    {
        Transform[] multiShotPoints = new Transform[5];
        multiShotPoints[0] = CreateFirePoint("Center", new Vector3(0f, 0f, -0.5f), Quaternion.Euler(0, 180, 0));
        multiShotPoints[1] = CreateFirePoint("TopRight", new Vector3(0.3f, 0.2f, -0.5f), Quaternion.Euler(-10, 180 + 15, 0));
        multiShotPoints[2] = CreateFirePoint("TopLeft", new Vector3(-0.3f, 0.2f, -0.5f), Quaternion.Euler(-10, 180 - 15, 0));
        multiShotPoints[3] = CreateFirePoint("BottomRight", new Vector3(0.2f, -0.2f, -0.5f), Quaternion.Euler(10, 180 + 10, 0));
        multiShotPoints[4] = CreateFirePoint("BottomLeft", new Vector3(-0.2f, -0.2f, -0.5f), Quaternion.Euler(10, 180 - 10, 0));
        return multiShotPoints;
    }

    Transform CreateFirePoint(string name, Vector3 position, Quaternion rotation)
    {
        GameObject firePoint = new GameObject($"MultiShot_{name}");
        firePoint.transform.SetParent(transform);
        firePoint.transform.localPosition = position;
        firePoint.transform.localRotation = rotation;
        return firePoint.transform;
    }

    void CleanupMultiShotFirePoints()
    {
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("MultiShot_"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    void ChangeShipLight(Color color)
    {
        if (shipLight != null)
        {
            shipLight.color = color;
            shipLight.enabled = true;
        }
    }

    void ResetToNormal()
    {
        // إطفاء النور
        if (shipLight != null)
        {
            shipLight.enabled = false;
        }

        // التأكد من إزالة الدرع
        if (currentShield != null)
        {
            Destroy(currentShield);
            currentShield = null;
        }
    }
}