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
    public float spawnDistanceFromShip = 50f;
    public float destroyDistanceBehindShip = 10f;

    [Header("Difficulty Progression")]
    public float timeToMaxDifficulty = 45f;
    public float maxAsteroidSpeed = 15f;

    [Header("Spawn Rate Increase")]
    public float spawnRateMultiplier = 1.5f; // معدل زيادة الريسباون

    [Header("Asteroid Properties")]
    public float minSize = 20f;
    public float maxSize = 30f;
    public int[] asteroidHealth = new int[] { 5, 10, 15 };

    [Header("Mesh Size Compensation")]
    public float[] meshSizeCompensation = new float[] { 1f, 1f, 1f };

    [Header("Power Up Settings")]
    public GameObject[] powerUpPrefabs;
    public float powerUpSpawnChance = 0.2f;
    public float powerUpSpeed = 5f; // ثابتة لا تتغير

    [Header("References")]
    public Transform spaceship;

    private Camera mainCamera;
    private bool isSpawning = true;
    private float currentSpawnInterval;
    private float currentAsteroidSpeed;
    private float gameTime = 0f;
    private float difficultyTimer = 0f;

    private void Start()
    {
        mainCamera = Camera.main;

        if (spaceship == null)
        {
            spaceship = GameObject.FindGameObjectWithTag("Player").transform;
        }

        currentSpawnInterval = startSpawnInterval;
        currentAsteroidSpeed = asteroidSpeed;

        StartCoroutine(SpawnAsteroidsRoutine());
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        difficultyTimer += Time.deltaTime;

        if (difficultyTimer >= 1.0f)
        {
            IncreaseDifficulty();
            difficultyTimer = 0f;
        }
    }

    void IncreaseDifficulty()
    {
        // زيادة سرعة الكويكبات فقط
        if (currentAsteroidSpeed < maxAsteroidSpeed)
        {
            currentAsteroidSpeed += (maxAsteroidSpeed - asteroidSpeed) / timeToMaxDifficulty;
            currentAsteroidSpeed = Mathf.Min(currentAsteroidSpeed, maxAsteroidSpeed);
        }

        // تقليل وقت السباون (زيادة الريسباون) بناءً على السرعة
        UpdateSpawnRateBasedOnSpeed();

        // زيادة فرصة الكويكبات الكبيرة بعد وقت معين
        if (gameTime > 30f)
        {
            if (Random.value < 0.3f)
            {
                SpawnExtraAsteroid();
            }
        }

        Debug.Log($"⏰ الوقت: {gameTime:F0}ث | 🚀 السرعة: {currentAsteroidSpeed:F1} | ⏱️ السباون: {currentSpawnInterval:F2}");
    }

    void UpdateSpawnRateBasedOnSpeed()
    {
        // حساب نسبة السرعة الحالية من السرعة القصوى
        float speedRatio = (currentAsteroidSpeed - asteroidSpeed) / (maxAsteroidSpeed - asteroidSpeed);

        // تطبيق المضاعف على معدل السباون
        float targetSpawnInterval = startSpawnInterval - (startSpawnInterval - minSpawnInterval) * speedRatio * spawnRateMultiplier;

        currentSpawnInterval = Mathf.Max(targetSpawnInterval, minSpawnInterval);
    }

    IEnumerator SpawnAsteroidsRoutine()
    {
        while (isSpawning)
        {
            SpawnAsteroid();

            // القدرات تظهر بنفس المعدل دائماً
            if (Random.value < powerUpSpawnChance && powerUpPrefabs != null && powerUpPrefabs.Length > 0)
            {
                SpawnRandomPowerUp();
            }

            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    void SpawnAsteroid()
    {
        if (asteroidPrefabs == null || asteroidPrefabs.Length == 0)
        {
            Debug.LogWarning("No asteroid prefabs assigned!");
            return;
        }

        Vector3 spawnPosition = GetRandomEdgePosition();
        int randomIndex = GetWeightedAsteroidIndex();
        GameObject selectedAsteroid = asteroidPrefabs[randomIndex];

        if (selectedAsteroid == null) return;

        GameObject asteroid = Instantiate(selectedAsteroid, spawnPosition, Random.rotation);

        SetupAsteroidSize(asteroid, randomIndex);
        SetupAsteroidHealth(asteroid, randomIndex);
        SetupAsteroidPhysics(asteroid);
        StartCoroutine(DestroyAsteroidAfterPassing(asteroid));
    }

    void SpawnExtraAsteroid()
    {
        if (asteroidPrefabs == null || asteroidPrefabs.Length == 0) return;

        Vector3 spawnPosition = GetRandomEdgePosition();
        int randomIndex = Random.Range(0, asteroidPrefabs.Length);
        GameObject selectedAsteroid = asteroidPrefabs[randomIndex];

        if (selectedAsteroid == null) return;

        GameObject asteroid = Instantiate(selectedAsteroid, spawnPosition, Random.rotation);

        SetupAsteroidSize(asteroid, randomIndex);
        SetupAsteroidHealth(asteroid, randomIndex);
        SetupAsteroidPhysics(asteroid);
        StartCoroutine(DestroyAsteroidAfterPassing(asteroid));
    }

    void SpawnRandomPowerUp()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0)
        {
            Debug.LogWarning("No power up prefabs assigned!");
            return;
        }

        Vector3 spawnPosition = GetRandomEdgePosition();
        int randomIndex = Random.Range(0, powerUpPrefabs.Length);
        GameObject selectedPowerUp = powerUpPrefabs[randomIndex];

        if (selectedPowerUp == null) return;

        GameObject powerUp = Instantiate(selectedPowerUp, spawnPosition, Quaternion.identity);

        PowerUp powerUpScript = powerUp.GetComponent<PowerUp>();
        if (powerUpScript == null)
        {
            powerUpScript = powerUp.AddComponent<PowerUp>();
        }

        // القدرات تتحرك بسرعة ثابتة دائماً
        SetupPowerUpPhysics(powerUp);
        StartCoroutine(DestroyPowerUpAfterPassing(powerUp));

        Debug.Log($"🎁 تم إنشاء قدرة: {selectedPowerUp.name}");
    }

    int GetWeightedAsteroidIndex()
    {
        float randomValue = Random.value;

        if (randomValue < 0.5f)
            return 0;
        else if (randomValue < 0.8f)
            return 1;
        else
            return 2;
    }

    void SetupPowerUpPhysics(GameObject powerUp)
    {
        Rigidbody rb = powerUp.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = powerUp.AddComponent<Rigidbody>();
        }

        rb.useGravity = false;
        rb.linearDamping = 0;

        Vector3 directionToPlayer = (spaceship.position - powerUp.transform.position).normalized;
        Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
        Vector3 finalDirection = (directionToPlayer + randomOffset).normalized;

        // سرعة ثابتة للقدرات
        rb.linearVelocity = finalDirection * powerUpSpeed;
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
        if (health == null)
        {
            health = asteroid.AddComponent<AsteroidHealth>();
        }

        if (prefabIndex < asteroidHealth.Length)
        {
            health.maxHealth = asteroidHealth[prefabIndex];
            health.currentHealth = asteroidHealth[prefabIndex];
        }

        asteroid.tag = "Asteroid";

        if (asteroid.GetComponent<Collider>() == null)
        {
            asteroid.AddComponent<BoxCollider>();
        }
    }

    void SetupAsteroidPhysics(GameObject asteroid)
    {
        Rigidbody rb = asteroid.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = asteroid.AddComponent<Rigidbody>();
        }

        rb.useGravity = false;
        rb.linearDamping = 0;
        rb.angularDamping = 0.5f;

        rb.angularVelocity = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ) * 2f;

        Vector3 movementDirection = GetMovementDirection();
        // الكويكبات تستخدم السرعة المتزايدة
        rb.linearVelocity = movementDirection * currentAsteroidSpeed;
    }

    Vector3 GetRandomEdgePosition()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        float cameraWidth = GetCameraWidth();
        float cameraHeight = GetCameraHeight();

        Vector3 spawnPosition = Vector3.zero;
        int edgeSelection = Random.Range(0, 4);

        switch (edgeSelection)
        {
            case 0: spawnPosition = new Vector3(-cameraWidth / 2, Random.Range(-cameraHeight / 2, cameraHeight / 2), 0); break;
            case 1: spawnPosition = new Vector3(cameraWidth / 2, Random.Range(-cameraHeight / 2, cameraHeight / 2), 0); break;
            case 2: spawnPosition = new Vector3(Random.Range(-cameraWidth / 2, cameraWidth / 2), cameraHeight / 2, 0); break;
            case 3: spawnPosition = new Vector3(Random.Range(-cameraWidth / 2, cameraWidth / 2), -cameraHeight / 2, 0); break;
        }

        Vector3 worldSpawnPosition = mainCamera.transform.TransformPoint(spawnPosition);
        worldSpawnPosition.z = spaceship.position.z + spawnDistanceFromShip;
        return worldSpawnPosition;
    }

    Vector3 GetMovementDirection()
    {
        bool moveTowardsShip = Random.value < 0.3f;

        if (moveTowardsShip)
        {
            Vector3 directionToShip = (spaceship.position - transform.position).normalized;
            return new Vector3(directionToShip.x, directionToShip.y, -1).normalized;
        }
        else
        {
            return new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.8f, -0.2f), -1).normalized;
        }
    }

    float GetCameraWidth()
    {
        if (mainCamera.orthographic)
        {
            return 2f * mainCamera.orthographicSize * mainCamera.aspect;
        }
        else
        {
            float distance = Mathf.Abs(mainCamera.transform.position.z - spaceship.position.z);
            return 2.0f * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * distance * mainCamera.aspect;
        }
    }

    float GetCameraHeight()
    {
        if (mainCamera.orthographic)
        {
            return 2f * mainCamera.orthographicSize;
        }
        else
        {
            float distance = Mathf.Abs(mainCamera.transform.position.z - spaceship.position.z);
            return 2.0f * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * distance;
        }
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
        gameTime = 0f;
    }
}