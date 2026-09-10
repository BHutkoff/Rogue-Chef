using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IngredientData
{
    public Ingredient ingredient;  // Reference to the ingredient
    public int quantity;           // Number of this ingredient in the inventory
}

public class Inventory : MonoBehaviour
{
    public List<IngredientData> ingredients;  // List of ingredients and their quantities

    // This method adds an ingredient to the inventory (or increases the quantity if it already exists)
    public void AddIngredient(Ingredient ingredient)
    {
        foreach (var data in ingredients)
        {
            if (data.ingredient == ingredient)
            {
                data.quantity++;
                Debug.Log($"Added {ingredient.ingredientName}. New quantity: {data.quantity}");
                return;
            }
        }

        // If the ingredient is not found in the inventory, add it with quantity 1
        ingredients.Add(new IngredientData { ingredient = ingredient, quantity = 1 });
        Debug.Log($"Added new ingredient {ingredient.ingredientName} to inventory.");
    }

    // This method removes an ingredient from the inventory (decreases the quantity)
    public void RemoveIngredient(Ingredient ingredient)
    {
        foreach (var data in ingredients)
        {
            if (data.ingredient == ingredient && data.quantity > 0)
            {
                data.quantity--;
                Debug.Log($"Removed {ingredient.ingredientName}. New quantity: {data.quantity}");
                return;
            }
        }
        Debug.Log($"Ingredient {ingredient.ingredientName} not found in inventory or quantity is 0.");
    }

    // This method checks if the player has a certain ingredient in the inventory
    public bool HasIngredient(Ingredient ingredient)
    {
        foreach (var data in ingredients)
        {
            if (data.ingredient == ingredient && data.quantity > 0)
            {
                return true;
            }
        }
        return false;
    }
}
