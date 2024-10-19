using Auxiliars;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

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

    private Rigidbody2D rig;

    public int Health => health;
    private IDamageable playerHealthRef;
	private EnemyMovement movRef;

    private EnemyBehaviour behaviourRef;
    private SpartanTimer attackCooldownTimer;

	private void Start()
    {
        this.attackCooldownTimer = new SpartanTimer(TimeMode.Framed);
        this.playerHealthRef = EntityFetcher.Instance.Player.GetComponent<PlayerHealth>();
        this.rig = GetComponent<Rigidbody2D>();
        this.movRef = GetComponent<EnemyMovement>();
        this.behaviourRef = GetComponent<EnemyBehaviour>();
    }

    public void PrepareAttack()
    {
        //Do the animation, then attack
        //NYI tho, so
        Attack();
    }

    private void Attack()
    {
        if (this.attackCooldownTimer.Started && this.attackCooldownTimer.CurrentTimeSeconds <= this.attackCooldown) {
            return;
        }

        switch (this.behaviourRef.Type)
        {
            case EnemyTypes.Normal:
                break;
            case EnemyTypes.Spiked:
                //Increase the scale of the object and do a sphere cast or something
                playerHealthRef.Damage(1, this.gameObject);
                break;
            case EnemyTypes.Shooting:
                Vector2 dir = (Vector2)EntityFetcher.Instance.Player.transform.position - this.rig.position;
                dir.Normalize();
                this.Shoot(dir, this.shootingSpeed);
                break;
        }
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
        this.attackCooldownTimer.Reset();
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
