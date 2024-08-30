using Auxiliars;
using UnityEngine;

public class PlayerCombat : MonoBehaviour {

	private SpartanTimer heldAttackTimer;

	[SerializeField]
	private float releaseThreshold;

	[SerializeField]
	private Color attackRadiusColor;

	[SerializeField]
	private float maximumRadius;

	private float currentAttackRadius;

	private void Update() {
		this.HandleInput();
		if (this.heldAttackTimer.Started) {
			float percentageAmount = Mathf.Clamp01(this.heldAttackTimer.CurrentTimeSeconds / this.releaseThreshold);
			Debug.Log($"Amount held: {percentageAmount}");
			this.currentAttackRadius = this.maximumRadius * percentageAmount;
			if (this.heldAttackTimer.CurrentTimeSeconds >= this.releaseThreshold) {
				this.SweepAttack(percentageAmount);
			}
		}
	}

	private void SweepAttack(float percentageAmount) {
		//Get a percentage of the radius to attack in
		
		this.heldAttackTimer.Stop();
	}

	private void HandleInput() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			this.heldAttackTimer.Reset();
		}
		if (Input.GetKeyUp(KeyCode.Space)) {
			this.heldAttackTimer.Stop();
			this.currentAttackRadius = 0f;
		}
	}

	private void OnDrawGizmos() {
		if (this.currentAttackRadius <= 0f) return;
		Gizmos.color = this.attackRadiusColor;
		Gizmos.DrawWireSphere(this.transform.position, this.currentAttackRadius);
	}

}