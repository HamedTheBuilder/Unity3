using UnityEngine;

public class EnemyController22 : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float moveSpeed = 2f;
    public int health = 1;
    public GameObject deathEffect;

    private bool isDead = false;
    private CharacterController enemyCharacterController;

    void Start()
    {
        enemyCharacterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!isDead && enemyCharacterController != null)
        {
            // ÍÑßÉ ÇáÚÏæ ÇáÈÓíØÉ
            Vector3 moveDirection = transform.forward * moveSpeed;
            enemyCharacterController.Move(moveDirection * Time.deltaTime);
        }
    }

    public void DieFromStomp()
    {
        if (isDead) return;

        isDead = true;

        // ÊÚØíá ÇáãßæäÇÊ
        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = false;

        if (enemyCharacterController != null)
            enemyCharacterController.enabled = false;

        // ÊÃËíÑ ÇáãæÊ (ÇÎÊíÇÑí)
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // ÊÍÑíß ÇáÚÏæ ááÃÓİá ŞáíáÇğ (ÊÃËíÑ Stomp)
        transform.position += Vector3.down * 0.2f;

        // ÊÚØíá script ÇáÌÇäÈí ááÚÏæ
        Enemyside enemySide = GetComponent<Enemyside>();
        if (enemySide != null)
            enemySide.enabled = false;

        // ÊÏãíÑ ÇáÚÏæ ÈÚÏ İÊÑÉ
        Destroy(gameObject);

        Debug.Log("Enemy died from stomp!");
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // ÊÛííÑ ÇÊÌÇå ÇáÚÏæ ÚäÏ ÇáÇÕØÏÇã ÈÍÇÆØ
        if (hit.gameObject.CompareTag("Wall"))
        {
            transform.forward *= -1;
        }
    }
}