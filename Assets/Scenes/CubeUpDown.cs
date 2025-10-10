using UnityEngine;

public class CubePlatform : MonoBehaviour
{
    public float Amount = 5f; 
    public float speed = 2f;       

    private Vector3 startPos;
    private Vector3 lastPos;
    private Rigidbody rb;

    void Start()
    {
        startPos = transform.position;
        lastPos = startPos;

    
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true; 
    }

    void FixedUpdate()
    {
     
        float newY = startPos.y + Mathf.Sin(Time.time * speed) * Amount;
        Vector3 newPos = new Vector3(startPos.x, newY, startPos.z);


        rb.MovePosition(newPos);

     
        Vector3 delta = newPos - lastPos;

        lastPos = newPos;
    }

    private void OnCollisionStay(Collision collision)
    {
        Rigidbody playerRb = collision.rigidbody;
        if (playerRb != null && collision.collider.CompareTag("Player"))
        {
            playerRb.MovePosition(playerRb.position + rb.linearVelocity * Time.fixedDeltaTime);
        }
    }
}
