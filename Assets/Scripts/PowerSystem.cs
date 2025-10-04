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
    public GameObject normalLaserPrefab;
    public GameObject blueLaserPrefab;
    public GameObject multiShotLaserPrefab;

    // حفظ الإعدادات الأصلية
    private Transform[] originalFirePoints;
    private SimpleLaserGun.FiringMode originalFiringMode;
    private GameObject originalLaserPrefab;
    private float originalSpeed;

    private SimpleLaserGun laserGun;
    private SpaceshipMovement spaceshipMovement;
    private Coroutine activePowerUpCoroutine;
    private PowerUpType currentActivePowerUp = PowerUpType.BlueLaser;

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

        Debug.Log("✅ نظام القدرات جاهز - الإعدادات الأصلية محفوظة");
    }

    void Update()
    {
        HandlePowerUpInput();
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
            StartPowerUp(type);
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

            // تفعيل فوري عند الجمع
            if (CanAutoActivate(type))
            {
                Debug.Log($"⚡ تفعيل فوري للقدرة: {type}");
                StartPowerUp(type);
            }
        }
    }

    // تحديد أي القدرات يتم تفعيلها فورياً عند الجمع
    bool CanAutoActivate(PowerUpType type)
    {
        // هنا يمكنك تحديد أي القدرات تتفعل فوراً
        // حالياً جميع القدرات تتفعل فوراً عند الجمع
        return true;
    }

    void StartPowerUp(PowerUpType type)
    {
        // إيقاف القدرة السابقة إذا كانت نشطة
        if (activePowerUpCoroutine != null)
        {
            StopCoroutine(activePowerUpCoroutine);
            ResetToNormal();
        }

        currentActivePowerUp = type;
        activePowerUpCoroutine = StartCoroutine(PowerUpRoutine(type));
    }

    IEnumerator PowerUpRoutine(PowerUpType type)
    {
        // تفعيل القدرة
        ActivatePowerUp(type);

        // الانتظار للمدة المحددة
        yield return new WaitForSeconds(powerUpDuration);

        // إلغاء القدرة
        DeactivatePowerUp(type);

        activePowerUpCoroutine = null;
        Debug.Log($"⏰ انتهت قدرة: {type}");
    }

    void ActivatePowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.BlueLaser:
                ActivateBlueLaser();
                break;
            case PowerUpType.SpeedBoost:
                ActivateSpeedBoost();
                break;
            case PowerUpType.Shield:
                ActivateShield();
                break;
            case PowerUpType.MultiShot:
                ActivateMultiShot();
                break;
        }
    }

    void DeactivatePowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.BlueLaser:
                DeactivateBlueLaser();
                break;
            case PowerUpType.SpeedBoost:
                DeactivateSpeedBoost();
                break;
            case PowerUpType.Shield:
                DeactivateShield();
                break;
            case PowerUpType.MultiShot:
                DeactivateMultiShot();
                break;
        }

        ResetToNormal();
    }

    void ActivateBlueLaser()
    {
        Debug.Log("🔵 ليزر أزرق مفعل!");
        ChangeShipLight(Color.blue);

        if (laserGun != null && blueLaserPrefab != null)
        {
            laserGun.laserPrefab = blueLaserPrefab;
        }
    }

    void DeactivateBlueLaser()
    {
        if (laserGun != null && normalLaserPrefab != null)
        {
            laserGun.laserPrefab = normalLaserPrefab;
            Debug.Log("🔄 تم إعادة الليزر العادي");
        }
    }

    void ActivateSpeedBoost()
    {
        Debug.Log("⚡ سرعة الحركة مفعلة!");
        ChangeShipLight(Color.yellow);

        if (spaceshipMovement != null)
        {
            spaceshipMovement.speed = originalSpeed * 2f;
        }
    }

    void DeactivateSpeedBoost()
    {
        if (spaceshipMovement != null)
        {
            spaceshipMovement.speed = originalSpeed;
            Debug.Log("🔄 تم إعادة السرعة الأصلية");
        }
    }

    void ActivateShield()
    {
        Debug.Log("🛡️ الدرع مفعل!");
        ChangeShipLight(Color.red);

        if (shieldPrefab != null)
        {
            currentShield = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
            currentShield.transform.SetParent(transform);
            currentShield.transform.localPosition = Vector3.zero;
        }
    }

    void DeactivateShield()
    {
        if (currentShield != null)
        {
            Destroy(currentShield);
            currentShield = null;
            Debug.Log("🗑️ تم إزالة الدرع");
        }
    }

    void ActivateMultiShot()
    {
        Debug.Log("💜 Multi-Shot مفعل!");
        ChangeShipLight(new Color(0.8f, 0.2f, 0.8f));

        if (laserGun != null)
        {
            // استخدام بريفاب الإطلاق المتعدد إذا متوفر
            if (multiShotLaserPrefab != null)
            {
                laserGun.laserPrefab = multiShotLaserPrefab;
            }

            // تفعيل نقاط الإطلاق المتعددة
            laserGun.firePoints = CreateMultiShotFirePoints();
            laserGun.firingMode = SimpleLaserGun.FiringMode.Simultaneous;
        }
    }

    void DeactivateMultiShot()
    {
        if (laserGun != null)
        {
            // إعادة الإعدادات الأصلية بشكل مؤكد
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

    void ResetToNormal()
    {
        if (shipLight != null)
        {
            shipLight.enabled = false;
        }
    }
}