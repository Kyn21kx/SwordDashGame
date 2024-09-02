using UnityEngine;
using UnityEngine.Assertions;

public class EnemyMovement : MonoBehaviour {
    private const int DIRECTIONS_COUNT = 8;

    public float Speed => this.speed;

    public Rigidbody2D Rig => this.rig;

    [SerializeField]
    private LayerMask collisionLayer;

    [SerializeField]
    private float speed;

    [Header("Gizmos settings")]
    [SerializeField]
    private Color desiredDirectionColor;
    [SerializeField]
    private Color otherDirectionsColor;

    /// <summary>
    /// The most likely direction the enemy will move towards
    /// </summary>
    private Vector2 desiredDirection;

    private Rigidbody2D rig;

    private RaycastHit2D[] directionHits;

    private void Start()
    {
        this.rig = GetComponent<Rigidbody2D>();
    }

	private void FixedUpdate() {
		if (Mathf.Approximately(this.rig.velocity.magnitude, 0f)) {
            this.rig.drag = 0.05f;
        }
	}

	public void MoveTo(Vector2 direction, float speed, float timestep = 1f)
    {
        Assert.IsTrue(
            Mathf.Approximately(direction.magnitude, 1f),
            $"Trying to move {this.transform.name} towards non-normalized direction [v: {direction}, |v|: {direction.magnitude}] might lead to unexpected behaviour"
        );
        this.desiredDirection = direction;
        //Calculate whatever we need here for the AI shit and adjust the vel
        this.rig.velocity = direction * speed * timestep;
    }

    public void Stop()
    {
        this.desiredDirection = Vector2.zero;
        this.rig.velocity = this.desiredDirection;
    }

	public void StopWithFriction() {
        this.rig.drag = 5f; //Some arbitrary drag coeficient
	}

	private void OnDrawGizmos()
    {
        Gizmos.color = this.desiredDirectionColor;
        const float visibleMultiplier = 2f;
        Gizmos.DrawLine(this.transform.position, (Vector2)this.transform.position + this.desiredDirection * visibleMultiplier);
    }

}