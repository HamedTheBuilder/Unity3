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

    [Header("Power Up Prefab Associations - ASSIGN IN INSPECTOR")]
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
        UpdateUI();
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
        powerUpInventory[PowerUpType.BlueLaser] = 0;
        powerUpInventory[PowerUpType.SpeedBoost] = 0;
        powerUpInventory[PowerUpType.Shield] = 0;
        powerUpInventory[PowerUpType.MultiShot] = 0;
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

    // إضافة قدرة مباشرة بالنوع
    public void AddPowerUp(PowerUpType type)
    {
        if (powerUpInventory.ContainsKey(type))
        {
            powerUpInventory[type]++;
            UpdateSpecificUI(type);
            Debug.Log($"➕ تمت إضافة {GetPowerUpName(type)} | المخزون: {powerUpInventory[type]}");
        }
    }

    public bool UsePowerUp(PowerUpType type)
    {
        if (powerUpInventory.ContainsKey(type) && powerUpInventory[type] > 0)
        {
            powerUpInventory[type]--;
            UpdateSpecificUI(type);
            Debug.Log($"➖ تم استخدام {GetPowerUpName(type)} | المخزون: {powerUpInventory[type]}");
            return true;
        }
        return false;
    }

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

    void UpdateTextAndIcon(Text textElement, Image iconElement, PowerUpType type)
    {
        if (textElement != null)
        {
            textElement.text = powerUpInventory[type].ToString();
            textElement.color = powerUpInventory[type] > 0 ? Color.white : Color.gray;
        }

        if (iconElement != null)
        {
            iconElement.color = powerUpInventory[type] > 0 ? activeColor : inactiveColor;
        }
    }

    void UpdateUI()
    {
        UpdateTextAndIcon(blueLaserCountText, blueLaserIcon, PowerUpType.BlueLaser);
        UpdateTextAndIcon(speedBoostCountText, speedBoostIcon, PowerUpType.SpeedBoost);
        UpdateTextAndIcon(shieldCountText, shieldIcon, PowerUpType.Shield);
        UpdateTextAndIcon(multiShotCountText, multiShotIcon, PowerUpType.MultiShot);
    }

    public int GetPowerUpCount(PowerUpType type)
    {
        return powerUpInventory.ContainsKey(type) ? powerUpInventory[type] : 0;
    }

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

    public PowerUpType GetPowerUpTypeFromPrefab(GameObject prefab)
    {
        if (prefabToTypeMap.ContainsKey(prefab))
            return prefabToTypeMap[prefab];

        return PowerUpType.BlueLaser; // قيمة افتراضية
    }
}