using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingPot : MonoBehaviour
{
    private Animator animator;  // Reference to the Animator
    public TurnManager turnManager;  // Reference to the TurnManager script

    public List<Ingredient> currentIngredients = new List<Ingredient>();
    public int maxIngredients = 5;  // Max ingredients that can be added at once
    public Player player;  // Reference to the player
    public Enemy enemy;    // Reference to the enemy

    private bool isCookingComplete = false;  // Flag to check if cooking is complete

    public SpriteRenderer timerSpriteRenderer;  // Reference to the SpriteRenderer of the timer
    public Sprite[] timerSprites;  // Array of sprites for timer (5, 4, 3, 2, 1, 0)

    private int cookingTime = 5;  // Timer value (starts at 5)

    // UI elements (assuming you're using Image components to display ingredients)
    public Image[] ingredientSlots;  // UI elements for ingredient slots
    public Sprite emptySlotSprite;  // Sprite to show when a slot is empty

    private void Start()
    {
        // Get the Animator component from the CookingPot GameObject
        animator = GetComponent<Animator>();

        // Ensure the timer starts at the correct sprite
        UpdateTimerSprite();

        // Initialize the ingredient slots (if necessary)
        if (ingredientSlots.Length != maxIngredients)
        {
            Debug.LogError("Ingredient slots array size does not match maxIngredients!");
        }
    }

    // Method to add an ingredient to the pot
    public void AddIngredient(Ingredient ingredient)
    {
        if (currentIngredients == null)
        {
            Debug.LogError("currentIngredients list is null");
            return;
        }

        if (ingredient == null)
        {
            Debug.LogError("Ingredient is null");
            return;
        }

        // Check if the player has the ingredient before adding it
        if (!player.HasIngredient(ingredient.ingredientName))
        {
            Debug.Log($"Player does not have {ingredient.ingredientName}. Ingredient cannot be added.");
            return;  // Prevent adding the ingredient if it's not in the player's inventory
        }

        if (currentIngredients.Count < maxIngredients)
        {
            currentIngredients.Add(ingredient);
            ingredient.ApplyEffect(player, enemy);  // Apply effect instantly when added
            Debug.Log($"Ingredient {ingredient.ingredientName} added and effect applied!");

            // Trigger AddIngredient animation (can be customized)
            animator.SetTrigger("AddIngredient");  // Trigger the animation

            // Decrease the cooking time and update the timer sprite
            DecreaseCookingTime();

            // If we have reached max ingredients, start cooking
            if (currentIngredients.Count == maxIngredients)
            {
                // Trigger the End Cooking animation only when max ingredients are reached
                TriggerEndCooking();
            }

            // Update the ingredient slots UI
            UpdateIngredientSlots();

            // Create floating text based on the ingredient's effect
            if (ingredient.healAmount > 0)
            {
                // Create floating text for healing above the player
                Vector3 playerPosition = player.transform.position + new Vector3(-5, -5, 0);  // text position
                floatingTextManager.CreateFloatingText("" + ingredient.healAmount, playerPosition, Color.green);
            }

            if (ingredient.damage > 0)
            {
                // Create floating text for damage above the enemy
                Vector3 enemyPosition = enemy.transform.position + new Vector3(-12, -5, 0);  // text position
                floatingTextManager.CreateFloatingText("" + ingredient.damage, enemyPosition, Color.red);
            }

            if (ingredient.burnDamage > 0)
            {
                // Create floating text for burn damage above the enemy
                Vector3 enemyPosition = enemy.transform.position + new Vector3(-12, -5, 0);  // text position
                floatingTextManager.CreateFloatingText("Burn: " + ingredient.burnDamage, enemyPosition, Color.red);
            }

            // Reduce the ingredient from the player's inventory
            player.UseIngredient(ingredient.ingredientName);  // Update the inventory
        }
        else
        {
            Debug.Log("Pot is full! Prepare your dish.");
        }
    }

    // Method to decrease cooking time and update the sprite
    private void DecreaseCookingTime()
    {
        // Decrease the cooking time by 1
        cookingTime--;

        // Make sure the timer doesn't go below 0
        if (cookingTime < 0)
        {
            cookingTime = 0;
        }

        // Update the sprite to reflect the new cooking time
        UpdateTimerSprite();

        // If the timer reaches 0, trigger end cooking and switch turns
        if (cookingTime == 0)
        {
            TriggerEndCooking();
            EndPlayerTurn();  // Switch the turn to the enemy
        }
    }

    // Method to update the timer sprite based on cooking time
    private void UpdateTimerSprite()
    {
        if (cookingTime >= 0 && cookingTime < timerSprites.Length)
        {
            timerSpriteRenderer.sprite = timerSprites[cookingTime];  // Set the sprite based on the timer value
        }
    }

    // Method to handle the end cooking process (triggered by various conditions)
    private void TriggerEndCooking()
    {
        Debug.Log("TriggerEndCooking() called.");  // ✅ Log to confirm this method runs

        if (!isCookingComplete)
        {
            isCookingComplete = true;  // Mark cooking as complete

            // Reset animation triggers
            animator.ResetTrigger("AddIngredient");
            animator.SetTrigger("EndCooking");

            Debug.Log("Calling AttemptToCreateRecipe()...");  // ✅ Log before calling

            // Check if a recipe exists
            AttemptToCreateRecipe();
        }
    }


    // Method to check if the current ingredients match a recipe and apply special effects if they do
    private void AttemptToCreateRecipe()
    {
        Debug.Log("Attempting to create a recipe...");

        if (currentIngredients.Count == 0)
        {
            Debug.Log("No ingredients in the pot. Recipe check skipped.");
            return;
        }

        // Ensure GetMatchingRecipe() is called here
        Recipe matchingRecipe = RecipeDatabase.Instance.GetMatchingRecipe(currentIngredients);
        if (matchingRecipe != null)
        {
            Debug.Log($"Created recipe: {matchingRecipe.recipeName}!");
            ApplyRecipeEffects(matchingRecipe);
        }
        else
        {
            Debug.Log("No recipe found. Ingredients were already processed individually.");
        }

        // Clear the pot after processing
        currentIngredients.Clear();
    }



    // Method that applies the special effects of a successfully created recipe
    private void ApplyRecipeEffects(Recipe recipe)
    {
        Debug.Log($"Applying special effects for recipe: {recipe.recipeName}");

        if (recipe.damage > 0)
        {
            enemy.TakeDamage(recipe.damage);
            
            // Display floating text for damage above the enemy
            FloatingTextManager.Instance.CreateFloatingText(
                $"-{recipe.damage} ({recipe.recipeName})",
                enemy.transform.position + new Vector3(-12, -5, 0), // Adjust position above enemy
                Color.red
            );
        }

        if (recipe.healing > 0)
        {
            player.Heal(recipe.healing);

            // Display floating text for healing above the player
            FloatingTextManager.Instance.CreateFloatingText(
                $"+{recipe.healing} ({recipe.recipeName})",
                player.transform.position + new Vector3(-5, -5, 0), // Adjust position above player
                Color.green
            );
        }

        if (recipe.appliesBurn)
        {
            enemy.AddBurnDamage(5, 3); // Example: Apply 5 burn damage for 3 turns

            // Display floating text for burn effect
            FloatingTextManager.Instance.CreateFloatingText(
                $"Burn ({recipe.recipeName})",
                enemy.transform.position + new Vector3(-12, -5, 0),
                Color.red
            );
        }
    }


    // Reference to the FloatingTextManager
    public FloatingTextManager floatingTextManager;

    // Executes the cooking process (applying ingredient effects)
    void ExecuteCooking()
    {
        int totalDamage = 0;
        int totalHeal = 0;
        int totalBurnDamage = 0;
        float totalBurnDuration = 0f;

        // Loop through all ingredients in the pot and apply their effects
        foreach (var ingredient in currentIngredients)
        {
            ingredient.ApplyEffect(player, enemy);

            totalDamage += ingredient.damage;
            totalHeal += ingredient.healAmount;
            totalBurnDamage += ingredient.burnDamage;
            totalBurnDuration += ingredient.burnDuration;
        }

        // Apply the effects and trigger floating texts
        if (totalHeal > 0) 
        {
            player.Heal(totalHeal);
            //floatingTextManager.CreateFloatingText($"+{totalHeal}", player.transform.position, Color.green); // Healing text
        }
        if (totalBurnDamage > 0) 
        {
            enemy.ApplyBurnDamage();
            floatingTextManager.CreateFloatingText($"-{totalBurnDamage} (Burn)", enemy.transform.position, Color.red); // Burn text
        }
        if (totalDamage > 0) 
        {
            enemy.TakeDamage(totalDamage);
            //floatingTextManager.CreateFloatingText($"-{totalDamage}", enemy.transform.position, Color.red); // Damage text
        }

        // Show cooking results in console
        Debug.Log($"Total Damage: {totalDamage}, Total Heal: {totalHeal}, Total Burn: {totalBurnDamage} over {totalBurnDuration} seconds");

        // Clear the pot after cooking
        currentIngredients.Clear();

        // Update the ingredient slots UI after cooking
        UpdateIngredientSlots();
    }

    // This method can be hooked to the End Turn button or some other event
    public void OnEndTurn()
    {
        Debug.Log("Player ended their turn. Running OnEndTurn()...");

        // Run cooking even if fewer than max ingredients were placed
        TriggerEndCooking();

        // Optionally, other end turn logic can be placed here
    }

    // Reset cooking state at the end of the turn
    public void ResetCookingState()
    {
        // Reset the isCookingComplete flag and clear the ingredients
        isCookingComplete = false;
        currentIngredients.Clear();  // Clear the ingredients from the pot

        // Reset the cooking time to 5 and update the sprite
        cookingTime = 5;
        UpdateTimerSprite();

        // Reset the "AddIngredient" trigger in the Animator to prepare for next turn
        animator.ResetTrigger("AddIngredient");

        // Optionally, trigger an animation to reset the pot's visual state if needed
        animator.SetTrigger("ResetPot");  // Trigger the animation (if you have one)

        // Update the ingredient slots UI after reset
        UpdateIngredientSlots();
    }

    // Method to end the player's turn when the timer hits 0
    private void EndPlayerTurn()
    {
        // Notify the TurnManager to end the player's turn and switch to enemy
        turnManager.EndPlayerTurn();
    }

    // Method to update the ingredient slots UI
    private void UpdateIngredientSlots()
    {
        // Clear ingredient slot UI images
        for (int i = 0; i < ingredientSlots.Length; i++)
        {
            if (i < currentIngredients.Count)
            {
                // Assuming ingredient has a sprite for UI
                ingredientSlots[i].sprite = currentIngredients[i].icon;  // Use ingredient icon
            }
            else
            {
                // Empty slot
                ingredientSlots[i].sprite = emptySlotSprite;
            }
        }
    }

    // Method to remove an ingredient (optional for UI updates or ingredient removal)
    public void RemoveIngredient(Ingredient ingredient)
    {
        if (currentIngredients.Contains(ingredient))
        {
            currentIngredients.Remove(ingredient);
            UpdateIngredientSlots();  // Update the UI slots after removal
        }
    }
}
