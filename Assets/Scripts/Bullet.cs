using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Components
    private SpriteRenderer sprite;
    private TrailRenderer trailRenderer;
    private Rigidbody2D rb;

    // Basic data
    protected double size = 1;      // in pxs
    protected double speed = 1;
    protected double ttl = 1;       // in seconds
    protected double damage = 40;
    protected double knockback = 3;
    protected Vector2 direction;
    
    // Special property
    public bool goThrough = false;

    public void Init(double size, double speed, double ttl, double damage, double knockback, Vector2 direction)
    {
        this.size = size;
        this.speed = speed;
        this.ttl = ttl;
        this.damage = damage;
        this.knockback = knockback;
        this.direction = direction;

        // Rotate the sprite.
        float angle = Vector2.SignedAngle(Vector2.right, direction);
        transform.eulerAngles = Vector3.forward * angle;
    }

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // Set renderers' size.
        float unitSize = (float)GameManager.PixelToUnit(size);
        sprite.size = Vector2.one * unitSize;
        trailRenderer.startWidth = unitSize * 0.9f;

        // Shoot the bullet.
        rb.velocity = direction.normalized * (float)speed;
    }

    // Update is called once per frame
    void Update()
    {
        // TTL check.
        ttl -= Time.deltaTime;
        if(ttl < 0)
            Destroy(gameObject);    // Destroy self.
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit detect.");
        if(other.gameObject.CompareTag("Enemy")) {
            Debug.Log("Hit enemy.");
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            enemy.HitBullet(damage, (float)knockback, rb.velocity.normalized);

            if(!goThrough)
                ttl = 0;
        }
    }
}
