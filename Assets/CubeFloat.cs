using UnityEngine;

public class CubeFloat : MonoBehaviour
{
    public float distance = 5f;   // How far it moves up and down
    public float speed = 2f;      // How fast it moves

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Sine wave motion (smooth up and down)
        float yOffset = Mathf.Sin(Time.time * speed) * distance / 2f;
        transform.position = startPos + new Vector3(0, yOffset, 0);
    }
}
