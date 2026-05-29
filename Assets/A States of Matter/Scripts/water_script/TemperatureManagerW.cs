using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TemperatureManagerW : MonoBehaviour
{
    public Button increaseTemperature, decreaseTemperature;
    private AdvancedParticleSimulationW advancedParticleSimulation;
    public Image thermostatfillimage;

    public bool isIncreasing = false;
    public bool isDecreasing = false;
    private float tempChangeSpeed = 10f; // Speed at which the temperature changes per second

    private void Awake()
    {
        tempChangeSpeed = 5;
         advancedParticleSimulation = FindAnyObjectByType<AdvancedParticleSimulationW>();
        
        Debug.Log("AdvancedParticleSimulation initialized.");

        // Add EventTrigger components for press-and-hold functionality
        AddEventTrigger(increaseTemperature.gameObject, EventTriggerType.PointerDown, OnIncreaseButtonDown);
        AddEventTrigger(increaseTemperature.gameObject, EventTriggerType.PointerUp, OnIncreaseButtonUp);

        AddEventTrigger(decreaseTemperature.gameObject, EventTriggerType.PointerDown, OnDecreaseButtonDown);
        AddEventTrigger(decreaseTemperature.gameObject, EventTriggerType.PointerUp, OnDecreaseButtonUp);
    }

    private void Update()
    {
        if (isIncreasing)
        {
            advancedParticleSimulation.temperature += tempChangeSpeed * Time.deltaTime;
            Debug.Log($"Increasing temperature: {advancedParticleSimulation.temperature}");
            UpdateTemperatureUI();
        }

        if (isDecreasing)
        {
            advancedParticleSimulation.temperature -= tempChangeSpeed * Time.deltaTime;
            Debug.Log($"Decreasing temperature: {advancedParticleSimulation.temperature}");
            UpdateTemperatureUI();
        }
    }

    private void AddEventTrigger(GameObject obj, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>() ?? obj.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    private void OnIncreaseButtonDown(BaseEventData data)
    {
        Debug.Log("Increase button pressed.");
        isIncreasing = true;
    }

    private void OnIncreaseButtonUp(BaseEventData data)
    {
        Debug.Log("Increase button released.");
        isIncreasing = false;
    }

    private void OnDecreaseButtonDown(BaseEventData data)
    {
        Debug.Log("Decrease button pressed.");
        isDecreasing = true;
    }

    private void OnDecreaseButtonUp(BaseEventData data)
    {
        Debug.Log("Decrease button released.");
        isDecreasing = false;
    }

    private void UpdateTemperatureUI()
{
    // Clamp temperature within the argon simulation range
    advancedParticleSimulation.temperature = Mathf.Clamp(
        advancedParticleSimulation.temperature,
        -5f, // Minimum temp for argon simulation
        110f  // Maximum temp for argon simulation
    );

    // Update simulation state based on the clamped temperature
    advancedParticleSimulation.UpdateStateByTemperature();

    // Round to one decimal place
    float roundedTemperature = Mathf.Round(advancedParticleSimulation.temperature * 10f) / 10f;

    // Update thermostat fill image
    float normalizedTemperature = (roundedTemperature - (-5f)) / (110f - (-5f)); // Normalize temperature to 0-1 range
    thermostatfillimage.fillAmount = normalizedTemperature;

    // Debug log for tracking values
    Debug.Log($"Temperature updated to {roundedTemperature}, fill amount set to {thermostatfillimage.fillAmount}");
}




    public void SetSolidMode()
    {
        Debug.Log("Setting Solid Mode.");
        advancedParticleSimulation.SetToSolid();
        advancedParticleSimulation.UpdateStateByTemperature();
        advancedParticleSimulation.temperature = -0f;
        UpdateTemperatureUI();
    }

    public void SetLiquidMode()
    {
        Debug.Log("Setting Liquid Mode.");
        advancedParticleSimulation.currentState = AdvancedParticleSimulationW.State.Liquid;
        advancedParticleSimulation.temperature = 2f;
        UpdateTemperatureUI();
    }

    public void SetGasMode()
    {
        Debug.Log("Setting Gas Mode.");
        advancedParticleSimulation.currentState = AdvancedParticleSimulationW.State.Gas;
        advancedParticleSimulation.temperature = 100f;
        UpdateTemperatureUI();
    }
}
