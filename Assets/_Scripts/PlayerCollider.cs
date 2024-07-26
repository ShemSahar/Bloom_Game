using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    public Transform capsuleTransform; // Reference to the capsule's transform
    public float speed = 5f;
    public float zLimit = 11f;
    public float rotationSpeed = 5f;
    public float leanAngle = 30f;
    public Vector3 normalScale = new Vector3(1.3f, 1.3f, 1.3f);
    public Vector3 movingScale = new Vector3(1.4f, 1.4f, 1.4f);

    void Start()
    {
        // Set the initial scale
        if (capsuleTransform != null)
        {
            capsuleTransform.localScale = normalScale;
        }
    }

    void Update()
    {
        if (capsuleTransform == null)
        {
            Debug.LogWarning("Capsule Transform not set!");
            return;
        }

        // Handle movement input
        float moveZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        // If there is movement, move, lean and scale up
        if (moveZ != 0)
        {
            // Move the player
            capsuleTransform.Translate(0, 0, moveZ);

            // Clamp the Z position between -zLimit and zLimit
            float clampedZ = Mathf.Clamp(capsuleTransform.position.z, -zLimit, zLimit);
            capsuleTransform.position = new Vector3(capsuleTransform.position.x, capsuleTransform.position.y, clampedZ);

            // Gradually lean the player to 30 degrees on the X-axis
            float targetAngle = Mathf.Sign(moveZ) * leanAngle;
            Quaternion targetRotation = Quaternion.Euler(targetAngle, capsuleTransform.rotation.eulerAngles.y, capsuleTransform.rotation.eulerAngles.z);
            capsuleTransform.rotation = Quaternion.Slerp(capsuleTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Scale the player to moving scale
            capsuleTransform.localScale = Vector3.Lerp(capsuleTransform.localScale, movingScale, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Gradually return to upright position when not moving
            Quaternion uprightRotation = Quaternion.Euler(0, capsuleTransform.rotation.eulerAngles.y, capsuleTransform.rotation.eulerAngles.z);
            capsuleTransform.rotation = Quaternion.Slerp(capsuleTransform.rotation, uprightRotation, rotationSpeed * Time.deltaTime);

            // Scale the player back to normal scale
            capsuleTransform.localScale = Vector3.Lerp(capsuleTransform.localScale, normalScale, rotationSpeed * Time.deltaTime);
        }
    }
}
