using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiFunctionGun : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float bulletSpeed = 30f;
    public float bulletLifetime = 2f;
    public KeyCode shootKey = KeyCode.Space; // زر واحد للطلق

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;

    [Header("Target Tags")]
    public string teleportTag = "TeleportTarget";
    public string swapTag = "SwapTarget";

    [Header("Teleport Settings")]
    public float teleportDuration = 0.5f;
    public AnimationCurve teleportCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Swap Settings")]
    public float swapDuration = 0.5f;
    public AnimationCurve swapCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Visual Effects")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem teleportEffect;
    public ParticleSystem swapEffect;
    public ParticleSystem arrivalEffect;

    [Header("Audio Effects")]
    public AudioClip shootSound;
    public AudioClip teleportSound;
    public AudioClip swapSound;

    [Header("Weapon Model")]
    public GameObject weaponModel;
    public Transform weaponHoldPoint;

    private AudioSource audioSource;
    private bool isActive = false;
    private GameObject currentWeapon;
    private GameObject currentBullet;
    private bool isFacingRight = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (weaponModel != null && weaponHoldPoint != null)
            currentWeapon = Instantiate(weaponModel, weaponHoldPoint.position, weaponHoldPoint.rotation, weaponHoldPoint);

        if (bulletSpawnPoint == null)
            bulletSpawnPoint = weaponHoldPoint;

        // تحديد الاتجاه الافتراضي
        UpdateWeaponDirection();

        Debug.Log("🔫 السلاح جاهز! استخدم Space للطلق");
    }

    void Update()
    {
        // تحديث اتجاه المسدس بناءً على حركة اللاعب
        UpdateWeaponDirection();

        if (!isActive && currentBullet == null && Input.GetKeyDown(shootKey))
        {
            Shoot();
        }
    }

    void UpdateWeaponDirection()
    {
        // تحديث اتجاه المسدس بناءً على حركة اللاعب
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (horizontal > 0 && !isFacingRight)
        {
            FlipWeapon(true);
        }
        else if (horizontal < 0 && isFacingRight)
        {
            FlipWeapon(false);
        }
    }

    void FlipWeapon(bool faceRight)
    {
        isFacingRight = faceRight;

        if (currentWeapon != null)
        {
            Vector3 scale = currentWeapon.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (faceRight ? 1 : -1);
            currentWeapon.transform.localScale = scale;
        }

        // تحديث موقع السباون بوينت إذا needed
        if (bulletSpawnPoint != null && currentWeapon != null)
        {
            // يمكنك تعديل موقع السباون حسب الاتجاه إذا needed
        }
    }

    void Shoot()
    {
        PlayMuzzleFlash();
        PlayShootSound();

        if (bulletPrefab == null || bulletSpawnPoint == null) return;

        currentBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);

        // تحديد اتجاه الطلقة بناءً على اتجاه المسدس
        Vector3 shootDirection = isFacingRight ? Vector3.right : Vector3.left;
        SetupBullet(currentBullet, shootDirection);
    }

    void SetupBullet(GameObject bullet, Vector3 direction)
    {
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb == null) rb = bullet.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearVelocity = direction.normalized * bulletSpeed;

        Collider col = bullet.GetComponent<Collider>();
        if (col == null)
        {
            SphereCollider s = bullet.AddComponent<SphereCollider>();
            s.isTrigger = true;
            s.radius = 0.2f;
        }
        else col.isTrigger = true;

        SimpleBulletController bulletController = bullet.GetComponent<SimpleBulletController>();
        if (bulletController == null)
            bulletController = bullet.AddComponent<SimpleBulletController>();

        bulletController.Initialize(this);

        bullet.tag = "Bullet";
        Destroy(bullet, bulletLifetime);
    }

    public void OnBulletHit(GameObject hitObject, Vector3 hitPoint)
    {
        if (hitObject == null)
        {
            currentBullet = null;
            return;
        }

        if (hitObject.CompareTag(teleportTag))
            StartCoroutine(TeleportToTarget(hitObject));
        else if (hitObject.CompareTag(swapTag))
            StartCoroutine(SwapWithTarget(hitObject));

        currentBullet = null;
    }

    IEnumerator TeleportToTarget(GameObject target)
    {
        if (isActive || target == null) yield break;
        isActive = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = target.transform.position + Vector3.up * 0.5f;

        PlayTeleportEffect(startPos);
        PlayTeleportSound();
        SetPlayerVisible(false);

        float t = 0;
        while (t < teleportDuration)
        {
            t += Time.deltaTime;
            float p = teleportCurve.Evaluate(t / teleportDuration);
            transform.position = Vector3.Lerp(startPos, targetPos, p);
            yield return null;
        }

        transform.position = targetPos;
        SetPlayerVisible(true);
        PlayArrivalEffect(transform.position);
        isActive = false;
    }

    IEnumerator SwapWithTarget(GameObject target)
    {
        if (isActive || target == null) yield break;
        isActive = true;

        Vector3 playerStart = transform.position;
        Vector3 targetStart = target.transform.position;

        Vector3 playerEnd = targetStart + Vector3.up * 0.5f;
        Vector3 targetEnd = playerStart;

        PlaySwapEffect(playerStart);
        PlaySwapEffect(targetStart);
        PlaySwapSound();

        SetPlayerVisible(false);
        SetTargetVisible(target, false);

        float t = 0;
        while (t < swapDuration)
        {
            t += Time.deltaTime;
            float p = swapCurve.Evaluate(t / swapDuration);
            transform.position = Vector3.Lerp(playerStart, playerEnd, p);
            target.transform.position = Vector3.Lerp(targetStart, targetEnd, p);
            yield return null;
        }

        transform.position = playerEnd;
        target.transform.position = targetEnd;

        SetTargetVisible(target, true);
        SetPlayerVisible(true);
        PlayArrivalEffect(transform.position);
        isActive = false;
    }

    void SetPlayerVisible(bool visible)
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = visible;

        if (currentWeapon != null)
            currentWeapon.SetActive(visible);
    }

    void SetTargetVisible(GameObject target, bool visible)
    {
        if (target == null) return;
        foreach (Renderer r in target.GetComponentsInChildren<Renderer>())
            r.enabled = visible;
    }

    void PlayMuzzleFlash()
    {
        if (muzzleFlash != null && bulletSpawnPoint != null)
        {
            var fx = Instantiate(muzzleFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            fx.Play();
            Destroy(fx.gameObject, fx.main.duration);
        }
    }

    void PlayTeleportEffect(Vector3 pos)
    {
        if (teleportEffect != null)
        {
            var fx = Instantiate(teleportEffect, pos, Quaternion.identity);
            fx.Play();
            Destroy(fx.gameObject, fx.main.duration);
        }
    }

    void PlaySwapEffect(Vector3 pos)
    {
        if (swapEffect != null)
        {
            var fx = Instantiate(swapEffect, pos, Quaternion.identity);
            fx.Play();
            Destroy(fx.gameObject, fx.main.duration);
        }
    }

    void PlayArrivalEffect(Vector3 pos)
    {
        if (arrivalEffect != null)
        {
            var fx = Instantiate(arrivalEffect, pos, Quaternion.identity);
            fx.Play();
            Destroy(fx.gameObject, fx.main.duration);
        }
    }

    void PlayShootSound() { if (shootSound) audioSource.PlayOneShot(shootSound); }
    void PlayTeleportSound() { if (teleportSound) audioSource.PlayOneShot(teleportSound); }
    void PlaySwapSound() { if (swapSound) audioSource.PlayOneShot(swapSound); }

    // دالة مساعدة لمعرفة الاتجاه الحالي
    public bool IsFacingRight()
    {
        return isFacingRight;
    }
}

// سكربت البوليت المحدث
public class SimpleBulletController : MonoBehaviour
{
    private MultiFunctionGun gun;
    private bool hasHit = false;
    private Rigidbody rb;

    public void Initialize(MultiFunctionGun g)
    {
        gun = g;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // توجيه البوليت في اتجاه حركتها
        if (rb != null && rb.linearVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        if (other == null) return;

        // تجاهل الاصطدام باللاعب نفسه
        if (other.transform == gun.transform) return;
        if (other.CompareTag("Player")) return;

        hasHit = true;
        gun?.OnBulletHit(other.gameObject, transform.position);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Collider c = GetComponent<Collider>();
        if (c != null) c.enabled = false;

        Renderer r = GetComponent<Renderer>();
        if (r != null) r.enabled = false;

        Destroy(gameObject, 0.3f);
    }
}