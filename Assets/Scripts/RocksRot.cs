using UnityEngine;

public class AsteroidRotation : MonoBehaviour
{
    public float rotationSpeed = 50f; // ÓÑÚÉ ÇáÊÏæíÑ¡ íãßä ÊÚÏíáåÇ İí ÇáÜ Inspector

    void Update()
    {
        // ÊÏæíÑ ÇáßæíßÈ Íæá ÇáãÍæÑ Z
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
