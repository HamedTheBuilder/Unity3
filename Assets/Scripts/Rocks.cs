using UnityEngine;
using System.Collections;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Asteroid Prefabs")]
    public GameObject[] asteroidPrefabs;

    [Header("Spawn Settings")]
    public float spawnInterval = 0.5f;
    public float asteroidSpeed = 8f;
    public float spawnDistanceFromShip = 50f;
    public float destroyDistanceBehindShip = 10f;

    [Header("Asteroid Properties")]
    public float minSize = 20f; // حجم كبير جداً لتعويض صغر الـ Mesh
    public float maxSize = 30f;
    public int[] asteroidHealth = new int[] { 5, 10, 15 };

    [Header("Mesh Size Compensation")]
    public float[] meshSizeCompensation = new float[] { 1f, 1f, 1f }; // مضاعفات حجم لكل كويكب

    [Header("References")]
    public Transform spaceship;

    private Camera mainCamera;
    private bool isSpawning = true;

    private void Start()
    {
        mainCamera = Camera.main;

        if (spaceship == null)
        {
            spaceship = GameObject.FindGameObjectWithTag("Player").transform;
        }

        StartCoroutine(SpawnAsteroidsRoutine());
    }

    IEnumerator SpawnAsteroidsRoutine()
    {
        while (isSpawning)
        {
            SpawnAsteroid();
            yield return new WaitForSeconds(spawnInterval);
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
        int randomIndex = Random.Range(0, asteroidPrefabs.Length);
        GameObject selectedAsteroid = asteroidPrefabs[randomIndex];

        if (selectedAsteroid == null) return;

        GameObject asteroid = Instantiate(selectedAsteroid, spawnPosition, Random.rotation);

        SetupAsteroidSize(asteroid, randomIndex);
        SetupAsteroidHealth(asteroid, randomIndex);
        SetupAsteroidPhysics(asteroid);
        StartCoroutine(DestroyAsteroidAfterPassing(asteroid));
    }

    void SetupAsteroidSize(GameObject asteroid, int prefabIndex)
    {
        float randomSize = Random.Range(minSize, maxSize);

        // مضاعف حجم إضافي بناءً على نوع الكويكب
        float compensation = 1f;
        if (prefabIndex < meshSizeCompensation.Length)
        {
            compensation = meshSizeCompensation[prefabIndex];
        }
        else
        {
            // قيم افتراضية إذا لم يتم تعيين المضاعفات
            compensation = prefabIndex switch
            {
                0 => 25f, // كويكب صغير - مضاعف كبير
                1 => 20f, // كويكب متوسط
                2 => 15f, // كويكب كبير
                _ => 20f
            };
        }

        asteroid.transform.localScale = Vector3.one * randomSize * compensation;

        Debug.Log($"Asteroid {prefabIndex} size: {randomSize * compensation} (compensation: {compensation})");
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
        rb.linearVelocity = movementDirection * asteroidSpeed;
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

    public void StartSpawning() { isSpawning = true; StartCoroutine(SpawnAsteroidsRoutine()); }
    public void StopSpawning() { isSpawning = false; StopAllCoroutines(); }
    public void SetSpawnRate(float newRate) { spawnInterval = newRate; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isSpawning) StopSpawning(); else StartSpawning();
        }
    }
}