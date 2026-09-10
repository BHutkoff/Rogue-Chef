using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance; // Singleton instance
    public GameObject floatingTextPrefab;  // Reference to the prefab
    public Transform canvasTransform;  // Reference to the canvas

    private void Awake()
    {
        // Ensure there is only one instance of the FloatingTextManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateFloatingText(string text, Vector3 worldPosition, Color color)
    {
        // Convert world position to screen space
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // Log to verify conversion
        Debug.Log($"World Position: {worldPosition}, Screen Position: {screenPosition}");

        if (floatingTextPrefab != null && canvasTransform != null)
        {
            // Instantiate the floating text prefab
            GameObject floatingTextObject = Instantiate(floatingTextPrefab, screenPosition, Quaternion.identity);
            floatingTextObject.transform.SetParent(canvasTransform, false);

            // Access the FloatingText script and set the properties directly
            FloatingText floatingText = floatingTextObject.GetComponent<FloatingText>();
            if (floatingText != null)
            {
                floatingText.SetText(text); // Assign text
                floatingText.SetColor(color); // Assign color
            }
            else
            {
                Debug.LogError("FloatingText script not found on the prefab!");
            }
        }
        else
        {
            Debug.LogError("FloatingTextPrefab or CanvasTransform is not assigned!");
        }
    }


}
