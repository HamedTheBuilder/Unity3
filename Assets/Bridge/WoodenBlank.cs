using UnityEngine;
using System.Collections;

public class WoodenPlank : MonoBehaviour
{
    public enum PlankType
    {
        Stable,         // À«»  ·« ÌÿÌÕ
        TimedFall,      // ÌÿÌÕ »⁄œ À«‰Ì Ì‰
        InstantFall     // ÌÿÌÕ ›Ê— «·„‘Ì ⁄·ÌÂ
    }

    public PlankType plankType = PlankType.Stable;
    public float fallDelay = 2f;

    private Rigidbody rb;
    private bool hasBeenSteppedOn = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasBeenSteppedOn)
        {
            hasBeenSteppedOn = true;

            switch (plankType)
            {
                case PlankType.TimedFall:
                    StartCoroutine(FallAfterDelay());
                    break;

                case PlankType.InstantFall:
                    Fall();
                    break;
            }
        }
    }

    IEnumerator FallAfterDelay()
    {
        yield return new WaitForSeconds(fallDelay);
        Fall();
    }

    void Fall()
    {
        if (rb != null)
        {
            rb.isKinematic = false;

            //  œ„Ì— «··ÊÕ… »⁄œ ”ﬁÊÿÂ«
            Destroy(gameObject, 5f);
        }
    }
}