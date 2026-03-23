using UnityEngine;
using System.Collections;

public class MagicianUltimateAttack : IBossSkillStrategy
{
    private float damage;
    private float attackRange;
    
    private float distSqr;
    
    private InputFilter inputFilter;
        
    public void Init(GameObject boss, BossSkills data, BossAttack bossAttack, GameObject hitArea, GameObject target)
    {
        inputFilter = target.GetComponent<InputFilter>();
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        boss.GetComponent<MonoBehaviour>().
            StartCoroutine(Attack(boss,  target, onComplete));
    }
    private IEnumerator Attack(GameObject boss, GameObject target, System.Action onComplete)
    {
        yield return new WaitForSeconds(4f);
        
        inputFilter.ActivateSkill(30f);
        
        EndAttack(null, onComplete);
    }
    
    public void EndAttack(GameObject hitArea, System.Action onComplete)
    {
        onComplete?.Invoke();
    }
}
