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
    public Image blueLaserIcon;
    public Image speedBoostIcon;
    public Image shieldIcon;
    public Image multiShotIcon;

    [Header("Settings")]
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(1, 1, 1, 0.3f);

    // مخزون القدرات
    private Dictionary<PowerUpType, int> powerUpInventory = new Dictionary<PowerUpType, int>();

    void Start()
    {
        InitializeInventory();
        UpdateUI();
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
        powerUpInventory[type]++;
        UpdateUI();

        Debug.Log($"➕ {GetPowerUpName(type)} | المخزون: {powerUpInventory[type]}");
    }

    // استخدام قدرة من المخزون
    public bool UsePowerUp(PowerUpType type)
    {
        if (powerUpInventory[type] > 0)
        {
            powerUpInventory[type]--;
            UpdateUI();

            Debug.Log($"➖ {GetPowerUpName(type)} | المخزون: {powerUpInventory[type]}");
            return true;
        }
        else
        {
            Debug.LogWarning($"❌ لا يوجد {GetPowerUpName(type)} في المخزون");
            return false;
        }
    }

    // الحصول على عدد القدرات المتاحة
    public int GetPowerUpCount(PowerUpType type)
    {
        return powerUpInventory[type];
    }

    // تحديث واجهة المستخدم
    void UpdateUI()
    {
        // تحديث النصوص
        UpdateText(blueLaserCountText, PowerUpType.BlueLaser);
        UpdateText(speedBoostCountText, PowerUpType.SpeedBoost);
        UpdateText(shieldCountText, PowerUpType.Shield);
        UpdateText(multiShotCountText, PowerUpType.MultiShot);

        // تحديث الأيقونات
        UpdateIcon(blueLaserIcon, PowerUpType.BlueLaser);
        UpdateIcon(speedBoostIcon, PowerUpType.SpeedBoost);
        UpdateIcon(shieldIcon, PowerUpType.Shield);
        UpdateIcon(multiShotIcon, PowerUpType.MultiShot);
    }

    void UpdateText(Text textElement, PowerUpType type)
    {
        if (textElement != null)
        {
            textElement.text = powerUpInventory[type].ToString();

            // تغيير لون النص إذا كان المخزون 0
            textElement.color = powerUpInventory[type] > 0 ? Color.white : Color.gray;
        }
    }

    void UpdateIcon(Image iconElement, PowerUpType type)
    {
        if (iconElement != null)
        {
            // تغيير شفافية الأيقونة حسب المخزون
            iconElement.color = powerUpInventory[type] > 0 ? activeColor : inactiveColor;
        }
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
    public void AddTestPowerUps()
    {
        AddPowerUp(PowerUpType.BlueLaser);
        AddPowerUp(PowerUpType.SpeedBoost);
        AddPowerUp(PowerUpType.Shield);
        AddPowerUp(PowerUpType.MultiShot);
    }

    // مسح جميع القدرات (لإعادة الضبط)
    public void ClearAllPowerUps()
    {
        powerUpInventory[PowerUpType.BlueLaser] = 0;
        powerUpInventory[PowerUpType.SpeedBoost] = 0;
        powerUpInventory[PowerUpType.Shield] = 0;
        powerUpInventory[PowerUpType.MultiShot] = 0;

        UpdateUI();
        Debug.Log("🗑️ تم مسح جميع القدرات");
    }
}