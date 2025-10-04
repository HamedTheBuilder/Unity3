using UnityEngine;

public class SimpleLaserGun : MonoBehaviour
{
    [Header("Laser Settings")]
    public GameObject laserPrefab; // «·»—Ì›«» «·Õ«·Ì (Ì„ﬂ‰  €ÌÌ—Â œÌ‰«„ÌﬂÌ«)
    public Transform[] firePoints;
    public float laserSpeed = 20f;
    public float fireRate = 0.2f;
    public int damagePerShot = 1;

    [Header("Sound Settings")]
    public AudioClip laserSound;
    public float soundVolume = 0.2f;
    public float soundInterval = 0.5f;

    [Header("Firing Mode")]
    public FiringMode firingMode = FiringMode.Alternate;

    public enum FiringMode
    {
        Alternate,
        Simultaneous,
        FirstOnly
    }

    [System.NonSerialized] public float nextFireTime = 0f;

    private float nextSoundTime = 0f;
    private int currentFirePointIndex = 0;
    private AudioSource audioSource;

    void Start()
    {
        if (firePoints == null || firePoints.Length == 0)
        {
            firePoints = new Transform[1] { transform };
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = soundVolume;
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            ShootLasers();
            nextFireTime = Time.time + fireRate;
        }
    }

    void ShootLasers()
    {
        if (laserPrefab == null || firePoints.Length == 0) return;

        switch (firingMode)
        {
            case FiringMode.Simultaneous:
                foreach (Transform firePoint in firePoints)
                {
                    if (firePoint != null)
                        CreateLaser(firePoint);
                }
                break;

            case FiringMode.Alternate:
                Transform currentFirePoint = firePoints[currentFirePointIndex];
                if (currentFirePoint != null)
                    CreateLaser(currentFirePoint);
                currentFirePointIndex = (currentFirePointIndex + 1) % firePoints.Length;
                break;

            case FiringMode.FirstOnly:
                if (firePoints[0] != null)
                    CreateLaser(firePoints[0]);
                break;
        }

        PlayLaserSound();
    }

    void CreateLaser(Transform firePoint)
    {
        GameObject laser = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);

        Rigidbody laserRb = laser.GetComponent<Rigidbody>();
        if (laserRb != null)
        {
            laserRb.linearVelocity = -firePoint.forward * laserSpeed;
        }

        LaserProjectile laserProjectile = laser.GetComponent<LaserProjectile>();
        if (laserProjectile != null)
        {
            laserProjectile.damage = damagePerShot;
        }

        Destroy(laser, 3f);
    }

    public void PlayLaserSound()
    {
        if (laserSound != null && Time.time >= nextSoundTime)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(laserSound, soundVolume);
            }
            else
            {
                AudioSource.PlayClipAtPoint(laserSound, transform.position, soundVolume);
            }

            nextSoundTime = Time.time + soundInterval;
        }
    }

    // œ«·… „”«⁄œ… · €ÌÌ— »—Ì›«» «··Ì“— œÌ‰«„ÌﬂÌ«
    public void ChangeLaserPrefab(GameObject newLaserPrefab)
    {
        laserPrefab = newLaserPrefab;
    }
}