using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Needed for Select()

[System.Serializable]
public class RecipeDatabase : MonoBehaviour
{
    [SerializeField]  // Ensure Unity shows this in the Inspector
    private List<Recipe> allRecipes = new List<Recipe>(); 

    public static RecipeDatabase Instance { get; private set; }  // Singleton for easy access

    private void Awake()
    {
        // Ensure only one instance of RecipeDatabase exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Log to check if recipes are loaded
        Debug.Log($"Recipe Database initialized with {allRecipes.Count} recipes.");

        foreach (Recipe recipe in allRecipes)
        {
            if (recipe == null)
            {
                Debug.LogError("RecipeDatabase contains a null recipe entry!");
                continue;
            }

            Debug.Log($"Recipe: {recipe.recipeName}, Required Ingredients: {string.Join(", ", recipe.requiredIngredients.Select(i => i != null ? i.ingredientName : "NULL"))}");
        }
    }

    public Recipe GetMatchingRecipe(List<Ingredient> ingredients)
    {
        if (ingredients == null || ingredients.Count == 0)
        {
            Debug.Log("No ingredients in the pot. Skipping recipe check.");
            return null;
        }

        Debug.Log($"Checking for matching recipe. Ingredients in pot: {string.Join(", ", ingredients.ConvertAll(i => i.ingredientName))}");

        foreach (Recipe recipe in allRecipes)
        {
            if (recipe == null)
            {
                Debug.LogError("Encountered a null recipe in RecipeDatabase!");
                continue;
            }

            Debug.Log($"Checking recipe: {recipe.recipeName} with required ingredients: {string.Join(", ", recipe.requiredIngredients.Select(i => i != null ? i.ingredientName : "NULL"))}");

            if (IsRecipeMatch(recipe, ingredients))
            {
                Debug.Log($"Match found: {recipe.recipeName}");
                return recipe;
            }
        }

        Debug.Log("No recipe matches the given ingredients.");
        return null;
    }

    private bool IsRecipeMatch(Recipe recipe, List<Ingredient> ingredients)
    {
        Debug.Log($"Comparing recipe {recipe.recipeName} with pot ingredients...");

        if (recipe.requiredIngredients.Length != ingredients.Count)
        {
            Debug.Log($"Recipe {recipe.recipeName} rejected: ingredient count does not match ({recipe.requiredIngredients.Length} needed, {ingredients.Count} provided).");
            return false;
        }

        // Convert both lists to lowercase ingredient names for comparison
        List<string> potIngredientNames = ingredients
            .ConvertAll(i => i.ingredientName.ToLower().Trim());  

        List<string> recipeIngredientNames = recipe.requiredIngredients
            .Select(i => i.ingredientName.ToLower().Trim())
            .ToList();

        Debug.Log($"Pot Ingredients: {string.Join(", ", potIngredientNames)}");
        Debug.Log($"Recipe Ingredients: {string.Join(", ", recipeIngredientNames)}");

        // Sort both lists to allow unordered comparison
        potIngredientNames.Sort();
        recipeIngredientNames.Sort();

        bool isMatch = potIngredientNames.SequenceEqual(recipeIngredientNames);
        Debug.Log($"Ingredients match: {isMatch}");

        if (isMatch)
        {
            Debug.Log($"Recipe {recipe.recipeName} is a match!");
            return true;
        }
        else
        {
            Debug.Log($"Recipe {recipe.recipeName} rejected: ingredient mismatch.");
            return false;
        }
    }
}
