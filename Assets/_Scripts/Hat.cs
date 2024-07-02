using UnityEngine;

public class HatPlacement : MonoBehaviour
{
    public GameObject coatRack; // Assign the coat rack in the inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == coatRack)
        {
            // Snap to the desired position and rotation
            transform.position = coatRack.transform.position + new Vector3(0, 1.5f, 0); // Adjust the offset as needed
            transform.rotation = coatRack.transform.rotation;

            // Freeze the hat's position and rotation
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

            // Optionally, set the parent
            transform.parent = coatRack.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == coatRack)
        {
            // Unfreeze the hat's position and rotation
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.None;
            }

            // Unparent the hat
            transform.parent = null;
        }
    }
}
