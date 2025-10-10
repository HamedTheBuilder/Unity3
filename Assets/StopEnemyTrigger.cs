using UnityEngine;

public class StopEnemyTrigger : MonoBehaviour
{
    public EnemyController enemy;
    public float stopDuration = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // Or check specific enemy
            enemy.StopForSeconds(stopDuration);
    }
}
