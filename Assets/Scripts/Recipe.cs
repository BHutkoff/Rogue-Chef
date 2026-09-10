using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Cooking/Recipe")]
public class Recipe : ScriptableObject
{
    public string recipeName;  // Name of the recipe
    public Sprite recipeIcon;  // Optional UI icon
    public Ingredient[] requiredIngredients;  // List of required ingredients
    public string effectDescription;  // Description of the recipe’s effect

    // Special effects
    public int damage;  // Damage dealt by the recipe
    public int healing;  // Healing effect
    public bool appliesBurn;  // Does this recipe apply burn?
}
