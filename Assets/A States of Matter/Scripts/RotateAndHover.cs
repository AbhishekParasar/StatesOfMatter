using UnityEngine;

public class RotateAndHover : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 50, 0);
    public float hoverAmplitude = 0.5f;
    public float hoverSpeed = 2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Rotation
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // Hover
        float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}

