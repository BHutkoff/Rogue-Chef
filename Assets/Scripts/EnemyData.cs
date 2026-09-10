using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;     // Name of the enemy
    public int maxHealth;       // Maximum health
    public int burnDamage;      // Burn damage applied each turn
    public float burnDuration;  // Duration of the burn effect (in turns)

    // Array of attacks for the enemy
    public Attack[] attacks;
}

[System.Serializable]
public class Attack
{
    public string attackName;  // Name of the attack (e.g., Fireball)
    public int damage;         // Damage dealt by the attack
    public float cooldown;     // Time between consecutive uses of this attack (in seconds)
}
