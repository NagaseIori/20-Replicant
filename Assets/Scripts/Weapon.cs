using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Animator animator;

    protected bool reloading = false;

    // Parent player
    protected Player player;
    // Muzzle maker
    protected MuzzleMarker muzzleMarker;

    // Weapon basic data
    public double rateOfFire = 0.2;         // in seconds
    public double timeToReload = 2;         // in seconds
    public int magazineSize = 10;   
    public double bulletSpeed = 20;         // ups
    public double bulletDamage = 20;
    public double bulletKnockback = 3;
    public double bulletSize = 1;           // in px
    public double bulletTTL = 1;            // in seconds
    
    // Weapon states
    protected int ammoCount;
    protected double reloadTimer;
    protected double fireCDTimer;
    protected float angle;

    // Prefabs
    public Bullet bulletPrefab;

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

        // Get muzzle marker.
        muzzleMarker = GetComponentInChildren<MuzzleMarker>();
        if(muzzleMarker == null) {
            Debug.LogError("Muzzle marker not set.");
        }

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
        BulletCreate();
    }

    void FireCheck() {
        if(player.Triggering && fireCDTimer <= 0 && ammoCount > 0)
            Fire();
        if(fireCDTimer > 0)
            fireCDTimer -= Time.deltaTime;
    }

    void BulletCreate() {
        Bullet inst = Instantiate(bulletPrefab);
        inst.transform.position = muzzleMarker.transform.position;
        inst.Init(bulletSize, bulletSpeed, bulletTTL, bulletDamage, bulletKnockback, 
            transform.TransformDirection(Vector2.right * transform.localScale.x));
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

        angle = Vector2.SignedAngle(Vector2.right * Mathf.Sign(mousePos.x), mousePos);
        transform.eulerAngles = Vector3.forward * angle;
    }
}
