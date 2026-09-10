using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    public TurnManager turnManager;  // Reference to the TurnManager

    // Start is called before the first frame update
    void Start()
    {
        // Add listener to the End Turn button
        GetComponent<Button>().onClick.AddListener(OnEndTurnClicked);
    }

    // Method to handle the button click
    private void OnEndTurnClicked()
    {
        turnManager.EndPlayerTurn();  // End the player's turn when the button is clicked
    }
}
