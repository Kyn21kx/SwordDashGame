using Auxiliars;
using UnityEngine;

public class PlayerCombat : MonoBehaviour {

	private SpartanTimer heldAttackTimer;

	[SerializeField]
	private float rotationAnimationSpeed;

	private bool rotationStarted;
	private float initialAngle;

	[SerializeField]
	private float releaseThreshold;

	[SerializeField]
	private Color attackRadiusColor;

	[SerializeField]
	private float maximumRadius;

	[SerializeField]
	private float minimumRadius;

	[SerializeField]
	private float sweepForce;

	[SerializeField]
	private LayerMask bombAffectedLayer;

	[SerializeField]
	private GameObject rotationSweepPrefab;

	[SerializeField]
	private int attackDamage;

	private float currentAttackRadius;

	private Rigidbody2D rig;

	private Animator animatorController;

	private PlayerHealth healthController;

	public int AttackDamage => this.attackDamage;

	private void Start() {
		this.rig = GetComponent<Rigidbody2D>();
		this.animatorController = GetComponent<Animator>();
		this.healthController = GetComponent<PlayerHealth>();
		this.rotationStarted = false;
	}

	private void Update() {
		this.HandleInput();
		this.HandleSweepAttackControl();
	}

	private void HandleSweepAttackControl() {
		if (!this.heldAttackTimer.Started || this.rotationStarted) return;
		float minimumRequiredRadius = this.minimumRadius / this.maximumRadius;
		float percentageAmount = Mathf.Clamp01(minimumRequiredRadius + (this.heldAttackTimer.CurrentTimeSeconds / this.releaseThreshold));
		this.currentAttackRadius = this.maximumRadius * percentageAmount;
		if (this.heldAttackTimer.CurrentTimeSeconds >= this.releaseThreshold) {
			this.SweepAttack(this.currentAttackRadius);
		}
	}

	private void RotationVfx(float radius) {
		//Spawn in the rotating prefab anim
		this.rotationStarted = true;
		this.animatorController.SetTrigger("Spin");
		GameObject instance = TimedObject.InstantiateTimed(this.rotationSweepPrefab, 1f, this.transform);
		Vector3 scale = instance.transform.localScale;
		scale.x += radius;
		scale.y += radius;
        instance.transform.localScale = scale;
	}

	private void OnRotationFinished()
	{
		this.rotationStarted = false;
	}

	private void SweepAttack(float radius) {

		//Get a percentage of the radius to attack in
		//Sphere override collider
		this.StopHolding();
		//Trigger rotate the sword around
		this.RotationVfx(radius);

		RaycastHit2D[] enemiesToHit = Physics2D.CircleCastAll(this.transform.position, radius, Vector2.zero, 0f, bombAffectedLayer);
		for (int i = 0; i < enemiesToHit.Length; i++)
		{
			Transform enemyTransform = enemiesToHit[i].transform;
			var enemyDmg = enemyTransform.GetComponent<IDamageable>();
			Vector2 distanceVector = (enemyTransform.position - this.transform.position);
			Vector2 direction = distanceVector.normalized;
			/*
			//Knockback should be just outside the radius
			//Get the furthest point on the radius
			//The radius is the distance between the player and the desired point
			//We can then take the distance betwen the enemy and the player and substract that, use it as a magnitude
			*/
			float distanceToBeOutside = (radius - distanceVector.magnitude);
			//t = d / V
			float time = distanceToBeOutside / this.sweepForce;
			if (time < 0) continue;
			enemyDmg.KnockbackForSeconds(direction * this.sweepForce, time);
		}
	}

	private void HandleInput() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			this.heldAttackTimer.Reset();
		}
		if (Input.GetKeyUp(KeyCode.Space)) {
			if (this.currentAttackRadius > 0f) {
				this.SweepAttack(this.currentAttackRadius);
			}
			this.StopHolding();
		}
	}

	private void StopHolding() {
		this.heldAttackTimer.Stop();
		this.currentAttackRadius = 0f;
	}

	private void OnDrawGizmos() {
        Gizmos.color = this.attackRadiusColor * 0.5f;
        Gizmos.DrawWireSphere(this.transform.position, this.minimumRadius);
        if (this.currentAttackRadius <= 0f) return;
		Gizmos.color = this.attackRadiusColor;
		Gizmos.DrawWireSphere(this.transform.position, this.currentAttackRadius);
    }

}
