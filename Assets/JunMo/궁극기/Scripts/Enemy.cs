using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyTest : MonoBehaviour
{
    [SerializeField] private ParticleSystem mainParticle;
    [SerializeField] private ParticleSystem sparkParticle;
    [SerializeField] private SpriteRenderer enemy;
    [SerializeField]private GameObject ultra;
    [SerializeField] private ParticleSystem ultraSparkParticle;
    [SerializeField] private GameObject player;
    
    public float offsetDistance = 1f;

    void Start()
    {
        ultra.SetActive(false);
        mainParticle.Stop();
        sparkParticle.Stop();
        ultraSparkParticle.Stop();
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) //충돌시 호출할것
        {
            if (UltraVFXController.usingUltra)
            {
                UltraAttacked();
            }
            else
            {
                Attacked();
            }
        }
    }

    void UltraAttacked()
    {
        StartCoroutine(UltraAttack());
    }

    private IEnumerator UltraAttack()
    {
        ultra.SetActive(true);
        SpawnHitEffect(player.transform, gameObject.transform);
        enemy.color = new Color(1f, 1f, 1f, 1f);

        yield return new WaitForSeconds(0.3f);
        
        ultra.SetActive(false);
        enemy.color = new Color(1f, 0f, 0f, 1f);
    }

    public void SpawnHitEffect(Transform attacker, Transform target)
    {
        Vector3 hitDirection = (target.position - attacker.position).normalized;
        Vector3 effectDirection = hitDirection;
    
        Vector3 effectPosition = target.position + effectDirection * offsetDistance;
    
        ParticleSystem ps = Instantiate(ultraSparkParticle, effectPosition, Quaternion.identity);
    
        var shape = ps.shape;
        ps.Play();
    
        Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
    }

    void Attacked()
    {
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        mainParticle.Play();
        sparkParticle.Play();
        enemy.color = new Color(1f, 0f, 0f, 1f);

        yield return new WaitForSeconds(0.4f);
        
        mainParticle.Stop();
        sparkParticle.Stop();
        enemy.color = new Color(1f, 1f, 1f, 1f);
    }

    public void UseUltra(bool enable)
    {
        if (enable)
        {
            enemy.color = new Color(1f, 0f, 0f, 1f);
        }
        else
        {
            enemy.color = new Color(1f, 1f, 1f, 1f);
        }
    }
}
