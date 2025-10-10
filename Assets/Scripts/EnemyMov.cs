using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyMov : MonoBehaviour
{
    public Transform A;
    public Transform B;
    public float speed = 5f;   // control speed

    void Start()
    {
        transform.LookAt(A);
    }

    void Update()
    {
        // Move forward with adjustable speed
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        // Restart scene if player collides
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Use triggers for points A and B
        if (other.gameObject.CompareTag("A"))
        {
            transform.LookAt(B);
        }

        if (other.gameObject.CompareTag("B"))
        {
            transform.LookAt(A);
        }
    }
}
