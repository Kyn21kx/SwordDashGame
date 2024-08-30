using Auxiliars;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyCombat : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int health;

    [SerializeField]
    private float movementSpeed; // Only applicable if the enemy is of type normal
    private Rigidbody2D rig;

    public int Health => health;
    private IDamageable playerHealthRef;
	private EnemyMovement movRef;

	private void Start()
    {
        this.playerHealthRef = EntityFetcher.Instance.Player.GetComponent<PlayerHealth>();
        this.rig = GetComponent<Rigidbody2D>();
        this.movRef = GetComponent<EnemyMovement>();
    }

    public void PlayAttackAnimation()
    {
        //Here we call the animation itself, and at the end of the last frame, have a call to Attack()

    }

    public void Attack()
    {
        //Increase the scale of the object and do a sphere cast or something
        playerHealthRef.Damage(1, this.transform.position);
    }

    public bool IsPlayerInRange(float detectionRange)
    {
        //Do a distance check and see if the player is within a range
        return SpartanMath.ArrivedAt(this.transform.position, EntityFetcher.Instance.Player.transform.position, detectionRange);
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

    public void Damage(int value, Vector2 damageSourcePosition)
    {
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
		this.movRef.Stop();
		this.movRef.Rig.AddForce(force, ForceMode2D.Impulse);
		StartCoroutine(this.ResetVelocityAfter(seconds));
	}

	private IEnumerator ResetVelocityAfter(float seconds) {
		yield return new WaitForSeconds(seconds);
		this.movRef.Stop();
	}
}
