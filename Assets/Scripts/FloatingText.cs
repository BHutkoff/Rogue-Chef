using UnityEngine;
using TMPro;
using System.Collections; //for IEnumerator as it requires the Systems.Collections namespace


public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float moveSpeed = 1f;  // Speed at which the text moves
    public float fadeDuration = 1f;  // Duration for fading out

    private void Awake()
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }
    }

    public void SetText(string text)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
            Debug.Log($"Text set to: {text}");
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found!");
        }

        // Start the floating text animation
        StartCoroutine(FloatingAndFading());
    }

    public void SetColor(Color color)
    {
        if (textMesh != null)
        {
            textMesh.color = color;
            Debug.Log($"Text color set to: {color}");
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found!");
        }
    }

    private IEnumerator FloatingAndFading()
    {
        Vector3 startPosition = transform.position;

        // Move the text upwards
        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            transform.position = startPosition + new Vector3(0, moveSpeed * timeElapsed, 0);
            float alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration);
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // After fading out, destroy the text object
        Destroy(gameObject);
    }
}
