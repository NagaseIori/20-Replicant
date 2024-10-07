using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Components
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    // Enemy basic data.
    public float targetSpeed = 0.5f;
    public double maxHealth = 40f;
    public float flashDuration = 0.2f;

    // Physics parameters.
    public float speedUpForce = 3f;
    public float dragForce = 2f;
    
    // Enemy state.
    protected double health = 40f;

    // Materials
    private Material blinkMaterial;
    private Material originalMaterial;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        blinkMaterial = new Material(Shader.Find("Custom/Sprites/Colorized"));
        blinkMaterial.color = Color.white;
        originalMaterial = sprite.material;

        // State init.
        health = maxHealth;

        // Events registration.
        Player player = GameManager.GetPlayer();
        if(player != null) {
            player.PlayerDamaged += OnPlayerDamaged;
        }
        else {
            Debug.LogWarning("Player not found.");
        }
    }

    void OnDestroy()
    {
        Player player = GameManager.GetPlayer();
        if(player != null)
            player.PlayerDamaged -= OnPlayerDamaged;
    }

    public void TriggerFlash() {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine() {
        sprite.material = blinkMaterial;
        yield return new WaitForSeconds(flashDuration);
        sprite.material = originalMaterial;
    }

    void Update()
    {
        // Get direction.
        var direction = -((Vector2)transform.position - GameManager.GetPlayerPosition()).normalized;
        if(!GameManager.IsPlayerAlive())
            direction *= -1;
        
        var targetVelocity = direction * targetSpeed;


        if((targetVelocity - rb.velocity).magnitude < 0.01f) {
            // Constant motion.
            rb.drag = 0;
        }
        else {
            rb.drag = dragForce;
            if(rb.velocity.magnitude < targetSpeed)
                rb.AddForce(direction * speedUpForce);
        }

        // Sprite control.
        sprite.flipX = direction.x < 0;
    }

    void Death() {
        // Destroy self.
        Destroy(gameObject);
    }

    void Damage(double damage) {
        health -= damage;
        if(health <= 0)
            Death();
    }

    // Hit by bullet.
    public void HitBullet(double damage, float knockback, Vector2 direction) {
        Damage(damage);
        // Apply knockback force.
        rb.AddForce(direction.normalized * knockback, ForceMode2D.Impulse);
        TriggerFlash();
    }

    public void OnPlayerDamaged(Vector2 playerPosition, float knockback) {
        Vector2 dir = (Vector2)transform.position - playerPosition;
        rb.AddForce(knockback * Mathf.Min(1, 1 / dir.magnitude) * dir.normalized, ForceMode2D.Impulse);
    }
}
