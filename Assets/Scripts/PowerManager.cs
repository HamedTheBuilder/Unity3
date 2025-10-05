using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PowerUpManager : MonoBehaviour
{
    [Header("UI Text References")]
    public Text blueLaserCountText;
    public Text speedBoostCountText;
    public Text shieldCountText;
    public Text multiShotCountText;

    [Header("UI Icon References")]
    public RawImage blueLaserIcon;
    public RawImage speedBoostIcon;
    public RawImage shieldIcon;
    public RawImage multiShotIcon;

    [Header("Power Up Prefab Associations")]
    public GameObject blueLaserPowerUpPrefab;
    public GameObject speedBoostPowerUpPrefab;
    public GameObject shieldPowerUpPrefab;
    public GameObject multiShotPowerUpPrefab;

    [Header("Settings")]
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(1, 1, 1, 0.3f);

    private Dictionary<PowerUpType, int> powerUpInventory = new Dictionary<PowerUpType, int>();
    private Dictionary<GameObject, PowerUpType> prefabToTypeMap = new Dictionary<GameObject, PowerUpType>();

    void Start()
    {
        InitializePrefabMappings();
        InitializeInventory();
        UpdateAllUI();
    }

    void InitializePrefabMappings()
    {
        // ربط البريفابات مع أنواع القدرات
        if (blueLaserPowerUpPrefab != null)
            prefabToTypeMap[blueLaserPowerUpPrefab] = PowerUpType.BlueLaser;

        if (speedBoostPowerUpPrefab != null)
            prefabToTypeMap[speedBoostPowerUpPrefab] = PowerUpType.SpeedBoost;

        if (shieldPowerUpPrefab != null)
            prefabToTypeMap[shieldPowerUpPrefab] = PowerUpType.Shield;

        if (multiShotPowerUpPrefab != null)
            prefabToTypeMap[multiShotPowerUpPrefab] = PowerUpType.MultiShot;

        Debug.Log("✅ تم ربط البريفابات مع أنواع القدرات");
    }

    void InitializeInventory()
    {
        // بداية اللعبة بدون قدرات
        powerUpInventory[PowerUpType.BlueLaser] = 0;
        powerUpInventory[PowerUpType.SpeedBoost] = 0;
        powerUpInventory[PowerUpType.Shield] = 0;
        powerUpInventory[PowerUpType.MultiShot] = 0;

        Debug.Log("🔄 تم تهيئة مخزون القدرات");
    }

    // إضافة قدرة جديدة للمخزون
    public void AddPowerUp(PowerUpType type)
    {
        if (powerUpInventory.ContainsKey(type))
        {
            powerUpInventory[type]++;
            UpdateSpecificUI(type);
            Debug.Log($"➕ {GetPowerUpName(type)} | المخزون: {powerUpInventory[type]}");
        }
        else
        {
            Debug.LogWarning($"❌ نوع القدرة غير معترف به: {type}");
        }
    }

    // استخدام قدرة من المخزون
    public bool UsePowerUp(PowerUpType type)
    {
        if (powerUpInventory.ContainsKey(type) && powerUpInventory[type] > 0)
        {
            powerUpInventory[type]--;
            UpdateSpecificUI(type);
            Debug.Log($"➖ {GetPowerUpName(type)} | المخزون: {powerUpInventory[type]}");
            return true;
        }
        else
        {
            Debug.LogWarning($"❌ لا يوجد {GetPowerUpName(type)} في المخزون");
            return false;
        }
    }

    // تحديث واجهة المستخدم لقدرة محددة
    void UpdateSpecificUI(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.BlueLaser:
                UpdateTextAndIcon(blueLaserCountText, blueLaserIcon, type);
                break;
            case PowerUpType.SpeedBoost:
                UpdateTextAndIcon(speedBoostCountText, speedBoostIcon, type);
                break;
            case PowerUpType.Shield:
                UpdateTextAndIcon(shieldCountText, shieldIcon, type);
                break;
            case PowerUpType.MultiShot:
                UpdateTextAndIcon(multiShotCountText, multiShotIcon, type);
                break;
        }
    }

    // تحديث النص والأيقونة
    void UpdateTextAndIcon(Text textElement, RawImage iconElement, PowerUpType type)
    {
        // تحديث النص
        if (textElement != null)
        {
            textElement.text = powerUpInventory[type].ToString();

            // تغيير لون النص حسب الكمية
            if (powerUpInventory[type] > 0)
            {
                textElement.color = Color.white;
                textElement.fontStyle = FontStyle.Bold;
            }
            else
            {
                textElement.color = Color.gray;
                textElement.fontStyle = FontStyle.Normal;
            }
        }

        // تحديث الأيقونة
        if (iconElement != null)
        {
            iconElement.color = powerUpInventory[type] > 0 ? activeColor : inactiveColor;

            // تأثير بسيط عند التغيير
            if (powerUpInventory[type] > 0)
            {
                StartCoroutine(IconPulseAnimation(iconElement));
            }
        }
    }

    // تأثير نبض للأيقونة عند التفعيل
    System.Collections.IEnumerator IconPulseAnimation(RawImage icon)
    {
        if (icon == null) yield break;

        Vector3 originalScale = icon.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        // تكبير
        float timer = 0f;
        float duration = 0.1f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            icon.transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }

        // تصغير
        timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            icon.transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            yield return null;
        }

        icon.transform.localScale = originalScale;
    }

    // تحديث كل واجهة المستخدم
    void UpdateAllUI()
    {
        UpdateTextAndIcon(blueLaserCountText, blueLaserIcon, PowerUpType.BlueLaser);
        UpdateTextAndIcon(speedBoostCountText, speedBoostIcon, PowerUpType.SpeedBoost);
        UpdateTextAndIcon(shieldCountText, shieldIcon, PowerUpType.Shield);
        UpdateTextAndIcon(multiShotCountText, multiShotIcon, PowerUpType.MultiShot);
    }

    // الحصول على عدد القدرات المتاحة
    public int GetPowerUpCount(PowerUpType type)
    {
        return powerUpInventory.ContainsKey(type) ? powerUpInventory[type] : 0;
    }

    // الحصول على اسم القدرة بالعربية
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

    // إضافة قدرة بناءً على البريفاب
    public void AddPowerUpByPrefab(GameObject powerUpPrefab)
    {
        if (prefabToTypeMap.ContainsKey(powerUpPrefab))
        {
            PowerUpType type = prefabToTypeMap[powerUpPrefab];
            AddPowerUp(type);
        }
        else
        {
            Debug.LogWarning($"❌ البريفاب {powerUpPrefab.name} غير مرتبط بأي نوع قدرة");
        }
    }

    // عرض معلومات المخزون (للتشخيص)
    public void PrintInventory()
    {
        Debug.Log("📦 مخزون القدرات:");
        Debug.Log($"🔵 ليزر أزرق: {powerUpInventory[PowerUpType.BlueLaser]}");
        Debug.Log($"🟡 سرعة: {powerUpInventory[PowerUpType.SpeedBoost]}");
        Debug.Log($"🔴 درع: {powerUpInventory[PowerUpType.Shield]}");
        Debug.Log($"💜 إطلاق متعدد: {powerUpInventory[PowerUpType.MultiShot]}");
    }

    // إضافة قدرات للاختبار (اختياري)
    [ContextMenu("Add Test Power Ups")]
    public void AddTestPowerUps()
    {
        AddPowerUp(PowerUpType.BlueLaser);
        AddPowerUp(PowerUpType.SpeedBoost);
        AddPowerUp(PowerUpType.Shield);
        AddPowerUp(PowerUpType.MultiShot);
        Debug.Log("🧪 تمت إضافة قدرات اختبار");
    }

    // مسح جميع القدرات (لإعادة الضبط)
    [ContextMenu("Clear All Power Ups")]
    public void ClearAllPowerUps()
    {
        powerUpInventory[PowerUpType.BlueLaser] = 0;
        powerUpInventory[PowerUpType.SpeedBoost] = 0;
        powerUpInventory[PowerUpType.Shield] = 0;
        powerUpInventory[PowerUpType.MultiShot] = 0;

        UpdateAllUI();
        Debug.Log("🗑️ تم مسح جميع القدرات");
    }
}