using Auxiliars;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyCombat))]
public class EnemyBehaviour : MonoBehaviour, IDamageable {

    private const float SHOOTING_RANGE_BOOST = 2.5f;

    public UnityEvent OnPlayerDetectedCallback { get; private set; } = new UnityEvent();

    [SerializeField]
    private float detectRange;
    [SerializeField]
    private EnemyTypes type;
    [SerializeField]
	private float restoreDetectionRange;

	[SerializeField]
	private float attackRange;

	private PlayerHealth playerHealthReference;
    //Implement the damageable interface through this field
    private EnemyCombat enemyCombat;

    public int Health => enemyCombat.Health;

	public EnemyTypes Type => type;

    public bool IsPlayerDetected { get; set; }

	private void Start()
    {
        this.playerHealthReference = EntityFetcher.Instance.Player.GetComponent<PlayerHealth>();
        this.enemyCombat = GetComponent<EnemyCombat>();
        this.IsPlayerDetected = false;
    }

	private void Update() {
        if (!this.IsPlayerDetected || this.type != EnemyTypes.Shooting) return;
        //Face the player if shooty fella
        Vector2 inverseDir = this.transform.position - EntityFetcher.Instance.Player.transform.position;
        Quaternion lookDirection = SpartanMath.LookTowardsDirection(this.transform.forward, inverseDir);

        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, lookDirection, Time.deltaTime * 5f);
	}

	private void FixedUpdate()
    {
        //Detect the player
        this.RestorePlayerDetectionStatus();
        if (!this.IsPlayerDetected && this.enemyCombat.IsPlayerInRange(this.detectRange))
        {
            this.IsPlayerDetected = true;
            this.OnPlayerDetected();
        }
        //If we're detected and we're either a long range shooting enemy or within the attacking distance, shoot
        else if (this.IsPlayerDetected && this.enemyCombat.IsPlayerInRange(this.attackRange)) {
            //We can attack
            this.enemyCombat.Attack();
        }
    }

    private void OnPlayerDetected() {
        this.OnPlayerDetectedCallback.Invoke();
        switch (this.type) {
            case EnemyTypes.Normal:
				Debug.Log("Player detected on normal!");
                break;
            case EnemyTypes.Spiked:
                break;
            case EnemyTypes.Shooting:
				//Increase the attack range to some arbitrary value
				this.attackRange *= SHOOTING_RANGE_BOOST;
				this.detectRange *= SHOOTING_RANGE_BOOST;
                break;
            default:
                break;
        }
    }

    private void OnDetectionRestored() {
        switch (this.type) {
            case EnemyTypes.Normal:
                break;
            case EnemyTypes.Spiked:
                break;
            case EnemyTypes.Shooting:
                //Reduce it back 1.5x
                this.attackRange /= SHOOTING_RANGE_BOOST;
                this.detectRange /= SHOOTING_RANGE_BOOST;
                break;
            default:
                break;
        }
    }

    private void RestorePlayerDetectionStatus() {
		//Restore the player detection status if they're already detected and outside of the restoreDetectionRange
		if (IsPlayerDetected && !this.enemyCombat.IsPlayerInRange(this.restoreDetectionRange)) {
            this.IsPlayerDetected = false;
            this.OnDetectionRestored();
		}
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, detectRange);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(this.transform.position, this.attackRange);
	}

	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(this.transform.position, this.restoreDetectionRange);
	}

	public void Damage(int value, Vector2 damageSourcePosition, bool shouldKnockback = true)
    {
        ((IDamageable)enemyCombat).Damage(value, damageSourcePosition, shouldKnockback);
    }

    public void Die()
    {
        ((IDamageable)enemyCombat).Die();
    }

	public void KnockbackForSeconds(Vector2 force, float seconds) {
		((IDamageable)enemyCombat).KnockbackForSeconds(force, seconds);
	}

	public void Damage(int value, GameObject damageSource, bool shouldKnockback = true) {
		((IDamageable)enemyCombat).Damage(value, damageSource, shouldKnockback);
	}
}
