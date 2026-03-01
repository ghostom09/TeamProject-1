using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] private GameObject ammoPrefab;
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private float speed = 50f;
    [SerializeField] private float maxDistance = 6f;

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

    void Attack()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);

        Vector2 dir = (mouseWorld - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject bulletObj = Instantiate(
            ammoPrefab,
            transform.position,
            Quaternion.Euler(0f, 0f, angle)
        );

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.Initialize(dir, speed * 2, maxDistance * 2, true); // false = 일반 공격
    }
    
    //trigger
    
    [SerializeField]private GameObject ghostPrefab;
    public float spawnInterval = 0.05f;
    void SpawnGhost()
    {
        GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);

        SpriteRenderer ghostSr = ghost.GetComponent<SpriteRenderer>();
        SpriteRenderer playerSr = GetComponent<SpriteRenderer>();

        ghostSr.sprite = playerSr.sprite;
        ghostSr.flipX = playerSr.flipX;
    }
}