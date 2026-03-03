using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] private GameObject ammoNormalPrefab;
    [SerializeField] private GameObject ammoGoldenPrefab;
    [SerializeField] private GameObject ammoUltraPrefab;
    
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private float speed = 50f;
    private float maxDistance = 6f;

    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void OnEnable()
    {
        playerInput.onAttack += Attack;
    }

    void OnDisable()
    {
        playerInput.onAttack -= Attack;
    }

    (Vector2 dir, float angle) MousePosition()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);

        Vector2 dir = (mouseWorld - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        return (dir, angle);
    }

    //GunNormalAttack TryAttack Shoot다음에 넣기
    void Attack() //평타
    {
        (Vector2 dir, float angle) = MousePosition();
        if (true)
        {
            GoldenBullet();
            // NormalBullet(dir, angle);
        }
        else if(true)
        {
            UltraBullet(dir, angle);
        }
    }

    void NormalBullet(Vector2 dir, float angle)
    {
        // Vector2 endPoint = hit
        //     ? hit.point
        //     : origin + dir.normalized * data.Range * 2;
        GameObject bulletObj = Instantiate(
            ammoNormalPrefab,
            transform.position,
            Quaternion.Euler(0f, 0f, angle)
        );

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.Initialize(dir, speed, maxDistance);
    }
    void UltraBullet(Vector2 dir, float angle)
    {
        GameObject bulletObj = Instantiate(
            ammoUltraPrefab,
            transform.position,
            Quaternion.Euler(0f, 0f, angle)
        );

        UltraBullet bullet = bulletObj.GetComponent<UltraBullet>();
        bullet.Initialize(dir, speed * 2, maxDistance * 2);
    }
    
    // golden Shot
    
    void GoldenBullet()
    {
        // Vector2 endPoint = hit
        //     ? hit.point
        //     : origin + dir.normalized * data.Range * 2;
        (Vector2 dir, float angle) = MousePosition();
        
        GameObject bulletObj = Instantiate(
            ammoGoldenPrefab,
            transform.position,
            Quaternion.Euler(0f, 0f, angle)
        );

        GoldenBullet bullet = bulletObj.GetComponent<GoldenBullet>();
        bullet.Initialize(dir, speed * 2, maxDistance * 2);
    }

    
    //trigger
    
    [SerializeField]private GameObject ghostPrefab;
    
    void SpawnGhost()
    {
        GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);

        SpriteRenderer ghostSr = ghost.GetComponent<SpriteRenderer>();
        SpriteRenderer playerSr = GetComponent<SpriteRenderer>();

        ghostSr.sprite = playerSr.sprite;
        ghostSr.flipX = playerSr.flipX;
    }
}