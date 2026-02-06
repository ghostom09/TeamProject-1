using UnityEditor.Timeline.Actions;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/ShinBub")]
public class ShinBubSkill : SkillData
{
    public float damage;
    public override void Execute(GameObject owner, Vector2 mouseAngle)
    {
        RaycastHit2D hit = Physics2D.Raycast(mouseAngle, mouseAngle, LayerMask.GetMask("Enemy"));
        if (hit.collider != null)
        {
            owner.transform.position = hit.point;
        }
    }
    
}
