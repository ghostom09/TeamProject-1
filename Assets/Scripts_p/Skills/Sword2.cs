using Mono.Cecil.Cil;
using UnityEngine;

public class Sword2 : SkillBase
{
    public Sword2() : base(5f)
    {
    }
    protected override void Execute(GameObject user, Vector2 dir)
    {
        Debug.Log("반인호ㅋ");
    }
}
