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
	private ProjectileType projectileType;

	private Rigidbody2D rig;

	private new SpriteRenderer renderer;

	private void Awake() {
		this.rig = GetComponent<Rigidbody2D>();
		this.renderer =	GetComponent<SpriteRenderer>();
	}

	public void Initialize(Vector2 direction, float speed) {
		this.direction = direction;
		this.speed = speed;
	}

	private void FixedUpdate() {
		this.rig.velocity = direction * speed;
		this.DetectPlayer();
	}

	private void DetectPlayer() {
	}

	private void OnDrawGizmos() {
		if (this.rig == null) return;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(this.rig.position, this.rig.position + (direction * speed));
	}

}
