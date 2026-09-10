using UnityEngine;
using UnityEngine.UI;  // Add this for UI elements
using System.Collections.Generic; // For Dictionary
using TMPro;           // required for TextMeshPro

public class Player : MonoBehaviour
{
    public int health;  
    public int maxHealth;  
    public HealthBar healthBar;  
    public Inventory inventory;  

    private int burnTurnsRemaining = 0;  
    private int burnDamagePerTurn = 0;  

    // Ingredient inventory: Dictionary to hold ingredient names and quantities
    public Dictionary<string, int> ingredientInventory = new Dictionary<string, int>();

    // UI elements to display ingredient counts
    public TextMeshProUGUI bananaCountText;
    public TextMeshProUGUI lemonCountText;
    public TextMeshProUGUI limeCountText;
    public TextMeshProUGUI breadCountText;
    public TextMeshProUGUI eggCountText;
    public TextMeshProUGUI butterCountText;

    private void Start()
    {
        healthBar.SetHealth(health, maxHealth);

        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();  
        }

        // Add some ingredients to start with
        AddIngredient("Banana", 3);
        AddIngredient("Lemon", 2);
        AddIngredient("Lime", 5);
        AddIngredient("Bread", 3);
        AddIngredient("Egg", 2);
        AddIngredient("Butter", 5);
    }

    // Method to heal the player
    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        Debug.Log($"Player healed for {amount}. Current Health: {health}");
        healthBar.SetHealth(health, maxHealth);
    }

    // Method to apply damage to the player
    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log($"Player took {amount} damage. Current Health: {health}");
        healthBar.SetHealth(health, maxHealth);

        // Show floating text for damage above the player
        if (FloatingTextManager.Instance != null)
        {
            FloatingTextManager.Instance.CreateFloatingText(
                $"-{amount}", 
                transform.position + new Vector3(-5, -5, 0), // Offset to appear above the player
                Color.red
            );
        }
        else
        {
            Debug.LogError("FloatingTextManager instance is null!");
        }
    }

    // Apply burn damage at the start of the player's turn
    public void ApplyBurnDamage()
    {
        if (burnTurnsRemaining > 0)
        {
            TakeDamage(burnDamagePerTurn);
            burnTurnsRemaining--;
            Debug.Log($"Player took {burnDamagePerTurn} burn damage. {burnTurnsRemaining} turns remaining.");
        }
    }

    // Call this method to set burn details
    public void SetBurnEffect(int damagePerTurn, int turns)
    {
        burnDamagePerTurn = damagePerTurn;
        burnTurnsRemaining = turns;
    }

    // Add an ingredient to the inventory and update UI
    public void AddIngredient(string ingredientName, int amount)
    {
        if (ingredientInventory.ContainsKey(ingredientName))
        {
            ingredientInventory[ingredientName] += amount;
        }
        else
        {
            ingredientInventory[ingredientName] = amount;
        }

        Debug.Log($"Added {amount} {ingredientName}(s). Total: {ingredientInventory[ingredientName]}");

        // Update the UI
        UpdateIngredientUI();
    }

    // Use an ingredient and update UI
    public bool UseIngredient(string ingredientName)
    {
        if (HasIngredient(ingredientName))
        {
            ingredientInventory[ingredientName]--;
            Debug.Log($"Used {ingredientName}. Remaining: {ingredientInventory[ingredientName]}");

            UpdateIngredientUI();  // Update UI after using an ingredient

            if (ingredientInventory[ingredientName] <= 0)
            {
                DestroyIngredientObject(ingredientName);
            }

            return true;
        }

        Debug.Log($"No {ingredientName} left!");
        return false;
    }

    // Update the UI to show ingredient counts
    private void UpdateIngredientUI()
    {
        if (bananaCountText != null && ingredientInventory.ContainsKey("Banana"))
            bananaCountText.text = "" + ingredientInventory["Banana"];

        if (lemonCountText != null && ingredientInventory.ContainsKey("Lemon"))
            lemonCountText.text = "" + ingredientInventory["Lemon"];

        if (limeCountText != null && ingredientInventory.ContainsKey("Lime"))
            limeCountText.text = "" + ingredientInventory["Lime"];

        if (breadCountText != null && ingredientInventory.ContainsKey("Bread"))
            breadCountText.text = "" + ingredientInventory["Bread"];

        if (eggCountText != null && ingredientInventory.ContainsKey("Egg"))
            eggCountText.text = "" + ingredientInventory["Egg"];

        if (butterCountText != null && ingredientInventory.ContainsKey("Butter"))
            butterCountText.text = "" + ingredientInventory["Butter"];
    }

    // Check if player has an ingredient
    public bool HasIngredient(string ingredientName)
    {
        return ingredientInventory.ContainsKey(ingredientName) && ingredientInventory[ingredientName] > 0;
    }

    // Destroy ingredient object if it runs out
    private void DestroyIngredientObject(string ingredientName)
    {
        GameObject ingredientObject = GameObject.Find(ingredientName);
        if (ingredientObject != null)
        {
            Destroy(ingredientObject);
            Debug.Log($"{ingredientName} has been destroyed because it's out of stock.");
        }
    }

    // Add an ingredient to the pot from the player's inventory
    public bool AddIngredientToPot(Ingredient ingredient, CookingPot cookingPot)
    {
        // Check if the player has the ingredient in their inventory
        if (!HasIngredient(ingredient.ingredientName))
        {
            Debug.Log("Ingredient not available in inventory.");
            return false;  // Ingredient is not in the inventory, do nothing
        }

        // Use the ingredient (remove it from the inventory) before adding to the cooking pot
        UseIngredient(ingredient.ingredientName);

        // Add the ingredient to the cooking pot
        cookingPot.AddIngredient(ingredient);  // No return value needed

        return true;  // Ingredient was successfully added
    }
}
