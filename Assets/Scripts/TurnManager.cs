using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public Player player;
    public Enemy enemy;
    private bool isPlayerTurn = true;

    public CookingPot cookingPot;  // Reference to the CookingPot script

    private void Start()
    {
        // Start with the player's turn
        StartPlayerTurn();
    }

    // Called when it's the player's turn
    private void StartPlayerTurn()
    {
        // Apply burn damage to the player at the start of their turn
        player.ApplyBurnDamage();
        
        // Any other player actions here (add ingredients, attack, etc.)
        Debug.Log("Player's Turn: Start.");
    }

    // Called when it's the enemy's turn
    private void StartEnemyTurn()
    {
        // Apply burn damage to the enemy at the start of their turn
        enemy.ApplyBurnDamage();
        
        // Perform enemy actions (attack, etc.)
        enemy.PerformTurnAction(player);

        // End enemy turn after performing actions
        EndEnemyTurn();
    }

    // Called when the player ends their turn, this method ends the player's turn
    // and transitions to the enemy's turn.
    public void EndPlayerTurn()
{
    // End the player's turn and process cooking if necessary
    isPlayerTurn = false;

    //Debug.Log("Player's turn is over. Triggering end turn cooking...");

    // Ensure cooking triggers even if fewer than 5 ingredients are in the pot
    if (cookingPot != null)
    {
        cookingPot.OnEndTurn();
    }
    else
    {
        Debug.LogError("CookingPot reference is missing in TurnManager!");
    }

    // Reset cooking state at the end of the turn
    cookingPot.ResetCookingState();

    // Transition to enemy's turn
    StartEnemyTurn();
}


    // Called when the enemy ends their turn, this method ends the enemy's turn
    // and transitions back to the player's turn.
    public void EndEnemyTurn()
    {
        // End the enemy's turn and switch to player's turn
        isPlayerTurn = true;
        cookingPot.ResetCookingState();  // Reset cooking state at the end of the turn

        Debug.Log("Enemy's turn is over.");
        StartPlayerTurn();  // Transition back to player's turn
    }

    // This function allows for automatic switching between turns
    public void EndTurn()
    {
        if (isPlayerTurn)
        {
            EndPlayerTurn(); // Player ends turn
        }
        else
        {
            EndEnemyTurn(); // Enemy ends turn
        }
    }
}
