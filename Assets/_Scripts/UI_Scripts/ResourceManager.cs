using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI lightText;
    public float maxResource = 100f;
    public float water = 50f;
    public float lightResource = 50f; // Renamed to avoid conflict
    public float waterDrainRate = 1f;
    public float lightDrainRate = 1f;
    public float jumpDrainMultiplier = 2f;
    public float minSpeed = 1f;
    public float maxSpeed = 5f;
    [SerializeField] private GameObject player; // Reference to the player GameObject

    private JoystickController joystickController;
    private bool isJumping = false;

    void Start()
    {
        UpdateUI();
        joystickController = player.GetComponent<JoystickController>();
        if (joystickController == null)
        {
            Debug.LogError("JoystickController component not found on the player GameObject.");
        }
    }

    void Update()
    {
        DrainResources();
        UpdateSpeed();
        UpdateUI();
    }

    public void AddWater(float amount)
    {
        water = Mathf.Clamp(water + amount, 0f, maxResource);
        Debug.Log("Added Water: " + amount + ", New Water Level: " + water);
        UpdateSpeed();
        UpdateUI();
    }

    public void AddSunlight(float amount)
    {
        lightResource = Mathf.Clamp(lightResource + amount, 0f, maxResource);
        Debug.Log("Added Sunlight: " + amount + ", New Light Level: " + lightResource);
        UpdateSpeed();
        UpdateUI();
    }

    void DrainResources()
    {
        float waterDrain = isJumping ? waterDrainRate * jumpDrainMultiplier : waterDrainRate;
        float lightDrain = isJumping ? lightDrainRate * jumpDrainMultiplier : lightDrainRate;

        water -= waterDrain * Time.deltaTime;
        lightResource -= lightDrain * Time.deltaTime;

        water = Mathf.Clamp(water, 0f, maxResource);
        lightResource = Mathf.Clamp(lightResource, 0f, maxResource);
    }

    void UpdateSpeed()
    {
        if (joystickController != null)
        {
            float moveSpeed = CalculateSpeed();
            joystickController.SetSpeed(moveSpeed);
        }
    }

    float CalculateSpeed()
    {
        float waterEffect = CalculateResourceEffect(water);
        float lightEffect = CalculateResourceEffect(lightResource);

        return Mathf.Clamp(maxSpeed - Mathf.Max(waterEffect, lightEffect), minSpeed, maxSpeed);
    }

    float CalculateResourceEffect(float resource)
    {
        float middlePoint = maxResource / 2f;
        float deviation = Mathf.Abs(resource - middlePoint);

        return deviation / middlePoint * (maxSpeed - minSpeed);
    }

    public void OnJump()
    {
        isJumping = true;
        Invoke("StopJumping", 0.5f); // Assume jump duration is 0.5 seconds
    }

    void StopJumping()
    {
        isJumping = false;
    }

    void UpdateUI()
    {
        waterText.text = water.ToString("0");
        lightText.text = lightResource.ToString("0");

        waterText.color = CalculateTextColor(water);
        lightText.color = CalculateTextColor(lightResource);
    }

    Color CalculateTextColor(float resource)
    {
        if (resource < 50 || resource > 100)
        {
            float t;
            if (resource < 50)
            {
                t = (50 - resource) / 50;
            }
            else
            {
                t = (resource - 100) / 50;
            }
            return Color.Lerp(Color.white, Color.red, t);
        }
        else
        {
            return Color.white;
        }
    }
}
