using Auxiliars;
using UnityEngine;

public class PlayerCombat : MonoBehaviour {

	private SpartanTimer heldAttackTimer;

	[SerializeField]
	private float rotationAnimationSpeed;

	private bool rotationStarted;
	private float rotatingBlend; //Lol
	private float initialAngle;

	[SerializeField]
	private float releaseThreshold;

	[SerializeField]
	private Color attackRadiusColor;

	[SerializeField]
	private float maximumRadius;

	[SerializeField]
	private float sweepForce;
	
	[SerializeField]
	private LayerMask bombAffectedLayer;

	[SerializeField]
	private GameObject rotationSweepPrefab;

	private float currentAttackRadius;

	private Rigidbody2D rig;

	private void Start() {
		this.rig = GetComponent<Rigidbody2D>();
		this.rotationStarted = false;
		this.rotatingBlend = 0f;
	}

	private void Update() {
		this.HandleInput();
		this.HandleSweepAttackControl();
		if (!this.rotationStarted) return;
		this.RotateFx();
	}

	private void RotateFx() {
		this.rotatingBlend += Time.deltaTime * this.rotationAnimationSpeed;
		if (rotatingBlend >= 1f) {
			this.rotationStarted = false;
			this.rotatingBlend = 0f;
		}
		Vector3 eulerRotation = this.transform.rotation.eulerAngles;
		eulerRotation.z = this.initialAngle - (SpartanMath.TAU * Mathf.Rad2Deg * this.rotatingBlend); //Unit circle is 1 Tau Radians
		this.transform.rotation = Quaternion.Euler(eulerRotation);
	}

	private void HandleSweepAttackControl() {
		if (!this.heldAttackTimer.Started || this.rotationStarted) return;
		float percentageAmount = Mathf.Clamp01(this.heldAttackTimer.CurrentTimeSeconds / this.releaseThreshold);
		this.currentAttackRadius = this.maximumRadius * percentageAmount;
		if (this.heldAttackTimer.CurrentTimeSeconds >= this.releaseThreshold) {
			this.SweepAttack(this.currentAttackRadius);
		}
	}

	private void PrepareRotationFx() {
		//Spawn in the rotating prefab anim
		this.rotationStarted = true;
		this.initialAngle = this.transform.rotation.eulerAngles.z;
		TimedObject.InstantiateTimed(this.rotationSweepPrefab, 1f, this.transform);
	}

	private void SweepAttack(float radius) {
		//Get a percentage of the radius to attack in
		//Sphere override collider
		this.StopHolding();
		//Trigger rotate the sword around
		this.PrepareRotationFx();

		RaycastHit2D[] enemiesToHit = Physics2D.CircleCastAll(this.transform.position, radius, Vector2.zero, 0f, bombAffectedLayer);
		Debug.Log($"Enemies found: {enemiesToHit.Length}");
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
			Debug.Log($"Time to stop pushback: {time}");
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
		if (this.currentAttackRadius <= 0f) return;
		Gizmos.color = this.attackRadiusColor;
		Gizmos.DrawWireSphere(this.transform.position, this.currentAttackRadius);
	}

}