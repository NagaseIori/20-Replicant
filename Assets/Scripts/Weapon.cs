using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Animator animator;

    private bool reloading = false;
    public float timeToReload = 2;  // in seconds

    private Player player;

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
    }

    void ReloadStart() {
        reloading = true;
        Invoke(nameof(ReloadFinish), timeToReload);
        animator.Play("WeaponReload");
    }

    void ReloadFinish() {
        reloading = false;
        animator.Play("WeaponIdle");
    }

    // Update is called once per frame
    void Update()
    {
        // Handle Input.

        if(!reloading && Input.GetKeyDown(KeyCode.R)) { // for debug
            ReloadStart();
        }

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
