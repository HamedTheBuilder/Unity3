using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 2, -5);

    private Vector3 velocity = Vector3.zero;
    private bool isFollowing = true;

    void LateUpdate()
    {
        if (target == null || !isFollowing) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }

    public void StopFollowing()
    {
        isFollowing = false;
    }

    public void ResumeFollowing()
    {
        isFollowing = true;
    }
}