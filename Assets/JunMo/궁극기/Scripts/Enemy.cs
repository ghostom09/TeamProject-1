using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyTest : MonoBehaviour
{
    [SerializeField] private ParticleSystem mainParticle;
    [SerializeField] private ParticleSystem sparkParticle;
    [SerializeField] private SpriteRenderer enemy;
    [SerializeField]private GameObject ultra;
    [SerializeField] private ParticleSystem ultraSparkParticle;

    void Start()
    {
        ultra.SetActive(false);
        mainParticle.Stop();
        sparkParticle.Stop();
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
        ultraSparkParticle.Play();
        enemy.color = new Color(1f, 1f, 1f, 1f);

        yield return new WaitForSeconds(0.3f);
        
        ultra.SetActive(false);
        ultraSparkParticle.Stop();
        enemy.color = new Color(1f, 0f, 0f, 1f);
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
