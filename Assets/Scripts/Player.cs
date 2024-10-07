using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 5;

    // Components
    private Rigidbody2D body;
    private Animator animator;
    private SpriteRenderer sprite;
    public float flashDuration = 1f;

    // Player basic data
    public int healthMax = 100;
    public double abbyROFSpeed = 0.8;
    public float damagedKnockback = 4;
    public Color damagedBlinkColor = Color.white;

    // Player current states
    protected bool triggering = false;
    protected bool specialAbby = false;
    protected Weapon currentWeapon = null;
    protected int healthPoint;

    // Accessor
    public bool Triggering { get => triggering || specialAbby; }
    public bool Abbying { get => specialAbby; }

    // Events
    public delegate void onPlayerDamaged(Vector2 playerPosition, float knockback);
    public event onPlayerDamaged PlayerDamaged;

    // Materials
    private Material blinkMaterial;
    private Material originalMaterial;

    public void TriggerFlash() {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine() {
        sprite.material = blinkMaterial;
        yield return new WaitForSeconds(flashDuration);
        sprite.material = originalMaterial;
    }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        blinkMaterial = new Material(Shader.Find("Custom/Sprites/Colorized"));
        blinkMaterial.color = damagedBlinkColor;
        originalMaterial = sprite.material;

        // Init player's states
        healthPoint = healthMax;
    }

    // Abby stuff.
    void AbbyGo() {
        specialAbby = true;
        currentWeapon.Invisiblize();
    }
    void AbbyStop() {
        specialAbby = false;
        currentWeapon.Visiblize();
    }
    void AbbyCheck() {
        if(currentWeapon.NoAmmo) {
            AbbyStop();
            return;
        }
        if(!specialAbby && Input.GetMouseButtonDown(1))
            AbbyGo();
    }

    // Update is called once per frame
    void Update()
    {
        // Handle Input
        
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");
        triggering = Input.GetAxisRaw("Fire1") > 0;
        AbbyCheck();

        // Walking judge

        bool isWalking = currentWeapon.Shooting;

        var direction = new Vector2(hAxis, vAxis);
        body.velocity = moveSpeed * (isWalking ? 0.5f : 1) * direction;

        // Animation Control
        
        if(Abbying)
            animator.Play("PlayerAbbySpecial");
        else if(hAxis != 0 || vAxis != 0)
        {
            if (isWalking) animator.Play("PlayerWalk");
            else animator.Play("PlayerRun");
        }
        else
        {
            animator.Play("PlayerIdle");
        }


        // Sprite Flip Control
        
        if(hAxis != 0)
        {
            sprite.flipX = hAxis < 0;
        }
    }

    public void RegisterWeapon(Weapon weapon) {
        if(currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
            Debug.Log("Weapon destroyed.");
        }

        currentWeapon = weapon;

        Debug.Log("Weapon registered.");
    }

    // Player gets damaged.
    public void Damage(int damage) {
        healthPoint -= damage;
        AbbyStop();     // Abby interrupt.
        TriggerFlash();
        if(healthPoint == 0) {
            // Gameover.
            Destroy(gameObject);
        }
        else {
            PlayerDamaged(transform.position, damagedKnockback);
        }
    }

    // Player gets hit.
    public void Hit() {
        Damage(1);
    }

    // Collisons methods
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Enemy")) {
            Hit();
        }
    }
}
