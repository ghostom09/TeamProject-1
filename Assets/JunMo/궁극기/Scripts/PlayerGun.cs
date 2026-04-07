using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] private GameObject ammoNormalPrefab;
    [SerializeField] private GameObject ammoGoldenPrefab;
    [SerializeField] private GameObject ammoUltraPrefab;

    [SerializeField]private GameObject ghostPrefab;
    [SerializeField]private float speed = 50f;
    private float maxDistance = 6f;
    private bool isTriggerUse = false;
    private GameObject player;
    private SpriteRenderer playerRenderer;
    
    
    public void NormalBullet(Vector2 origin, Vector2 dir)
    {
        GameObject bulletObj =
            ObjectPoolManager.Instance.Get
                (ObjectName.NormalBullet, origin, Quaternion.Euler(0f, 0f, Angle(dir)));

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.Initialize(dir, speed, maxDistance);
    }
    public void UltraBullet(Vector2 origin, Vector2 dir)
    {
        GameObject bulletObj = ObjectPoolManager.Instance.Get
            (ObjectName.UltraBullet, origin, Quaternion.Euler(0f, 0f, Angle(dir)));

        UltraBullet bullet = bulletObj.GetComponent<UltraBullet>();
        bullet.Initialize(dir, speed * 2, maxDistance * 2);
    }
    
    
    public void GoldenBullet(Vector2 pos, Vector2 dir)
    {
        GameObject bulletObj = ObjectPoolManager.Instance.Get
            (ObjectName.GoldenBullet, pos, Quaternion.Euler(0f, 0f, Angle(dir)));

        GoldenBullet bullet = bulletObj.GetComponent<GoldenBullet>();
        bullet.Initialize(dir, speed * 2, maxDistance * 2);
    }

    private float Angle(Vector2 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
    
    public void SpawnGhost(GameObject player, bool use)
    {
        isTriggerUse = use;
        
        this.player = player;
        playerRenderer = player.GetComponent<SpriteRenderer>();
        
        StartCoroutine(Ghost());
    }

    private IEnumerator Ghost()
    {
        WaitForSeconds wait = new WaitForSeconds(0.05f);

        while (isTriggerUse)
        {
            GameObject ghost = ObjectPoolManager.Instance.Get(ObjectName.Ghost, player.transform.position, Quaternion.identity);

            var ghostSr = ghost.GetComponent<SpriteRenderer>();

            ghostSr.sprite = playerRenderer.sprite;
            ghostSr.flipX = playerRenderer.flipX;

            yield return wait;
        }
    }
}