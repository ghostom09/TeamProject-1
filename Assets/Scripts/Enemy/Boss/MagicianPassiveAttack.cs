using UnityEngine;
using System.Collections;

public class MagicianPassiveAttack : IBossSkillStrategy
{
    private Vector2 dir;
    private BossAttack bossAttack;
    private EnemySpawnerManager spawnerManager;
    private Vector2 spawnoffset;
    private const float CastDelay = 0.2f;
    private const float MagicCircleDuration = 1f;
    public void Init(GameObject boss, BossSkills data, BossAttack bossAttack, GameObject target)
    {
        this.bossAttack = bossAttack;
        spawnerManager = boss.GetComponent<Boss>().manager;
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        boss.GetComponent<MonoBehaviour>().
            StartCoroutine(AttackRoutine(boss, target, onComplete));
    }

    private IEnumerator AttackRoutine(GameObject boss, GameObject target, System.Action onComplete)
    {
        yield return new WaitForSeconds(CastDelay);
        for (int i = 0; i < 3; i++)
        {
            spawnoffset = boss.transform.position;
            spawnoffset.x = Random.Range(spawnoffset.x-2, spawnoffset.x+2);
            bossAttack.SpawnMagicCircle(BossSkillType.passive, spawnoffset, 1f, MagicCircleDuration);
            spawnerManager.SpawnFromPoint(spawnoffset, true);
        }
        EndAttack(null, onComplete);
    }

    public void EndAttack(GameObject hitArea, System.Action onComplete)
    {
        onComplete?.Invoke();
    }
}
