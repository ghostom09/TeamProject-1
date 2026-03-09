using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
// using DG.Tweening;

public class PlayerSword : MonoBehaviour
{
    [SerializeField] private CameraMove cam;
    [SerializeField] private GameObject Illusions;
    [SerializeField] private SpriteRenderer me;
    [SerializeField] private TrailRenderer trail;

    // public void SpawnIllusions(float damage)
    // {
    //     ClearIllusions();
    //
    //     int count = 3;
    //     float radius = 1.5f;
    //     float totalAngle = 80f;
    //
    //     for (int i = 0; i < count; i++)
    //     {
    //         float angle = (i - (count - 1) / 2f) * (totalAngle / (count - 1));
    //         
    //         float radian = (angle + 90f) * Mathf.Deg2Rad;
    //         Vector3 offset = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0) * radius;
    //         
    //         Vector3 spawnPos = transform.position + offset;
    //         
    //         Quaternion spawnRot = Quaternion.Euler(0, 0, angle);
    //
    //         GameObject obj = Instantiate(illusionPrefab, spawnPos, spawnRot);
    //
    //         var illusion = obj.GetComponent<SwordIllusionProjectile>();
    //         illusion.Init(damage); 
    //
    //         _illusions.Add(illusion);
    //     }
    // }
    
    // public PlayableDirector timeline;          // Timeline 컴포넌트
    public ParticleSystem auraParticle;         // 오라
    public ParticleSystem sparkPrefab;          // 히트 스파크 프리팹
    public GameObject bigSwordPrefab;           // 큰 검 프리팹
    public float ultDamage = 570f;              // 마지막 데미지
    // public LayerMask enemyLayer;

    private Animator animator;
    private bool isUlting = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerUltimate()  // 게이지 80 달성 시 호출
    {
        if (isUlting) return;
        isUlting = true;

        auraParticle.Play();
    }

    // Timeline Signal Receiver 예시 (중간 슬래시)
    public void SpawnSlashParticles(int count = 10)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = -60f + i * 12f;  // 120도 팬
            Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.right;
            var spark = Instantiate(sparkPrefab, transform.position + dir * 1.5f, Quaternion.identity);
            spark.Play();

            // DOVirtual.DelayedCall(i * 0.13f, () => { /* 사운드 */ });
        }
    }

    // 마지막 휘두르기 Signal
    public void BigSwordStrike()
    {
        GameObject sword = Instantiate(bigSwordPrefab, transform.position + Vector3.up * 3f, Quaternion.identity);
        // sword.transform.DOMove(transform.position + Vector3.right * 5f, 0.4f).SetEase(Ease.InOutSine);

        // 쉐이크 + 플래시
        // Camera.main.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
    }

    // Timeline 끝 이벤트
    public void EndUltimate()
    {
        isUlting = false;
        auraParticle.Stop();
        // 무적 OFF
        animator.Play("Idle");
    }
    
    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            // ActivateUltimate();
        }
    }
}
