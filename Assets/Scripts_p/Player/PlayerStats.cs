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

    // Flat Add
    private float bonusHp = 0f;

    // Current
    public float currentHp;

    public float MoveSpeed    => baseMoveSpeed    * moveSpeedModifier;
    public float Damage       => baseDamage       * damageModifier;
    public float Range        => baseRange        * rangeModifier;
    public float AttackSpeed  => baseAttackSpeed  * attackSpeedModifier;
    public float MaxHp        => baseHp + bonusHp;

    public void Init(CharacterData data)
    {
        baseMoveSpeed = data.MoveSpeed;
        baseDamage    = data.Damage;
        baseHp        = data.Hp;
        baseRange     = data.Range;
        baseAttackSpeed = data.AttackSpeed;

        currentHp = MaxHp;
    }

    // Modifier 추가
    public void AddMoveSpeed(float percent)    { moveSpeedModifier   += percent; }
    public void AddDamage(float percent)       { damageModifier      += percent; }
    public void AddAttackSpeed(float percent)  { attackSpeedModifier += percent; }
    public void AddRange(float percent)        { rangeModifier       += percent; }

    // Flat HP 증가 (Health 스탯)
    public void AddMaxHp(float flat)
    {
        bonusHp   += flat;
        currentHp += flat;
    }
    public void Heal(float amount)
    {
        currentHp = UnityEngine.Mathf.Min(currentHp + amount, MaxHp);
    }
    public void ApplyStat(ArgumentResult result)
    {
        switch (result.statType)
        {
            case StatType.Health:      AddMaxHp(result.statValue);        break;
            case StatType.Attack:      AddDamage(result.statValue);       break;
            case StatType.AttackSpeed: AddAttackSpeed(result.statValue);  break;
            case StatType.Speed:       AddMoveSpeed(result.statValue);    break;
            case StatType.Range:       AddRange(result.statValue);        break;
            case StatType.Heal:        Heal(result.statValue);            break;
        }
    }
}