using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Components
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    // Enemy basic data.
    public float targetSpeed = 0.5f;

    // Physics parameters.
    public float speedUpForce = 3f;
    public float dragForce = 2f;


    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Get direction.
        var direction = -((Vector2)transform.position - GameManager.GetPlayerPosition()).normalized;
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
}
