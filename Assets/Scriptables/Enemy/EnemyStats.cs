using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats")]

public class EnemyStats : ScriptableObject
{
    public float health;
    public float speed;
    public float damage;
    public float attackSpeed;
    public float attackRange;
}
