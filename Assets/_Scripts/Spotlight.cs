using UnityEngine;

public class SpotLight : MonoBehaviour
{
    public Light spotLight; // Assign the spotlight in the Inspector
    public float maxIntensity = 5.0f;
    public float speed = 1.0f; // Speed of intensity increase
    private bool isIncreasing;

    private void Start()
    {
        if (spotLight != null)
        {
            spotLight.intensity = 0.0f; // Initialize intensity
        }
    }

    private void Update()
    {
        if (isIncreasing)
        {
            IncreaseIntensity();
        }
    }

    public void StartIncreasing()
    {
        isIncreasing = true;
    }

    public void StopIncreasing()
    {
        isIncreasing = false;
    }

    private void IncreaseIntensity()
    {
        if (spotLight.intensity < maxIntensity)
        {
            spotLight.intensity += speed * Time.deltaTime;
            if (spotLight.intensity > maxIntensity)
            {
                spotLight.intensity = maxIntensity;
            }
        }
    }
}
