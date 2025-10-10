using UnityEngine;

public class TriggerMoveObject : MonoBehaviour
{
    [Header("References")]
    public GameObject objectToMove;   // The object that will move
    public Transform targetPoint;     // Assign the empty object in the Inspector
    public float speed = 3f;          // Movement speed

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        // Check if the Player triggered
        if (other.CompareTag("Player"))
        {
            triggered = true;
        }
    }

    void Update()
    {
        if (triggered && objectToMove != null && targetPoint != null)
        {
            // Move the object toward the target
            objectToMove.transform.position = Vector3.MoveTowards(
                objectToMove.transform.position,
                targetPoint.position,
                speed * Time.deltaTime
            );

            // Stop when it reaches the target
            if (Vector3.Distance(objectToMove.transform.position, targetPoint.position) < 0.01f)
            {
                triggered = false; // stop moving
            }
        }
    }
}
