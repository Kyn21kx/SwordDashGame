using Auxiliars;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyCombat))]
public class EnemyBehaviour : MonoBehaviour, IDamageable {

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

    private void FixedUpdate()
    {
        //Detect the player
        this.RestorePlayerDetectionStatus();
        if (!this.IsPlayerDetected && this.enemyCombat.IsPlayerInRange(this.detectRange))
        {
            this.IsPlayerDetected = true;
        }
        else if (this.IsPlayerDetected && this.enemyCombat.IsPlayerInRange(this.attackRange)) {
            //We can attack
            this.enemyCombat.PrepareAttack();
        }
    }

    private void RestorePlayerDetectionStatus() {
		//Restore the player detection status if they're already detected and outside of the restoreDetectionRange
		if (IsPlayerDetected && !this.enemyCombat.IsPlayerInRange(this.restoreDetectionRange)) {
            this.IsPlayerDetected = false;
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

	public void Damage(int value, Vector2 damageSourcePosition)
    {
        ((IDamageable)enemyCombat).Damage(value, damageSourcePosition);
    }

    public void Die()
    {
        ((IDamageable)enemyCombat).Die();
    }

	public void KnockbackForSeconds(Vector2 force, float seconds) {
		((IDamageable)enemyCombat).KnockbackForSeconds(force, seconds);
	}

	public void Damage(int value, GameObject damageSource) {
		((IDamageable)enemyCombat).Damage(value, damageSource);
	}
}
