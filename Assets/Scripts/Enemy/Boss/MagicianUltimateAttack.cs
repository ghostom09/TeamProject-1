using UnityEngine;
using System.Collections;

public class MagicianUltimateAttack : IBossSkillStrategy
{
    private PlayerInput playerInput;
    private const float CastDelay = 4f;
    private const float ShuffleDuration = 30f;
        
    public void Init(GameObject boss, BossSkills data, BossAttack bossAttack, GameObject target)
    {
        playerInput = FindPlayerInput(target);
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        boss.GetComponent<MonoBehaviour>().
            StartCoroutine(Attack(boss,  target, onComplete));
    }
    private IEnumerator Attack(GameObject boss, GameObject target, System.Action onComplete)
    {
        yield return new WaitForSeconds(CastDelay);

        if (playerInput == null)
            playerInput = FindPlayerInput(target);

        if (playerInput != null)
        {
            playerInput.ActivateInputShuffle(ShuffleDuration);
        }
        else
        {
            Debug.LogWarning("Magician ultimate could not find PlayerInput on the target.", boss);
        }
        
        EndAttack(null, onComplete);
    }
    
    public void EndAttack(GameObject hitArea, System.Action onComplete)
    {
        onComplete?.Invoke();
    }

    private PlayerInput FindPlayerInput(GameObject target)
    {
        if (target == null)
            return null;

        PlayerInput input = target.GetComponent<PlayerInput>();
        if (input != null)
            return input;

        input = target.GetComponentInParent<PlayerInput>();
        if (input != null)
            return input;

        return target.GetComponentInChildren<PlayerInput>();
    }
}
