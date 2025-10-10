using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Death Zone Settings")]
    public float respawnDelay = 1f;
    public GameObject deathEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMoveBridge player = other.GetComponent<PlayerMoveBridge>();
            if (player != null)
            {
                //  ›⁄Ì·  √ÀÌ— «·„Ê 
                if (deathEffect != null)
                {
                    Instantiate(deathEffect, other.transform.position, Quaternion.identity);
                }

                // „Ê  «··«⁄» (”ÌﬁÊ„  ·ﬁ«∆Ì« »«·—Ì” «—  »⁄œ À«‰Ì…)
                player.DieFromFall();
            }
        }
    }
}