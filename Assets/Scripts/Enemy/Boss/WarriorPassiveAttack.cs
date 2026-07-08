using UnityEngine;
using System.Collections;

public class WarriorPassiveAttack : IBossSkillStrategy
{
    private BossAttack bossAttack;
        
    public void Init(GameObject boss, BossSkills data, BossAttack bossAttack, GameObject target)
    {
        this.bossAttack = bossAttack;
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        boss.GetComponent<MonoBehaviour>().
            StartCoroutine(Attack(onComplete));
    }

    private IEnumerator Attack(System.Action onComplete)
    {
        Debug.Log("Warrior shield passive");
        
        yield return new WaitForSeconds(0.5f);
        bossAttack.Shield(3);

        EndAttack(null, onComplete);
    }
    
    public void EndAttack(GameObject hitArea, System.Action onComplete)
    {
        onComplete?.Invoke();
    }
}
