public class PlayerStats
{
    // Base
    private float baseMoveSpeed;
    private float baseDamage;
    private float baseHp;
    private float baseRange;
    private float baseAttackSpeed;

    // Modifier
    private float moveSpeedModifier = 1f;
    private float damageModifier = 1f;
    private float rangeModifier = 1f;
    private float attackSpeedModifier = 1f;

    // Current
    public float currentHp;

    public float MoveSpeed => baseMoveSpeed * moveSpeedModifier;
    public float Damage => baseDamage * damageModifier;
    public float Range => baseRange * rangeModifier;
    public float AttackSpeed => baseAttackSpeed * attackSpeedModifier;

    public void Init(CharacterData data)
    {
        baseMoveSpeed = data.MoveSpeed;
        baseDamage = data.Damage;
        baseHp = data.Hp;
        baseRange = data.Range;
        baseAttackSpeed = data.AttackSpeed;

        currentHp = baseHp;
    }

    // Modifier 추가
    public void AddMoveSpeed(float percent)
    {
        moveSpeedModifier += percent;
    }

    public void AddDamage(float percent)
    {
        damageModifier += percent;
    }

    public void AddAttackSpeed(float percent)
    {
        attackSpeedModifier += percent;
    }

    public void AddRange(float percent)
    {
        rangeModifier += percent;
    }
}