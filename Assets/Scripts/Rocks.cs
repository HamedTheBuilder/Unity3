using UnityEngine;
using System.Collections;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Asteroid Prefabs")]
    public GameObject[] asteroidPrefabs;

    [Header("Spawn Settings")]
    public float startSpawnInterval = 1.0f;
    public float minSpawnInterval = 0.2f;
    public float asteroidSpeed = 8f;
    public float spawnDistanceFromShip = 20f;
    public float destroyDistanceBehindShip = 10f;

    [Header("Difficulty Progression")]
    public float timeToMaxDifficulty = 45f;
    public float maxAsteroidSpeed = 15f;
    public float difficultyIncreaseRate = 0.1f;

    [Header("Asteroid Count Progression")]
    public int startAsteroidCount = 1;
    public int maxAsteroidCount = 5;
    public float timeToMaxAsteroidCount = 60f;

    [Header("Asteroid Properties")]
    public float minSize = 20f;
    public float maxSize = 30f;
    public int[] asteroidHealth = new int[] { 5, 10, 15 };

    [Header("Mesh Size Compensation")]
    public float[] meshSizeCompensation = new float[] { 1f, 1f, 1f };

    [Header("Asteroid Movement Settings - NEW")]
    [Range(0f, 1f)]
    public float chanceToMoveTowardsPlayer = 0.5f; // 50% فرصة للتحرك تجاه اللاعب

    [Header("Power Up Settings")]
    public GameObject blueLaserPowerUpPrefab;
    public GameObject speedBoostPowerUpPrefab;
    public GameObject shieldPowerUpPrefab;
    public GameObject multiShotPowerUpPrefab;

    [Header("Power Up Spawn Chance")]
    public float powerUpSpawnChance = 0.2f;
    public float powerUpSpeed = 3f;

    [Header("References")]
    public Transform spaceship;
    public PowerUpManager powerUpManager;

    private Camera mainCamera;
    private bool isSpawning = true;
    private float currentSpawnInterval;
    private float currentAsteroidSpeed;
    private int currentAsteroidCount;
    private float gameTime = 0f;
    private float difficultyTimer = 0f;

    private float screenLeft, screenRight, screenTop, screenBottom;
    private float spawnZPosition;

    private GameObject[] powerUpPrefabsArray;

    private void Start()
    {
        mainCamera = Camera.main;

        if (spaceship == null)
        {
            spaceship = GameObject.FindGameObjectWithTag("Player").transform;
        }

        InitializePowerUpArray();
        CalculateScreenBounds();

        currentSpawnInterval = startSpawnInterval;
        currentAsteroidSpeed = asteroidSpeed;
        currentAsteroidCount = startAsteroidCount;
        spawnZPosition = spaceship.position.z + spawnDistanceFromShip;

        StartCoroutine(SpawnAsteroidsRoutine());
    }

    void InitializePowerUpArray()
    {
        powerUpPrefabsArray = new GameObject[]
        {
            blueLaserPowerUpPrefab,
            speedBoostPowerUpPrefab,
            shieldPowerUpPrefab,
            multiShotPowerUpPrefab
        };
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        difficultyTimer += Time.deltaTime;

        CalculateScreenBounds();

        if (difficultyTimer >= 1.0f)
        {
            IncreaseDifficulty();
            difficultyTimer = 0f;
        }
    }

    void CalculateScreenBounds()
    {
        if (mainCamera == null) return;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, Mathf.Abs(mainCamera.transform.position.z - spaceship.position.z) + spawnDistanceFromShip));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, Mathf.Abs(mainCamera.transform.position.z - spaceship.position.z) + spawnDistanceFromShip));

        screenLeft = bottomLeft.x;
        screenRight = topRight.x;
        screenTop = topRight.y;
        screenBottom = bottomLeft.y;
    }

    void IncreaseDifficulty()
    {
        if (currentAsteroidSpeed < maxAsteroidSpeed)
        {
            currentAsteroidSpeed += (maxAsteroidSpeed - asteroidSpeed) / timeToMaxDifficulty;
            currentAsteroidSpeed = Mathf.Min(currentAsteroidSpeed, maxAsteroidSpeed);
        }

        if (currentSpawnInterval > minSpawnInterval)
        {
            float intervalReduction = (startSpawnInterval - minSpawnInterval) / timeToMaxDifficulty;
            currentSpawnInterval -= intervalReduction;
            currentSpawnInterval = Mathf.Max(currentSpawnInterval, minSpawnInterval);
        }

        if (currentAsteroidCount < maxAsteroidCount)
        {
            float countIncrease = (float)(maxAsteroidCount - startAsteroidCount) / timeToMaxAsteroidCount;
            currentAsteroidCount = startAsteroidCount + Mathf.FloorToInt(gameTime * countIncrease);
            currentAsteroidCount = Mathf.Min(currentAsteroidCount, maxAsteroidCount);
        }

        if (gameTime > 30f && Random.value < 0.3f)
        {
            SpawnExtraAsteroid();
        }

        Debug.Log($"⏰ الوقت: {gameTime:F0}ث | 🚀 السرعة: {currentAsteroidSpeed:F1} | ⏱️ السباون: {currentSpawnInterval:F2} | 🔥 العدد: {currentAsteroidCount}");
    }

    IEnumerator SpawnAsteroidsRoutine()
    {
        while (isSpawning)
        {
            SpawnAsteroidGroup();

            if (Random.value < powerUpSpawnChance && powerUpPrefabsArray != null && powerUpPrefabsArray.Length > 0)
            {
                SpawnRandomPowerUp();
            }

            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    void SpawnAsteroidGroup()
    {
        if (asteroidPrefabs == null || asteroidPrefabs.Length == 0) return;

        for (int i = 0; i < currentAsteroidCount; i++)
        {
            StartCoroutine(SpawnSingleAsteroidWithDelay(i * 0.1f));
        }
    }

    IEnumerator SpawnSingleAsteroidWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnSingleAsteroid();
    }

    void SpawnSingleAsteroid()
    {
        Vector3 spawnPosition = GetRandomPositionWithinScreen();
        int randomIndex = GetWeightedAsteroidIndex();
        GameObject selectedAsteroid = asteroidPrefabs[randomIndex];

        if (selectedAsteroid == null) return;

        GameObject asteroid = Instantiate(selectedAsteroid, spawnPosition, Random.rotation);

        SetupAsteroidSize(asteroid, randomIndex);
        SetupAsteroidHealth(asteroid, randomIndex);

        // تحديد اتجاه الحركة - NEW
        bool moveTowardsPlayer = Random.value < chanceToMoveTowardsPlayer;
        SetupAsteroidPhysics(asteroid, moveTowardsPlayer);

        StartCoroutine(DestroyAsteroidAfterPassing(asteroid));

        Debug.Log($"🌌 كويكب: {(moveTowardsPlayer ? "باتجاهك 🎯" : "عشوائي 🔀")}");
    }

    Vector3 GetRandomPositionWithinScreen()
    {
        float randomX = Random.Range(screenLeft + 1f, screenRight - 1f);
        float randomY = Random.Range(screenBottom + 1f, screenTop - 1f);
        return new Vector3(randomX, randomY, spawnZPosition);
    }

    void SpawnExtraAsteroid()
    {
        if (asteroidPrefabs == null || asteroidPrefabs.Length == 0) return;

        Vector3 spawnPosition = GetRandomPositionWithinScreen();
        int randomIndex = Random.Range(0, asteroidPrefabs.Length);
        GameObject selectedAsteroid = asteroidPrefabs[randomIndex];

        if (selectedAsteroid == null) return;

        GameObject asteroid = Instantiate(selectedAsteroid, spawnPosition, Random.rotation);

        SetupAsteroidSize(asteroid, randomIndex);
        SetupAsteroidHealth(asteroid, randomIndex);

        // تحديد اتجاه الحركة للكويكب الإضافي - NEW
        bool moveTowardsPlayer = Random.value < chanceToMoveTowardsPlayer;
        SetupAsteroidPhysics(asteroid, moveTowardsPlayer);

        StartCoroutine(DestroyAsteroidAfterPassing(asteroid));
    }

    void SpawnRandomPowerUp()
    {
        if (powerUpPrefabsArray == null || powerUpPrefabsArray.Length == 0) return;

        Vector3 spawnPosition = GetRandomPositionWithinScreen();
        int randomIndex = Random.Range(0, powerUpPrefabsArray.Length);
        GameObject selectedPowerUp = powerUpPrefabsArray[randomIndex];

        if (selectedPowerUp == null) return;

        GameObject powerUp = Instantiate(selectedPowerUp, spawnPosition, Quaternion.identity);

        PowerUp powerUpScript = powerUp.GetComponent<PowerUp>();
        if (powerUpScript == null)
        {
            powerUpScript = powerUp.AddComponent<PowerUp>();
        }

        PowerUpType powerUpType = GetPowerUpTypeFromPrefab(selectedPowerUp);
        powerUpScript.powerUpType = powerUpType;

        SetupPowerUpPhysics(powerUp);
        StartCoroutine(DestroyPowerUpAfterPassing(powerUp));

        Debug.Log($"🎁 تم إنشاء قدرة: {GetPowerUpName(powerUpType)}");
    }

    PowerUpType GetPowerUpTypeFromPrefab(GameObject prefab)
    {
        if (prefab == blueLaserPowerUpPrefab) return PowerUpType.BlueLaser;
        else if (prefab == speedBoostPowerUpPrefab) return PowerUpType.SpeedBoost;
        else if (prefab == shieldPowerUpPrefab) return PowerUpType.Shield;
        else if (prefab == multiShotPowerUpPrefab) return PowerUpType.MultiShot;
        else return PowerUpType.BlueLaser;
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

    int GetWeightedAsteroidIndex()
    {
        float randomValue = Random.value;
        if (randomValue < 0.5f) return 0;
        else if (randomValue < 0.8f) return 1;
        else return 2;
    }

    void SetupPowerUpPhysics(GameObject powerUp)
    {
        Rigidbody rb = powerUp.GetComponent<Rigidbody>();
        if (rb == null) rb = powerUp.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.linearDamping = 0;

        Vector3 directionToPlayer = (spaceship.position - powerUp.transform.position).normalized;
        rb.linearVelocity = directionToPlayer * powerUpSpeed;
        rb.angularVelocity = new Vector3(0, 2f, 0);
    }

    void SetupAsteroidSize(GameObject asteroid, int prefabIndex)
    {
        float randomSize = Random.Range(minSize, maxSize);
        float compensation = 1f;

        if (prefabIndex < meshSizeCompensation.Length)
        {
            compensation = meshSizeCompensation[prefabIndex];
        }
        else
        {
            compensation = prefabIndex switch
            {
                0 => 25f,
                1 => 20f,
                2 => 15f,
                _ => 20f
            };
        }

        asteroid.transform.localScale = Vector3.one * randomSize * compensation;
    }

    void SetupAsteroidHealth(GameObject asteroid, int prefabIndex)
    {
        AsteroidHealth health = asteroid.GetComponent<AsteroidHealth>();
        if (health == null) health = asteroid.AddComponent<AsteroidHealth>();

        if (prefabIndex < asteroidHealth.Length)
        {
            health.maxHealth = asteroidHealth[prefabIndex];
            health.currentHealth = asteroidHealth[prefabIndex];
        }

        asteroid.tag = "Asteroid";
        if (asteroid.GetComponent<Collider>() == null) asteroid.AddComponent<BoxCollider>();
    }

    // دالة معدلة لفيزياء الكويكبات - NEW
    void SetupAsteroidPhysics(GameObject asteroid, bool moveTowardsPlayer)
    {
        Rigidbody rb = asteroid.GetComponent<Rigidbody>();
        if (rb == null) rb = asteroid.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.linearDamping = 0;
        rb.angularDamping = 0.5f;

        rb.angularVelocity = new Vector3(
            Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 2f;

        Vector3 movementDirection;

        if (moveTowardsPlayer)
        {
            // 50%: تتحرك مباشرة تجاه اللاعب
            movementDirection = (spaceship.position - asteroid.transform.position).normalized;
        }
        else
        {
            // 50%: تتحرك في اتجاهات عشوائية
            movementDirection = new Vector3(
                Random.Range(-0.8f, 0.8f),
                Random.Range(-0.8f, 0.8f),
                -1f
            ).normalized;
        }

        rb.linearVelocity = movementDirection * currentAsteroidSpeed;
    }

    IEnumerator DestroyAsteroidAfterPassing(GameObject asteroid)
    {
        while (asteroid != null)
        {
            if (asteroid.transform.position.z < spaceship.position.z - destroyDistanceBehindShip)
            {
                Destroy(asteroid);
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator DestroyPowerUpAfterPassing(GameObject powerUp)
    {
        while (powerUp != null)
        {
            if (powerUp.transform.position.z < spaceship.position.z - destroyDistanceBehindShip)
            {
                Destroy(powerUp);
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void StartSpawning() { isSpawning = true; StartCoroutine(SpawnAsteroidsRoutine()); }
    public void StopSpawning() { isSpawning = false; StopAllCoroutines(); }
    public void SetSpawnRate(float newRate) { currentSpawnInterval = newRate; }
    public void ResetDifficulty()
    {
        currentSpawnInterval = startSpawnInterval;
        currentAsteroidSpeed = asteroidSpeed;
        currentAsteroidCount = startAsteroidCount;
        gameTime = 0f;
    }
}