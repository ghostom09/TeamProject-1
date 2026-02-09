using UnityEngine;

public class Gun1 : SkillBase
{
    public Gun1() : base(4f)
    {
    }
    protected override void Execute(GameObject user, Vector2 dir)
    {
        Debug.Log("골든샷이나 쳐먹어라ㅏㅏㅏ");
    }
}
