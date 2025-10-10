using UnityEngine;

public class EnemyHead : MonoBehaviour
{
    public float minDownwardSpeed = -0.5f; // áÇÒã ÇááÇÚÈ íßæä äÇÒá ãæ ØÇáÚ
    private EnemyController enemyController;

    void Start()
    {
        // ÇáÍÕæá Úáì EnemyController ãä ÇáÃÈ
        enemyController = GetComponentInParent<EnemyController>();
    }

  /*  void OnTriggerEnter(Collider other)
    {
        // ÊÍŞŞ ãä Ãä ÇáÊÕÇÏã ãÚ ÇááÇÚÈ
        if (other.CompareTag("Player"))
        {
            // ÊÍŞŞ ÅĞÇ ßÇä ÇááÇÚÈ äÇÒá Úáì ÑÃÓ ÇáÚÏæ
            var player = other.GetComponent<CharMovement>();
            if (player != null && player.VerticalSpeed <= minDownwardSpeed && enemyController != null)
            {
                // ÊØÈíŞ ÇáŞİÒ ÇáÊáŞÇÆí ÚäÏãÇ íŞİÒ ÇááÇÚÈ Úáì ÑÃÓ ÇáÚÏæ
                player.AutoJumpOnEnemyHead();

                // ŞÊá ÇáÚÏæ
                enemyController.DieFromStomp();
            }
        }
    } */
}