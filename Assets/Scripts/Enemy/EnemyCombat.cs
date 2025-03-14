using Auxiliars;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public enum EAttackStates {
    Ready = 0,
    Preparing,
    Attacking,
    Cooldown,
    Recovering
}

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyCombat : MonoBehaviour, IDamageable
{
    
    [SerializeField]
    private int health;

    [SerializeField]
    private float movementSpeed; // Only applicable if the enemy is of type normal

    [SerializeField]
    private float attackCooldown;

    [Header("Only applicable to Enemies with type \"Shooting\"")]
    [SerializeField]
    private Projectile projectilePrefab;
    [SerializeField]
    private float shootingSpeed;

    [Header("Only applicable to Enemies with type \"Normal\"")]
    [SerializeField]
    private EnemyPunch m_punchCollider;
    
    private Rigidbody2D rig;

    public int Health => health;
    private IDamageable playerHealthRef;
	private EnemyMovement movRef;
    public EAttackStates AttackState { get; set; }

    private EnemyBehaviour behaviourRef;
    private SpartanTimer m_attackCooldownTimer;

	private void Start()
    {
        this.AttackState = EAttackStates.Ready;
        this.behaviourRef = GetComponent<EnemyBehaviour>();
        this.rig = GetComponent<Rigidbody2D>();
        this.movRef = GetComponent<EnemyMovement>();
        if (this.m_punchCollider != null) {
            this.m_punchCollider.DisablePunchHitbox();
        }
        this.m_attackCooldownTimer = new SpartanTimer(TimeMode.Framed);
        this.playerHealthRef = EntityFetcher.Instance.Player.GetComponent<PlayerHealth>();
    }

    private void Attack()
    {
        if (this.m_attackCooldownTimer.Started && this.m_attackCooldownTimer.CurrentTimeSeconds <= this.attackCooldown) {
            return;
        }
        this.AttackState = EAttackStates.Attacking;        
        Vector2 dirToPlayer = (Vector2)EntityFetcher.Instance.Player.transform.position - this.rig.position;
        dirToPlayer.Normalize();
        switch (this.behaviourRef.Type)
        {
            case EnemyTypes.Normal:
                // Trigger the hitbox, this will fire the animation, so send the attack cooldown in that lambda
                this.m_punchCollider.TriggerPunchHitbox(EntityFetcher.Instance.Player.transform, 1);
                break;
            case EnemyTypes.Spiked:
                //Increase the scale of the object and do a sphere cast or something
                playerHealthRef.Damage(1, this.gameObject);
                break;
            case EnemyTypes.Shooting:
                this.Shoot(dirToPlayer, this.shootingSpeed);
                break;
        }
        this.m_attackCooldownTimer.Reset();
    }

    public bool IsPlayerInRange(float detectionRange)
    {
        //Do a distance check and see if the player is within a range
        Collider2D playerColl = Physics2D.OverlapCircle(this.transform.position, detectionRange, EntityFetcher.PlayerLayer);
        return playerColl != null;
    }

    //Follow the player, with maybe a bit of noise, slowly
    public void FollowPlayer(float timeStep)
    {
        Transform playerTransform = EntityFetcher.Instance.Player.transform;
        //Get the position and slowly follow the player
        Vector2 playerPos = playerTransform.position;
        //Dest - source
        Vector2 direction = (playerPos - (Vector2)this.transform.position).normalized;
        this.rig.velocity = direction * movementSpeed * timeStep;
    }

    public void Shoot(Vector2 direction, float speed) {
        Assert.IsTrue(this.behaviourRef.Type == EnemyTypes.Shooting, $"Enemy {transform.name} is not a shooting type, this may cause unexpected behaviour!");
        Projectile projInstance = TimedObject.InstantiateTimed(this.projectilePrefab, 6f, this.transform.position, Quaternion.identity);
        projInstance.Initialize(direction, speed);
    }

    public void Damage(int value, Vector2 damageSourcePosition, bool shouldKnockback = true)
    {
        this.behaviourRef.IsPlayerDetected = true;
        this.health -= value;
        if (this.health <= 0)
        {
            this.Die();
        }
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }

	public void KnockbackForSeconds(Vector2 force, float seconds) {
        this.behaviourRef.IsPlayerDetected = true;
        this.movRef.Stop();
		this.movRef.Rig.AddForce(force, ForceMode2D.Impulse);
		StartCoroutine(this.ResetVelocityAfter(seconds));
	}

	private IEnumerator ResetVelocityAfter(float seconds) {
		yield return new WaitForSeconds(seconds);
		this.movRef.StopWithFriction();
	}

	public void Damage(int value, GameObject damageSource, bool shouldKnockback = true) {
        this.Damage(value, damageSource.transform.position, shouldKnockback);
	}
}
