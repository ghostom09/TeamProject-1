using System;
using UnityEngine;
using System.Collections.Generic;

public class SkillController : MonoBehaviour
{
    public static SkillController Instance;
    
    [SerializeField]private ParadoxController paradoxController;
    [SerializeField] private PlayerGun playerGun;
    [SerializeField] private PlayerSword playerSword;
    [SerializeField]private CharacterData charData;
    
    public bool usingUltra = false;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        paradoxController.ResetUltra();
        
        // charData = CharDataManager.Instance.data;
    }

    public void Gun1(Vector2 pos, Vector2 dir)
    {
        playerGun.GoldenBullet(pos, dir);
    }

    public void Gun1(Vector2 pos, Vector2 dir, float distance, bool explodeAtMaxDistance)
    {
        playerGun.GoldenBullet(pos, dir, distance, explodeAtMaxDistance);
    }

    public void Gun1(Vector2 pos, Vector2 dir, float distance, bool explodeAtMaxDistance, float boomScaleDistance)
    {
        playerGun.GoldenBullet(pos, dir, distance, explodeAtMaxDistance, boomScaleDistance);
    }
    
    public void Gun2(GameObject player, bool use)
    {
        playerGun.SpawnGhost(player, use);
    }
    
    public void GunUltra()
    {
        PressUltra();
    }
    
    public void SwordUltra(Vector2 pos)
    {
        playerSword.Ultimate(pos);
    }

    void PressUltra()
    {
        if (!usingUltra)
        {
            UseUltra();
        }
        else
        {
            OffUltra();
        }
    }

    private void UseUltra()
    {
        paradoxController.SetVolumeActive(true);
        usingUltra = !usingUltra;
    }

    private void OffUltra()
    {
        paradoxController.SetVolumeActive(false);
        usingUltra = !usingUltra;
    }

    public void GunNormalAttack(Vector2 origin, Vector2 dir, float distance)
    {
        playerGun.NormalBullet(origin, dir, distance);
    }

    public void GunUltraAttack(Vector2 origin, Vector2 dir)
    {
        playerGun.UltraBullet(origin, dir);
    }
}
