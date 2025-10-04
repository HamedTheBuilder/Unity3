using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpType powerUpType;
    public float rotationSpeed = 50f;

    void Start()
    {
        AddLightOnly();
    }

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PowerUpSystem powerSystem = other.GetComponent<PowerUpSystem>();
            PowerUpManager powerManager = other.GetComponent<PowerUpManager>();

            if (powerSystem != null)
            {
                // استخدام النظام الجديد - تفعيل فوري
                powerSystem.CollectPowerUp(powerUpType);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning($"❌ لم يتم العثور على PowerUpSystem على اللاعب");
            }
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