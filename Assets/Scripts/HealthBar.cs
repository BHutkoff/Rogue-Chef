using UnityEngine;
using UnityEngine.UI;  // Make sure to include this for working with UI components

public class HealthBar : MonoBehaviour
{
    public Image healthFillImage;  // Reference to the Image component for the health fill

    // Set the health bar's maximum and current health
    public void SetHealth(int currentHealth, int maxHealth)
    {
        // Calculate the fill amount as a percentage
        float fillAmount = (float)currentHealth / maxHealth;
        healthFillImage.fillAmount = fillAmount;  // Update the health fill amount
    }

    // Optionally, if you want to smoothly update the health bar
    public void SetHealthSmooth(int currentHealth, int maxHealth)
    {
        // Smoothly transition the health fill
        float fillAmount = (float)currentHealth / maxHealth;
        healthFillImage.fillAmount = fillAmount;
    }
}
