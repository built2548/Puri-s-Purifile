using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Transform target;
    // Make sure you drag the actual Slider component from the Hierarchy onto this slot in the Inspector
    [SerializeField] private Slider slider;
    [SerializeField] private Vector3 offset;

    // Corrected the typo in the parameter name
    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        // Set the slider's value based on the ratio of current health to max health
        if (maxValue > 0)
        {
            slider.value = currentValue / maxValue;
        }
        else
        {
            slider.value = 0f;
        }
    } 
    void Update()
    {
        transform.rotation = camera.transform.rotation;
        transform.position = target.position + offset;
    }
}