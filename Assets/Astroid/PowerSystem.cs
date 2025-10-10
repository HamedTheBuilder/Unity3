using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public GameObject normalLaserPrefab;
    public GameObject blueLaserPrefab;
    public GameObject multiShotLaserPrefab;

    [Header("Score Settings - NEW")]
    public int powerUpScore = 10; // النقاط اللي تاخذها من كل قدرة

    // تتبع القدرات النشطة
    private Dictionary<PowerUpType, Coroutine> activePowerUps = new Dictionary<PowerUpType, Coroutine>();
    private Dictionary<PowerUpType, float> powerUpEndTimes = new Dictionary<PowerUpType, float>();

    // الإعدادات الأصلية
    private Transform[] originalFirePoints;
    private SimpleLaserGun.FiringMode originalFiringMode;
    private GameObject originalLaserPrefab;
    private float originalSpeed;

    private SimpleLaserGun laserGun;
    private SpaceshipMovement spaceshipMovement;

    void Start()
    {
        spaceshipMovement = GetComponent<SpaceshipMovement>();
        laserGun = GetComponent<SimpleLaserGun>();

        // حفظ الإعدادات الأصلية
        if (laserGun != null)
        {
            originalFirePoints = laserGun.firePoints;
            originalFiringMode = laserGun.firingMode;
            originalLaserPrefab = laserGun.laserPrefab;
        }

        if (spaceshipMovement != null)
        {
            originalSpeed = spaceshipMovement.speed;
        }

        if (shipLight != null)
        {
            shipLight.enabled = false;
        }

        // تهيئة القواميس
        activePowerUps.Clear();
        powerUpEndTimes.Clear();

        Debug.Log("✅ نظام القدرات جاهز");
    }

    void Update()
    {
        HandlePowerUpInput();
        UpdatePowerUpTimers();
    }

    void HandlePowerUpInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TryUsePowerUp(PowerUpType.BlueLaser);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TryUsePowerUp(PowerUpType.SpeedBoost);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TryUsePowerUp(PowerUpType.Shield);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TryUsePowerUp(PowerUpType.MultiShot);
        }
    }

    void TryUsePowerUp(PowerUpType type)
    {
        if (powerUpManager != null && powerUpManager.UsePowerUp(type))
        {
            Debug.Log($"🎯 استخدام قدرة: {type}");
            ActivatePowerUp(type);
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
            Debug.Log($"✅ تمت إضافة {type} إلى المخزون - العدد: {powerUpManager.GetPowerUpCount(type)}");

            // إضافة نقاط عند جمع القدرة - NEW
            AddPowerUpScore(type);
        }
    }

    // دالة جديدة لإضافة النقاط عند جمع القدرة - NEW
    void AddPowerUpScore(PowerUpType type)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(powerUpScore);
            Debug.Log($"💰 +{powerUpScore} نقطة لجمع {GetPowerUpName(type)}");
        }
        else
        {
            Debug.LogWarning("❌ GameManager غير موجود لإضافة النقاط");
        }
    }

    // دالة مساعدة للحصول على اسم القدرة - NEW
    string GetPowerUpName(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.BlueLaser: return "ليزر أزرق";
            case PowerUpType.SpeedBoost: return "سرعة";
            case PowerUpType.Shield: return "درع";
            case PowerUpType.MultiShot: return "إطلاق متعدد";
            default: return "قدرة";
        }
    }

    void ActivatePowerUp(PowerUpType type)
    {
        // إلغاء القدرة السابقة إذا كانت نشطة
        if (activePowerUps.ContainsKey(type))
        {
            StopCoroutine(activePowerUps[type]);
            activePowerUps.Remove(type);
            DeactivatePowerUp(type);
        }

        // تفعيل القدرة الجديدة
        Coroutine powerUpCoroutine = StartCoroutine(PowerUpRoutine(type));
        activePowerUps[type] = powerUpCoroutine;
        powerUpEndTimes[type] = Time.time + powerUpDuration;

        // تطبيق تأثيرات القدرة
        ApplyPowerUpEffects(type);
    }

    IEnumerator PowerUpRoutine(PowerUpType type)
    {
        Debug.Log($"⏳ تفعيل قدرة: {type} لمدة {powerUpDuration} ثانية");

        yield return new WaitForSeconds(powerUpDuration);

        // انتهاء القدرة
        DeactivatePowerUp(type);
        activePowerUps.Remove(type);
        powerUpEndTimes.Remove(type);

        Debug.Log($"⏰ انتهت قدرة: {type}");
    }

    void ApplyPowerUpEffects(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.BlueLaser:
                ApplyBlueLaser();
                break;
            case PowerUpType.SpeedBoost:
                ApplySpeedBoost();
                break;
            case PowerUpType.Shield:
                ApplyShield();
                break;
            case PowerUpType.MultiShot:
                ApplyMultiShot();
                break;
        }
    }

    void DeactivatePowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.BlueLaser:
                RemoveBlueLaser();
                break;
            case PowerUpType.SpeedBoost:
                RemoveSpeedBoost();
                break;
            case PowerUpType.Shield:
                RemoveShield();
                break;
            case PowerUpType.MultiShot:
                RemoveMultiShot();
                break;
        }

        // إعادة الضوء إلى وضعه الطبيعي إذا لم تكن هناك قدرات نشطة
        if (activePowerUps.Count == 0 && shipLight != null)
        {
            shipLight.enabled = false;
        }
    }

    void UpdatePowerUpTimers()
    {
        List<PowerUpType> expiredPowerUps = new List<PowerUpType>();

        foreach (var kvp in powerUpEndTimes)
        {
            if (Time.time >= kvp.Value)
            {
                expiredPowerUps.Add(kvp.Key);
            }
        }

        foreach (var type in expiredPowerUps)
        {
            if (activePowerUps.ContainsKey(type))
            {
                StopCoroutine(activePowerUps[type]);
                activePowerUps.Remove(type);
            }
            powerUpEndTimes.Remove(type);
            DeactivatePowerUp(type);
            Debug.Log($"🕒 انتهت قدرة: {type} (تلقائي)");
        }
    }

    // دوال التطبيق والإلغاء لكل قدرة
    void ApplyBlueLaser()
    {
        Debug.Log("🔵 ليزر أزرق مفعل!");
        ChangeShipLight(Color.blue);

        if (laserGun != null && blueLaserPrefab != null)
        {
            laserGun.laserPrefab = blueLaserPrefab;
        }
    }

    void RemoveBlueLaser()
    {
        if (laserGun != null && normalLaserPrefab != null)
        {
            laserGun.laserPrefab = normalLaserPrefab;
            Debug.Log("🔄 تم إعادة الليزر العادي");
        }
    }

    void ApplySpeedBoost()
    {
        Debug.Log("⚡ سرعة الحركة مفعلة!");
        ChangeShipLight(Color.yellow);

        if (spaceshipMovement != null)
        {
            spaceshipMovement.speed = originalSpeed * 2f;
        }
    }

    void RemoveSpeedBoost()
    {
        if (spaceshipMovement != null)
        {
            spaceshipMovement.speed = originalSpeed;
            Debug.Log("🔄 تم إعادة السرعة الأصلية");
        }
    }

    void ApplyShield()
    {
        Debug.Log("🛡️ الدرع مفعل!");
        ChangeShipLight(Color.red);

        if (shieldPrefab != null && currentShield == null)
        {
            currentShield = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
            currentShield.transform.SetParent(transform);
            currentShield.transform.localPosition = Vector3.zero;
        }
    }

    void RemoveShield()
    {
        if (currentShield != null)
        {
            Destroy(currentShield);
            currentShield = null;
            Debug.Log("🗑️ تم إزالة الدرع");
        }
    }

    void ApplyMultiShot()
    {
        Debug.Log("💜 Multi-Shot مفعل!");
        ChangeShipLight(new Color(0.8f, 0.2f, 0.8f));

        if (laserGun != null)
        {
            if (multiShotLaserPrefab != null)
            {
                laserGun.laserPrefab = multiShotLaserPrefab;
            }

            laserGun.firePoints = CreateMultiShotFirePoints();
            laserGun.firingMode = SimpleLaserGun.FiringMode.Simultaneous;
        }
    }

    void RemoveMultiShot()
    {
        if (laserGun != null)
        {
            laserGun.firePoints = originalFirePoints;
            laserGun.firingMode = originalFiringMode;
            laserGun.laserPrefab = originalLaserPrefab;

            CleanupMultiShotFirePoints();
            Debug.Log("🔄 تم إعادة إعدادات الإطلاق العادية");
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

    // دالة جديدة: إلغاء جميع القدرات
    public void DeactivateAllPowerUps()
    {
        Debug.Log("🗑️ إلغاء جميع القدرات النشطة");

        // إيقاف جميع الكوروتينات
        foreach (var coroutine in activePowerUps.Values)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        // إلغاء جميع تأثيرات القدرات
        RemoveBlueLaser();
        RemoveSpeedBoost();
        RemoveShield();
        RemoveMultiShot();

        // مسح القواميس
        activePowerUps.Clear();
        powerUpEndTimes.Clear();

        // إطفاء ضوء السفينة
        if (shipLight != null)
        {
            shipLight.enabled = false;
        }

        Debug.Log("✅ تم إلغاء جميع القدرات بنجاح");
    }

    // دالة للمساعدة في عرض القدرات النشطة
    public void PrintActivePowerUps()
    {
        Debug.Log("📊 القدرات النشطة:");
        foreach (var type in activePowerUps.Keys)
        {
            float timeLeft = powerUpEndTimes[type] - Time.time;
            Debug.Log($"- {type}: {timeLeft:F1} ثانية متبقية");
        }
    }
}