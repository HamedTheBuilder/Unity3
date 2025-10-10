using UnityEngine;

public class CameraFlipTrigger : MonoBehaviour
{
    [Header("References")]
    public Camera targetCamera;       // Assign the camera in Inspector
    public float rotationSpeed = 50f; // Degrees per second

    private bool rotating = false;
    private float rotatedAmount = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !rotating)
        {
            rotating = true;
            rotatedAmount = 0f; // Reset for this flip
        }
    }

    void Update()
    {
        if (rotating && targetCamera != null)
        {
            float step = rotationSpeed * Time.deltaTime;

            // Rotate around Z axis
            targetCamera.transform.Rotate(0, 0, step);
            rotatedAmount += step;

            // Stop exactly at 180
            if (rotatedAmount >= 180f)
            {
                rotating = false;

                // Snap to the nearest exact 180 flipped value
                Vector3 angles = targetCamera.transform.eulerAngles;
                angles.z = Mathf.Round(angles.z / 180f) * 180f;
                targetCamera.transform.eulerAngles = angles;
            }
        }
    }
}
