using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private GameObject player;
    
    private Rigidbody2D rb2d;
    private BoxCollider2D collider;
    private float speed;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        speed = GetComponent<Enemy>().stats.speed;
    }

    private void Update()
    {
        horizontalmove();
    }
    
    private void verticalmove()
    {
        
    }
    private void horizontalmove()
    {
        if (player.transform.position.x < transform.position.x)
        {
            rb2d.linearVelocity = new Vector2(-speed, 0);
        }
        else if (player.transform.position.x > transform.position.x)
        {
            rb2d.linearVelocity = new Vector2(speed, 0);
        }
    }
}
