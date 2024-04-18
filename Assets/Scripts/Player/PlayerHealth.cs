using Auxiliars;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageable {

    public delegate bool OnPlayerDamagedDidParry(int damage, Vector2 sourcePosition);

    private const float RESTORE_COLOR_TIME_MS = 150f;
    private const float ON_DAMAGE_KNOCK_BACK_STRENGTH = 12f;
    public int Health => health;
    public OnPlayerDamagedDidParry OnDamageParryCheckCallback { get; set; }


    [SerializeField]
    private int health;

    [SerializeField]
    private SpriteRenderer spriteRenderer; // Should be in child component
    private Rigidbody2D rig;
    private DashMovement dashReference;
    private FloatingAnimator floatingAnimatorReference;
    private SpartanTimer spriteBackToNormalTimer;

    private void Start ()
    {
        rig = GetComponent<Rigidbody2D>();
        this.dashReference = GetComponent<DashMovement>();
        this.floatingAnimatorReference = GetComponentInChildren<FloatingAnimator>();
        this.spriteBackToNormalTimer = new SpartanTimer(TimeMode.Framed);
    }

    private void Update ()
    {
        if (!this.spriteBackToNormalTimer.Started) return;

        float currTime = this.spriteBackToNormalTimer.CurrentTimeMS;
        if (currTime >= RESTORE_COLOR_TIME_MS)
        {
            this.RecoverFromHit();
        }
    }

    public void Damage(int value, Vector2 damageSourcePosition)
    {
        //We want to call the event right after we've been hit, but before we are damaged to be able to parry
        bool didParry = this.OnDamageParryCheckCallback(value, damageSourcePosition);
        if (didParry)
        {
            Debug.Log("Parry!");
            return;
        }

        this.health -= value;
        
        if (this.health <= 0)
        {
            this.Die();
            return;
        }
        //Go back a bit and flash red
        this.OnDamaged(value, damageSourcePosition);
    }

    private void RecoverFromHit()
    {
        this.spriteBackToNormalTimer.Stop();
        spriteRenderer.color = Color.white;
        this.dashReference.StopDash();
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void OnDamaged(int value, Vector2 damageSourcePosition)
    {
        //Stop the dash
        this.dashReference.StopDash();
        this.spriteRenderer.color = Color.red;
        //Go backwards from where we received the damage
        Vector2 stepBackDirection = ((Vector2)this.transform.position - damageSourcePosition).normalized;
        this.rig.AddForce(stepBackDirection * ON_DAMAGE_KNOCK_BACK_STRENGTH, ForceMode2D.Impulse);
        //Then here, begin a timer, and once it's done, go back to normal
        Debug.Log("Damaged!");
        this.spriteBackToNormalTimer.Reset();
    }

}
