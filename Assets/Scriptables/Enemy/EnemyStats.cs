using UnityEngine;

public enum EnemyType
{
    None,
    normal,
    tanker,
    ranged,
    support
}

[CreateAssetMenu(fileName = "EnemyStats")]

public class EnemyStats : ScriptableObject
{
    public EnemyType enemyType;
    public float health;
    public float speed;
    public float damage;
    public float attackRange;
    public float attackSpeed;
    public float jumpForce;
}
