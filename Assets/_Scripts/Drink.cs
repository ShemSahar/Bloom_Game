using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DrinkWater : MonoBehaviour
{
    public Animator playerAnimator;
    public GameObject waterCup;
    public Transform handTarget;
    public Transform originalPosition;
    public MultiParentConstraint waterCupConstraint;

    private void Start()
    {
        // Initially set the water cup at the original position
        waterCup.transform.position = originalPosition.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Start the drinking animation
            playerAnimator.SetTrigger("Drink");
        }
    }

    public void StartDrinking()
    {
        // Adjust constraint weights to attach the cup to the hand
        waterCupConstraint.data.sourceObjects.SetTransform(1, handTarget);
        waterCupConstraint.data.sourceObjects.SetWeight(1, 1f);
        waterCupConstraint.data.sourceObjects.SetWeight(0, 0f);
    }

    public void StopDrinking()
    {
        // Adjust constraint weights to return the cup to the original position
        waterCupConstraint.data.sourceObjects.SetWeight(1, 0f);
        waterCupConstraint.data.sourceObjects.SetWeight(0, 1f);
    }
}
