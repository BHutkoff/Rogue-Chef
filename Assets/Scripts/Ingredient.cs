using UnityEngine;

public enum IngredientType
{
    Sweet,
    Spicy,
    Sour,
    Bitter
}

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Ingredient")]
public class Ingredient : ScriptableObject
{
    public string ingredientName;  // Name of the ingredient
    public IngredientType type;    // Type of the ingredient (Sweet, Spicy, etc.)
    public int damage;             // Damage dealt by this ingredient
    public int healAmount;         // Healing amount for the player
    public int burnDamage;         // Burn damage applied when added to the pot
    public float burnDuration;     // Duration for the burn effect (in turns)
    public Sprite icon;            // Icon to represent the ingredient in the UI

    // This method applies the effects instantly when the ingredient is added to the pot
    public void ApplyEffect(Player player, Enemy enemy)
    {
        // Apply direct damage to the enemy (if applicable)
        if (damage > 0)
        {
            enemy.TakeDamage(damage);
        }

        // Apply healing to the player (if applicable)
        if (healAmount > 0)
        {
            player.Heal(healAmount);
        }

        // Apply burn damage to the enemy (if applicable)
        if (burnDamage > 0)
        {
            // Add burn damage and duration to the enemy
            enemy.AddBurnDamage(burnDamage, burnDuration);
        }
    }
}
