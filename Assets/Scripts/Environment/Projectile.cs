using Auxiliars;
using UnityEngine;

public enum ProjectileType {
	Normal,
	Following,
	Multiple
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour
{

	private float speed;
	private Vector2 direction;

	[SerializeField]
	private float lifeTime;

	[SerializeField]
	private ProjectileType projectileType;
	
	private SpartanTimer lifeTimeTimer;

	private Rigidbody2D rig;

	private new SpriteRenderer renderer;

	private void Awake() {
		this.rig = GetComponent<Rigidbody2D>();
		this.renderer =	GetComponent<SpriteRenderer>();
	}

	public void Initialize(Vector2 direction, float speed) {
		this.direction = direction;
		this.speed = speed;
		this.lifeTimeTimer = new SpartanTimer(TimeMode.Fixed);
		this.lifeTimeTimer.Start();
	}

	private void FixedUpdate() {
		if (!this.lifeTimeTimer.Started) return;

		//This only serves to properly dispose of the projectile (incluiding using effects of any sort)
		this.HandleLifeTime();
		
		this.rig.velocity = direction * speed;
		this.DetectPlayer();
	}

	private void HandleLifeTime() {
		float elapsed = this.lifeTimeTimer.CurrentTimeSeconds;
		//We are no longer traveling, dissolve the projectile
		//Go from 0 to 1 on the remaining x% of the lifeTime
		Color targetColor = this.renderer.color;
		const float easingExponent = 2f;
		targetColor.a = SpartanMath.SmoothStart(1f, 0f, elapsed / lifeTime, easingExponent);
		this.renderer.color = targetColor;
		if (elapsed > lifeTime) {
			Destroy(this.gameObject);
		}
	}

	private void DetectPlayer() {
	}

	private void OnDrawGizmos() {
		if (this.rig == null) return;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(this.rig.position, this.rig.position + (direction * speed));
	}

}
