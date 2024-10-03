using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Animator animator;

    private bool reloading = false;

    // Parent player
    private Player player;

    // Weapon basic data
    private readonly double rateOfFire = 0.2;       // in seconds
    private readonly double timeToReload = 2;       // in seconds
    private readonly int magazineSize = 10;
    private readonly double bulletVelocity = 1;     // ups
    private readonly double bulletDamage = 20;
    private readonly double bulletScale = 1;
    
    // Weapon states
    private int ammoCount;
    private double reloadTimer;
    private double fireCDTimer;

    public bool Reloading {
        get => reloading;
    }
    public bool Shooting {
        get => !reloading && player.Triggering;
    }

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Get parent player.
        player = transform.parent.GetComponent<Player>();

        // Register self to player.
        player.RegisterWeapon(this);

        // Init weapon states.
        ammoCount = magazineSize;
        reloading = false;
        reloadTimer = 0;
    }

    void ReloadStart() {
        Debug.Log("Reloading.");
        reloading = true;
        reloadTimer = 0;
        animator.Play("WeaponReload");
    }

    void ReloadFinish() {
        reloading = false;
        ammoCount = magazineSize;
        animator.Play("WeaponIdle");
    }

    void ReloadInterrupt() {
        reloading = false;
        animator.Play("WeaponIdle");
    }

    void ReloadCheck() {
        // Check if a reload can be done.
        bool interruptReload = player.Triggering && ammoCount > 0;

        if (!reloading) {
            if(ammoCount != magazineSize && !interruptReload)
                ReloadStart();
            else return;
        }

        // Reloading.
        if(interruptReload)
            ReloadInterrupt();

        reloadTimer += Time.deltaTime;
        if(reloadTimer > timeToReload)
            ReloadFinish();
    }

    void Fire() {
        Debug.Log("Firing.");
        fireCDTimer = rateOfFire;
        ammoCount --;
    }

    void FireCheck() {
        if(player.Triggering && fireCDTimer <= 0 && ammoCount > 0)
            Fire();
        if(fireCDTimer > 0)
            fireCDTimer -= Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        ReloadCheck();          // Check reloading.
        FireCheck();            // Check firing.

        // Get the current facing direction.
        
        var mousePos = GameManager.GetMouseLocalPosition(transform.parent);     // Get mouse position of player's coordinate.
        Vector3 newScale = Vector3.one;
        newScale.x *= Mathf.Sign(mousePos.x);
        transform.localScale = newScale;

        // Caculate the current transform.

        float angle = Vector2.SignedAngle(Vector2.right * Mathf.Sign(mousePos.x), mousePos);
        transform.eulerAngles = Vector3.forward * angle;
    }
}
