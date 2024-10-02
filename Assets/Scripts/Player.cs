using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 5;

    // Components
    private Rigidbody2D body;
    private Animator animator;
    private SpriteRenderer sprRenderer;

    // Priavte state stuff
    private bool triggering = false;
    private Weapon currentWeapon = null;

    // Accessor
    public bool Triggering { get => triggering; }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        /// Handle Input
        
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");
        triggering = Input.GetAxisRaw("Fire1") > 0;

        /// Walking judge
        bool isWalking = currentWeapon.Shooting;

        var direction = new Vector2(hAxis, vAxis);
        body.velocity = moveSpeed * (isWalking ? 0.5f : 1) * direction;

        /// Animation Control
        if(hAxis != 0 || vAxis != 0)
        {
            if (isWalking) animator.Play("PlayerWalk");
            else animator.Play("PlayerRun");
        }
        else
        {
            animator.Play("PlayerIdle");
        }

        /// Sprite Flip Control
        
        if(hAxis != 0)
        {
            sprRenderer.flipX = hAxis < 0;
        }
    }

    public void RegisterWeapon(Weapon weapon) {
        currentWeapon = weapon;

        Debug.Log("Weapon registered.");
    }
}
