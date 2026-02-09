using UnityEngine;

public class Gun2 : SkillBase
{
    public Gun2() : base(25f)
    {
    }
    protected override void Execute(GameObject user, Vector2 dir)
    {
        Debug.Log("응 어쩔 트리거");
    }
}
