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
        paradoxController.SetVolumeActive(true);
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
        
    }

    private void OffUltra()
    {
        paradoxController.SetVolumeActive(false);
        usingUltra = false;
    }

    public void GunNormalAttack(GameObject user, Vector2 dir)
    {
        playerGun.NormalBullet(user, dir);
    }

    public void GunUltraAttack(Player user, Vector2 dir)
    {
        playerGun.UltraBullet(user, dir);
    }
}
