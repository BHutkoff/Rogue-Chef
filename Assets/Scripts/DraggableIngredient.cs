using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableIngredient : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;  // Reference to the Canvas
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;  // CanvasGroup (Optional)
    
    public Ingredient ingredient;  // The ingredient this button represents
    public CookingPot cookingPot;  // Reference to the CookingPot

    private Vector3 originalPosition;  // To store the ingredient's starting position

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();  // Get CanvasGroup component (may be missing)
        canvas = GetComponentInParent<Canvas>();  // Assuming this script is attached to a UI element inside a Canvas
    }

    private void Start()
    {
        // Ensure that cookingPot is assigned
        if (cookingPot == null)
        {
            Debug.LogError("CookingPot is not assigned in DraggableIngredient!", this);
        }

        // Save the original position when the ingredient is created
        originalPosition = rectTransform.position;

        // Ensure that ingredient is assigned
        if (ingredient == null)
        {
            Debug.LogError("Ingredient is not assigned in DraggableIngredient!", this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)  // Only modify CanvasGroup if it exists
        {
            canvasGroup.alpha = 0.6f;  // Make the ingredient semi-transparent when dragging
            canvasGroup.blocksRaycasts = false;  // Disable raycasting to allow dropping
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;  // Adjust for canvas scaling
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)  // Restore UI properties
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        // Cast a ray from the mouse position to detect if it hits the CookingPot collider
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        Collider2D potCollider = cookingPot.GetComponent<Collider2D>();  // Get the collider of the cooking pot
        if (potCollider != null && potCollider.OverlapPoint(mousePosition))  // Check if dropped on pot
        {
            Debug.Log("Ingredient dropped on Cooking Pot");

            if (cookingPot != null && ingredient != null)
            {
                Debug.Log($"Adding ingredient: {ingredient.ingredientName} to pot");
                cookingPot.AddIngredient(ingredient);  // Add the ingredient to the pot

                // Return the ingredient to its original position even if it was successfully added
                ReturnToOriginalPosition();
            }
            else
            {
                if (cookingPot == null) Debug.LogError("CookingPot is null in OnEndDrag!");
                if (ingredient == null) Debug.LogError("Ingredient is null in OnEndDrag!");
            }
        }
        else
        {
            Debug.Log("Ingredient not dropped on Cooking Pot");
            ReturnToOriginalPosition();
        }
    }


    // Move the ingredient back to its original position
    private void ReturnToOriginalPosition()
    {
        rectTransform.position = originalPosition;
    }
}
