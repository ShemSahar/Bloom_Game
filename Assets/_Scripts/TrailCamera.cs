using UnityEngine;

public class TrailCamera : MonoBehaviour
{
    public Transform target; // The target to follow (e.g., the player character)
    public float smoothSpeed = 0.125f; // The smoothness of camera movement

    private Vector3 offset; // The offset between the camera and the target

    void Start()
    {
        if (target != null)
        {
            // Calculate the initial offset between the camera and the target
            offset = transform.position - target.position;
        }
        else
        {
            Debug.LogWarning("TrailCamera: Target is not assigned!");
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the desired position of the camera, including the target's Z position
            Vector3 desiredPosition = target.position + offset;

            // Smoothly move the camera towards the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
}
