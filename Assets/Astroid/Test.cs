using UnityEngine;

public class PowerUpTester : MonoBehaviour
{
    public PowerUpSystem powerUpSystem;

    void Update()
    {
        // «Œ »— »«·√“—«—
        if (Input.GetKeyDown(KeyCode.Alpha1))
            powerUpSystem.CollectPowerUp(PowerUpType.BlueLaser);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            powerUpSystem.CollectPowerUp(PowerUpType.SpeedBoost);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            powerUpSystem.CollectPowerUp(PowerUpType.Shield);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            powerUpSystem.CollectPowerUp(PowerUpType.MultiShot);
    }
}