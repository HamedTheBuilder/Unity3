using UnityEngine;
using System.Collections;

public class LogTrap : MonoBehaviour
{
    public float spawnInterval = 5f;
    public float warningTime = 1f;
    public float logSpeed = 10f;
    public GameObject logPrefab;
    public Transform spawnPoint;
    public Vector3 rollDirection = Vector3.forward;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(SpawnLogs());
    }

    IEnumerator SpawnLogs()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (audioSource != null)
                audioSource.Play();

            yield return new WaitForSeconds(warningTime);

            GameObject log = Instantiate(logPrefab, spawnPoint.position, Quaternion.identity);
            Rigidbody logRb = log.GetComponent<Rigidbody>();

            if (logRb != null)
            {
                logRb.AddForce(rollDirection * logSpeed, ForceMode.Impulse);
            }

            Destroy(log, 10f);
        }
    }
}