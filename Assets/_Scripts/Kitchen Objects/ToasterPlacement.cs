using UnityEngine;

namespace MyGame
{
    public class ToasterPlacement : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Toaster"))
            {
                Debug.Log("Toaster has been placed.");
            }
        }
    }
}
