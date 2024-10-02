using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 5;


    private bool isShooting;
    private Rigidbody2D body;
    private Animator animator;
    private SpriteRenderer sprRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");
        isShooting = Input.GetKey(KeyCode.Space);

        var direction = new Vector2(hAxis, vAxis);
        body.velocity = moveSpeed * (isShooting ? 0.5f : 1) * direction;

        /// Animation Control
        
        if(hAxis != 0 || vAxis != 0)
        {
            if (isShooting) animator.Play("PlayerWalk");
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
}
