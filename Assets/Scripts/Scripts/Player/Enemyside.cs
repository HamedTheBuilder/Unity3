using UnityEngine;
using System.Collections;

public class Enemyside : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float damageCooldown = 1f; // Êﬁ  «·«‰ Ÿ«— »Ì‰ «·÷—»« 

    private EnemyController enemyController;
    private bool canDamage = true;
    private Coroutine damageCoroutine;

    void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player" && canDamage)
        {
            // „‰⁄ «·÷—— ≈–« ﬂ«‰ «·⁄œÊ „Ì «
            if (enemyController == null || !IsEnemyDead())
            {
                ApplyDamage(collision);
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.tag == "Player" && canDamage)
        {
            // „‰⁄ «·÷—— ≈–« ﬂ«‰ «·⁄œÊ „Ì «
            if (enemyController == null || !IsEnemyDead())
            {
                ApplyDamage(collision);
            }
        }
    }

    private void ApplyDamage(Collider playerCollider)
    {
        Health playerHealth = playerCollider.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            StartDamageCooldown();
        }
    }

    private void StartDamageCooldown()
    {
        canDamage = false;

        // ≈Ìﬁ«› «·ﬂÊ—Ê Ì‰ «·”«»ﬁ ≈–« ﬂ«‰ Ì⁄„·
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }

        damageCoroutine = StartCoroutine(DamageCooldownRoutine());
    }

    private IEnumerator DamageCooldownRoutine()
    {
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
        damageCoroutine = null;
    }

    private bool IsEnemyDead()
    {
        return enemyController != null && enemyController.enabled == false;
    }

    // ≈⁄«œ…  ⁄ÌÌ‰ «·ﬂÊ· œ«Ê‰ ⁄‰œ  ⁄ÿÌ· «·ﬂ«∆‰
    private void OnDisable()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
        canDamage = true;
    }
}