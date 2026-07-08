using UnityEngine;

public enum jobType
{
    Sword,
    Gun,
}
[CreateAssetMenu(menuName = "Character")]
public class CharacterData : ScriptableObject
{
    public jobType JobType;
    public float MoveSpeed;
    public float Damage;
    public float Hp;
    public float Range;
    public float AttackSpeed;
    public SkillData[] Skills;
    public UltData Ult;

    [Header("Animation")]
    public RuntimeAnimatorController AnimatorController;
    public Sprite DefaultSprite;
    public PlayerAnimationClips Animations;
    
    [Header("올라가는 스탯")]
    public float RisingMoveSpeed;
    public float RisingDamage;
    public float RisingRange;
    public float RisingAttackSpeed;
    public float RisingMaxHp;

}

[System.Serializable]
public class PlayerAnimationClips
{
    public AnimationClip Idle;
    public AnimationClip Run;
    public AnimationClip Attack;
    public AnimationClip Attack1;
    public AnimationClip Attack2;
    public AnimationClip Attack3;
    public AnimationClip Die;
    public AnimationClip JumpReady;
    public AnimationClip JumpUp;
    public AnimationClip JumpDown;

    public bool HasAnyClip()
    {
        return Idle != null
            || Run != null
            || Attack != null
            || Attack1 != null
            || Attack2 != null
            || Attack3 != null
            || Die != null
            || JumpReady != null
            || JumpUp != null
            || JumpDown != null;
    }

    public AnimationClip GetClip(string stateName)
    {
        switch (stateName)
        {
            case nameof(Idle):
                return Idle;
            case nameof(Run):
                return Run;
            case nameof(Attack):
                return Attack;
            case nameof(Attack1):
                return Attack1;
            case nameof(Attack2):
                return Attack2;
            case nameof(Attack3):
                return Attack3;
            case nameof(Die):
                return Die;
            case nameof(JumpReady):
                return JumpReady;
            case nameof(JumpUp):
                return JumpUp;
            case nameof(JumpDown):
                return JumpDown;
            default:
                return null;
        }
    }

    public AnimationClip GetClipOrFallback(string stateName)
    {
        AnimationClip clip = GetClip(stateName);

        if (clip != null)
            return clip;

        switch (stateName)
        {
            case nameof(Run):
                return Idle ?? GetFirstClip();
            case nameof(Attack):
                return GetAttackClip(0) ?? Idle ?? Run ?? GetFirstClip();
            case nameof(Die):
                return Idle ?? GetFirstClip();
            case nameof(JumpReady):
                return JumpUp ?? JumpDown ?? Idle ?? GetFirstClip();
            case nameof(JumpUp):
                return JumpReady ?? JumpDown ?? Idle ?? GetFirstClip();
            case nameof(JumpDown):
                return JumpUp ?? JumpReady ?? Idle ?? GetFirstClip();
            case nameof(Idle):
            default:
                return GetFirstClip();
        }
    }

    public bool HasAttackClip()
    {
        return Attack != null || Attack1 != null || Attack2 != null || Attack3 != null;
    }

    public AnimationClip GetAttackClip(int comboIndex)
    {
        switch (comboIndex)
        {
            case 0:
                return Attack1 ?? Attack ?? GetFirstAttackClip();
            case 1:
                return Attack2 ?? Attack1 ?? Attack ?? GetFirstAttackClip();
            case 2:
                return Attack3 ?? Attack2 ?? Attack1 ?? Attack ?? GetFirstAttackClip();
            default:
                return Attack ?? GetFirstAttackClip();
        }
    }

    public bool HasAnyJumpClip()
    {
        return JumpReady != null || JumpUp != null || JumpDown != null;
    }

    private AnimationClip GetFirstClip()
    {
        return Idle
            ?? Run
            ?? Attack
            ?? Attack1
            ?? Attack2
            ?? Attack3
            ?? Die
            ?? JumpReady
            ?? JumpUp
            ?? JumpDown;
    }

    private AnimationClip GetFirstAttackClip()
    {
        return Attack
            ?? Attack1
            ?? Attack2
            ?? Attack3;
    }
}
