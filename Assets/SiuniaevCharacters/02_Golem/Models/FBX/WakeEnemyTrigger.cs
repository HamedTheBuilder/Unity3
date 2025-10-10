using UnityEngine;

public class WakeEnemyTrigger : MonoBehaviour
{
    public EnemyController enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            enemy.WakeUpEnemy();
    }
}
