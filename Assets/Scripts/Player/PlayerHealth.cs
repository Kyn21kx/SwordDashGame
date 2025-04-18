using Auxiliars;
using System.Collections;
using UnityEngine;

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
    private PlayerCombat combatReference;


    [SerializeField]
    private bool isImmune;

    public bool DidParry { get; private set; }


    private void Start ()
    {
        this.isImmune = false;
        this.DidParry = false;
        rig = GetComponent<Rigidbody2D>();
        this.dashReference = GetComponent<DashMovement>();
        this.combatReference = GetComponent<PlayerCombat>();
        this.floatingAnimatorReference = GetComponentInChildren<FloatingAnimator>();
        this.spriteBackToNormalTimer = new SpartanTimer(TimeMode.Framed);
	}

    private void Update ()
    {
        if (!this.spriteBackToNormalTimer.Started) return;
        this.isImmune = true;
        float currTime = this.spriteBackToNormalTimer.CurrentTimeMS;
        
        //Flash the sprite back and forth
        Color currColor = this.spriteRenderer.color;
        if (currColor.a != 1f) {
            currColor.a = 1f;
        }
        else {
            currColor.a = 0.5f;
        }

        this.spriteRenderer.color = currColor;

        if (currTime >= RESTORE_COLOR_TIME_MS)
        {
			this.RecoverFromHit();
        }
    }

    public void Damage(int value, Vector2 damageSourcePosition, bool shouldKnockback = true)
    {
        if (this.isImmune) return;
        //We want to call the event right after we've been hit, but before we are damaged to be able to parry
        this.DidParry = this.OnDamageParryCheckCallback(value, damageSourcePosition);
        if (this.DidParry)
        {
            this.isImmune = true;
            const float arbitrarySeconds = 0.5f;
            StartCoroutine(this.DisableImmunityAfter(arbitrarySeconds));
            return;
        }

        this.health -= value;
        
        if (this.health <= 0)
        {
            this.Die();
            return;
        }
        //Go back a bit and flash red
        this.OnDamaged(value, damageSourcePosition, shouldKnockback);
    }

    private void RecoverFromHit()
    {
		this.isImmune = false;
		this.spriteBackToNormalTimer.Stop();
        spriteRenderer.color = Color.white;
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void OnDamaged(int value, Vector2 damageSourcePosition, bool shouldKnockback)
    {
        if (shouldKnockback) {
			//Stop the dash
			this.dashReference.StopDash();
			//Go backwards from where we received the damage
			Vector2 stepBackDirection = ((Vector2)this.transform.position - damageSourcePosition).normalized;
			this.rig.AddForce(stepBackDirection * ON_DAMAGE_KNOCK_BACK_STRENGTH, ForceMode2D.Impulse);
		}
        EntityFetcher.Instance.CameraActionsRef.SendCameraShake(0.1f, 0.05f);
		this.spriteRenderer.color = Color.red;
		//Then here, begin a timer, and once it's done, go back to normal
        this.spriteBackToNormalTimer.Reset();
    }

    private IEnumerator DisableImmunityAfter(float seconds) {
        yield return new WaitForSeconds(seconds);
		this.isImmune = false;
        this.DidParry = false;
    }

	public void KnockbackForSeconds(Vector2 force, float seconds) {
		throw new System.NotImplementedException();
	}

	public void Damage(int value, GameObject damageSource, bool shouldKnockback = true) {
		if (this.isImmune) return;
		//We want to call the event right after we've been hit, but before we are damaged to be able to parry
		this.DidParry = this.OnDamageParryCheckCallback(value, damageSource.transform.position);
		if (this.DidParry) {
			this.isImmune = true;
			const float arbitrarySeconds = 0.5f;
            IDamageable damageable = damageSource.GetComponent<IDamageable>();
            if (damageable != null) {
                damageable.Damage(this.combatReference.AttackDamage, this.transform.position);
            }
			StartCoroutine(this.DisableImmunityAfter(arbitrarySeconds));
			return;
		}

		this.health -= value;

		if (this.health <= 0) {
			this.Die();
			return;
		}
		//Go back a bit and flash red
		this.OnDamaged(value, damageSource.transform.position, shouldKnockback);
	}
}
