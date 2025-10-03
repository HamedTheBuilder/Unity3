using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpType powerUpType;
    public float rotationSpeed = 50f;

    void Start()
    {
        AddLightOnly();

        // تشخيص: طباعة معلومات القدرة
        Debug.Log($"🔧 تم إنشاء قدرة: {powerUpType} | التاج: {gameObject.tag} | الكوليدر: {GetComponent<Collider>() != null}");
    }

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"🎯 التصادم: {other.name} | التاج: {other.tag}");

        if (other.CompareTag("Player"))
        {
            Debug.Log($"✅ تم اكتشاف اللاعب: {other.name}");

            SpaceshipMovement shipMovement = other.GetComponent<SpaceshipMovement>();
            PowerUpSystem powerSystem = other.GetComponent<PowerUpSystem>();

            if (shipMovement != null && powerSystem != null)
            {
                Debug.Log($"🎁 تطبيق قدرة: {powerUpType}");
                ApplyPowerUp(other.gameObject);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning($"❌ لم يتم العثور على المكونات المطلوبة على اللاعب");
            }
        }
    }

    void ApplyPowerUp(GameObject player)
    {
        PowerUpSystem powerUpSystem = player.GetComponent<PowerUpSystem>();
        if (powerUpSystem != null)
        {
            powerUpSystem.CollectPowerUp(powerUpType);
            Debug.Log($"✅ تم جمع قدرة: {powerUpType}");
        }
        else
        {
            Debug.LogError($"❌ لم يتم العثور على PowerUpSystem على اللاعب");
        }
    }

    void AddLightOnly()
    {
        Light glowLight = gameObject.AddComponent<Light>();
        glowLight.type = LightType.Point;
        glowLight.range = 3f;
        glowLight.intensity = 2f;
        glowLight.color = GetPowerUpColor();
    }

    Color GetPowerUpColor()
    {
        return powerUpType switch
        {
            PowerUpType.BlueLaser => Color.blue,
            PowerUpType.SpeedBoost => Color.yellow,
            PowerUpType.Shield => Color.red,
            PowerUpType.MultiShot => new Color(0.8f, 0.2f, 0.8f),
            _ => Color.white
        };
    }
}