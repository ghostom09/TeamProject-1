using UnityEngine;

public class Sword1 : SkillBase
{
    public Sword1() : base(3f)   
    {
    }
    protected override void Execute(GameObject user, Vector2 dir)
    {
        Debug.Log("신법ㅋ");
    }
}
