using UnityEngine;

public class SimpleCollectible : MonoBehaviour
{
    [Header("Settings")]
    public int coinValue = 1;
    public AudioClip collectSound;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // تحديث العدادات
            if (UIManager.Instance != null)
            {
                UIManager.Instance.AddCoins(coinValue);
            }

            // تشغيل الصوت
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }
            else if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCoinCollect();
            }

            // إخفاء أو تدمير الكائن
            gameObject.SetActive(false);
            Destroy(gameObject, 0.5f);
        }
    }
}