using System.Collections;
using System.Collections.Generic;  // Add this to use Dictionary
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;  // Reference to the EnemyData ScriptableObject
    public HealthBar healthBar;  // Reference to the enemy's health bar

    private int health;
    private int maxHealth;
    private int totalBurnDamage;  // This will accumulate burn damage applied by ingredients
    private float burnDurationLeft; // Track remaining burn duration
    private float attackCooldownTimer;

    private SpriteRenderer spriteRenderer;  // Reference to the enemy's sprite renderer

    private void Start()
    {
        // Initialize the enemy stats from EnemyData
        if (enemyData != null)
        {
            maxHealth = enemyData.maxHealth;
            health = maxHealth;  // Start with full health
            healthBar.SetHealth(health, maxHealth);

            // Initialize burn damage and duration
            totalBurnDamage = enemyData.burnDamage;
            burnDurationLeft = enemyData.burnDuration;
        }
        else
        {
            Debug.LogError("EnemyData is not assigned!");
        }

        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on Enemy!");
        }
    }

    // Method to apply damage to the enemy
    public void TakeDamage(int amount)
    {
        if (gameObject == null) return;  // Prevent accessing the object if it’s destroyed

        health -= amount;
        Debug.Log($"Enemy took {amount} damage. Current Health: {health}");

        healthBar.SetHealth(health, maxHealth);  // Update the health bar after taking damage

        // Check if health has reached zero
        if (health <= 0)
        {
            StartCoroutine(FadeOutAndDestroy());  // Start the fade-out coroutine
        }
    }


    // Coroutine to fade out the enemy's sprite and health bar, then destroy the GameObject
    private IEnumerator FadeOutAndDestroy()
    {
        if (spriteRenderer == null) yield break;

        float fadeDuration = 1f;  // Duration of the fade effect
        float fadeTime = 0f;  // Time counter for the fade process
        float startAlpha = 1f;  // Start alpha value (fully visible)
        float targetAlpha = 0f;  // Target alpha value (fully transparent)

        // Fade the sprite (enemy body)
        while (fadeTime < fadeDuration)
        {
            fadeTime += Time.deltaTime;  // Increase fade time
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, fadeTime / fadeDuration);  // Smoothly interpolate alpha

            // Apply the alpha value to the sprite
            Color spriteColor = spriteRenderer.color;
            spriteColor.a = alpha;
            spriteRenderer.color = spriteColor;

            // Fade the health bar
            if (healthBar != null)
            {
                var canvasGroup = healthBar.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = alpha;  // Fade the health bar's canvas group
                }
                else
                {
                    // If no CanvasGroup, manually fade health bar images
                    foreach (var image in healthBar.GetComponentsInChildren<UnityEngine.UI.Image>())
                    {
                        Color imageColor = image.color;
                        imageColor.a = alpha;  // Apply fade to each image
                        image.color = imageColor;
                    }
                }
            }

            yield return null;  // Wait for the next frame
        }

        // After fade out is complete, destroy the enemy object
        Destroy(gameObject);
    }




    // Method to add burn damage, which stacks on the enemy
    public void AddBurnDamage(int burnDamage, float burnDuration)
    {
        // Stack burn damage and keep the longest duration
        totalBurnDamage += burnDamage;  // Stack burn damage
        burnDurationLeft = Mathf.Max(burnDurationLeft, burnDuration);  // Keep the longest duration
    }

    // Applies burn damage at the start of the enemy's turn
    public void ApplyBurnDamage()
    {
        if (burnDurationLeft > 0)
        {
            // Apply accumulated burn damage
            TakeDamage(totalBurnDamage);
            burnDurationLeft -= 1f;  // Decrease burn duration each time it's applied

             // Show floating text for burn damage immediately at the start of the enemy's turn
            FloatingTextManager.Instance.CreateFloatingText(
                $"-{totalBurnDamage} (Burn)",
                transform.position + new Vector3(-12, -5, 0), // Position above enemy
                Color.red
            );

        Debug.Log($"Enemy took {totalBurnDamage} burn damage. {burnDurationLeft} turns remaining.");
        }
    }

    private Dictionary<Attack, float> attackCooldowns = new Dictionary<Attack, float>();

    // New function to handle the enemy's turn actions (e.g., attack)
    public void PerformTurnAction(Player player)
    {
        // Reduce cooldowns for all attacks
        List<Attack> availableAttacks = new List<Attack>();

        foreach (var attack in enemyData.attacks)
        {
            if (!attackCooldowns.ContainsKey(attack))
                attackCooldowns[attack] = 0f;  // Ensure all attacks are tracked

            if (attackCooldowns[attack] <= 0f)
                availableAttacks.Add(attack);
            else
                attackCooldowns[attack] -= 1f; // Decrease cooldown per turn
        }

        if (availableAttacks.Count > 0)
        {
            // Select a random attack from available ones
            Attack selectedAttack = availableAttacks[Random.Range(0, availableAttacks.Count)];

            // Perform attack
            Debug.Log($"Enemy performs {selectedAttack.attackName} attack!");
            player.TakeDamage(selectedAttack.damage);

            // Apply cooldown for this attack
            attackCooldowns[selectedAttack] = selectedAttack.cooldown;

            // End the turn
            EndTurn();
        }
        else
        {
            Debug.Log("All attacks are on cooldown, enemy skips turn.");
            EndTurn();
        }
    }




    // Function to end the enemy's turn
    private void EndTurn()
    {
        // Logic for ending the enemy's turn (could include additional actions)
        Debug.Log("Enemy's turn has ended.");
    }
}
