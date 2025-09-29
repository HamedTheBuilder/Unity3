using UnityEngine;
using System.Collections;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject[] asteroidPrefabs; // مصفوفة من الكويكبات المختلفة
    public float spawnInterval = 0.5f; // الوقت بين كل ظهور لكويكب جديد
    public float asteroidSpeed = 8f; // سرعة الكويكبات
    public float spawnDistanceFromShip = 50f; // المسافة أمام السفينة حيث تظهر الكويكبات
    public Transform spaceship; // مرجع إلى السفينة لتحديد موقعها
    public float minSize = 2f; // الحد الأدنى لحجم الكويكب
    public float maxSize = 3f; // الحد الأقصى لحجم الكويكب

    private Camera mainCamera; // الكاميرا الرئيسية

    private void Start()
    {
        // الحصول على الكاميرا الرئيسية
        mainCamera = Camera.main;

        // بدء عملية ظهور الكويكبات بشكل مستمر
        InvokeRepeating("SpawnAsteroid", 0f, spawnInterval); // تم تقليل الوقت بين الكويكبات لجعلها أكثر
    }

    void SpawnAsteroid()
    {
        // تحديد مكان الظهور عشوائيًا على حواف الكاميرا
        Vector3 spawnPosition = GetRandomEdgePosition();

        // اختيار كويكب عشوائي من المصفوفة
        int randomIndex = Random.Range(0, asteroidPrefabs.Length);
        GameObject selectedAsteroid = asteroidPrefabs[randomIndex];

        // إنشاء كويكب جديد في المكان المحدد
        GameObject asteroid = Instantiate(selectedAsteroid, spawnPosition, Quaternion.identity);

        // تعيين الحجم الأصلي للكويكب (حجمه الأصلي هو الحجم الحالي للكويكب في الـ Prefab)
        float initialSize = Mathf.Max(minSize, asteroid.transform.localScale.x); // تأكد من أن الحجم لا يكون أصغر من الحجم الأدنى

        // تعيين الحجم الأولي (الحجم الأصلي لا يتغير)
        asteroid.transform.localScale = new Vector3(initialSize, initialSize, initialSize);

        // إضافة Rigidbody إذا لم يكن موجودًا
        Rigidbody rb = asteroid.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = asteroid.AddComponent<Rigidbody>(); // إضافة Rigidbody إذا لم يكن موجودًا
        }

        // إيقاف تأثير الجاذبية على الكويكب
        rb.useGravity = false;

        // تحديد ما إذا كان الكويكب سيتحرك نحو السفينة أو بشكل عشوائي (30% نحو السفينة، 70% عشوائي)
        bool moveTowardsShip = Random.value < 0.3f; // 30% احتمال أن يتحرك الكويكب نحو السفينة

        if (moveTowardsShip)
        {
            // حركة مباشرة نحو السفينة
            Vector3 directionToShip = (spaceship.position - asteroid.transform.position).normalized;
            rb.linearVelocity = directionToShip * asteroidSpeed;
        }
        else
        {
            // حركة عشوائية
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1).normalized;
            rb.linearVelocity = randomDirection * asteroidSpeed;
        }

        // إضافة Coroutine لتغيير الحجم تدريجيًا أثناء اقتراب الكويكب
        StartCoroutine(GrowAsteroid(asteroid, initialSize));

        // إضافة سلوك لاختفاء الكويكب بعد تجاوزه للسفينة
        StartCoroutine(DestroyAsteroidAfterPassing(asteroid));
    }

    // دالة لاختيار مكان عشوائي على الحواف (يمين، يسار، فوق، أسفل، أو الزوايا)
    Vector3 GetRandomEdgePosition()
    {
        // حساب عرض وارتفاع الكاميرا في العالم (world space)
        float cameraWidth = GetCameraWidth();
        float cameraHeight = GetCameraHeight();

        // اختيار عشوائي للجوانب (يمين، يسار، فوق، أسفل، زوايا)
        float spawnX = 0f;
        float spawnY = 0f;

        // تحديد إذا كانت الكويكب سيظهر من أحد الجوانب
        int edgeSelection = Random.Range(0, 4); // 0 = يسار, 1 = يمين, 2 = أعلى, 3 = أسفل

        switch (edgeSelection)
        {
            case 0: // من اليسار
                spawnX = -cameraWidth / 2;
                spawnY = Random.Range(-cameraHeight / 2, cameraHeight / 2);
                break;
            case 1: // من اليمين
                spawnX = cameraWidth / 2;
                spawnY = Random.Range(-cameraHeight / 2, cameraHeight / 2);
                break;
            case 2: // من الأعلى
                spawnX = Random.Range(-cameraWidth / 2, cameraWidth / 2);
                spawnY = cameraHeight / 2;
                break;
            case 3: // من الأسفل
                spawnX = Random.Range(-cameraWidth / 2, cameraWidth / 2);
                spawnY = -cameraHeight / 2;
                break;
        }

        // تحديد مكان الظهور أمام السفينة بناءً على المسافة المحددة
        return new Vector3(spawnX, spawnY, spaceship.position.z + spawnDistanceFromShip);
    }

    // الحصول على عرض الكاميرا في العالم (world space)
    float GetCameraWidth()
    {
        float aspectRatio = mainCamera.aspect; // نسبة العرض إلى الارتفاع
        float cameraHeight = 2f * mainCamera.orthographicSize; // ارتفاع الكاميرا
        return cameraHeight * aspectRatio; // العرض يتم حسابه باستخدام النسبة والارتفاع
    }

    // الحصول على ارتفاع الكاميرا في العالم (world space)
    float GetCameraHeight()
    {
        return 2f * mainCamera.orthographicSize; // ارتفاع الكاميرا في world space
    }

    // Coroutine لتغيير الحجم تدريجيًا أثناء اقتراب الكويكب من السفينة
    IEnumerator GrowAsteroid(GameObject asteroid, float initialSize)
    {
        float initialDistance = Vector3.Distance(asteroid.transform.position, spaceship.position); // المسافة الأولية
        while (asteroid != null)
        {
            // حساب المسافة الحالية بين الكويكب والسفينة
            float currentDistance = Vector3.Distance(asteroid.transform.position, spaceship.position);

            // حساب النسبة بين المسافة الحالية والمسافة الأولية
            float sizeFactor = Mathf.Lerp(initialSize, maxSize, 1 - (currentDistance / initialDistance));

            // تغيير الحجم تدريجيًا (لا يسمح أن يصبح أصغر من الحجم الأصلي)
            asteroid.transform.localScale = new Vector3(Mathf.Max(initialSize, sizeFactor), Mathf.Max(initialSize, sizeFactor), Mathf.Max(initialSize, sizeFactor));

            yield return null; // الانتظار للإطار التالي
        }
    }

    // Coroutine لإخفاء الكويكب بعد تجاوزه للسفينة
    IEnumerator DestroyAsteroidAfterPassing(GameObject asteroid)
    {
        while (asteroid != null)
        {
            // إذا الكويكب تجاوز السفينة، نقوم بحذفه
            if (asteroid.transform.position.z < spaceship.position.z)
            {
                Destroy(asteroid);
                break;
            }
            yield return null; // الانتظار للإطار التالي
        }
    }
}
