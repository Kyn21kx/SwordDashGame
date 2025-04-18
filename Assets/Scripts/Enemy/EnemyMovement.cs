using Auxiliars;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyMovement : MonoBehaviour {
    private const int DIRECTIONS_COUNT = 8;

    public float Speed => this.speed;

    private GameObject PlayerRef => EntityFetcher.Instance.Player;

    public Rigidbody2D Rig => this.m_rig;

    [SerializeField]
    private LayerMask collisionLayer;

    [SerializeField]
    private float speed;

    private EnemyBehaviour m_behaviourRef;

    [Header("Gizmos settings")]
    [SerializeField]
    private Color desiredDirectionColor;
    [SerializeField]
    private Color otherDirectionsColor;

    private float m_initialSpeed;

    /// <summary>
    /// The most likely direction the enemy will move towards
    /// </summary>
    private Vector2 desiredDirection;

    private Rigidbody2D m_rig;

    private RaycastHit2D[] directionHits;

    private void Start()
    {
        this.m_rig = GetComponent<Rigidbody2D>();
        this.m_behaviourRef = GetComponent<EnemyBehaviour>();
        this.m_initialSpeed = this.speed;
        this.m_behaviourRef.OnPlayerDetectedCallback.AddListener(() => {
           if (this.m_behaviourRef.Type == EnemyTypes.Health) {
            }
        });
    }

	private void FixedUpdate() {
        if (this.m_behaviourRef.IsPlayerDetected && this.desiredDirection != Vector2.zero) {
            this.MoveTo(this.desiredDirection, this.speed, 1f, this.m_behaviourRef.Type == EnemyTypes.Health);
        }
		if (Mathf.Approximately(this.m_rig.velocity.magnitude, 0f)) {
            this.m_rig.drag = 0.05f;
        }
	}

    private void Update() {
        if (!this.m_behaviourRef.IsPlayerDetected) return;
        switch(this.m_behaviourRef.Type) {
            case EnemyTypes.Health:
                Vector2 inverseDir = this.transform.position - this.PlayerRef.transform.position; 
                float dis = inverseDir.magnitude;
                if (dis <= this.m_behaviourRef.DetectionRange) {
                    this.speed = this.m_initialSpeed;
                    this.desiredDirection = inverseDir.normalized;
                }
                else {
                    Debug.Log($"Reducing speed to {this.speed}");
                    this.speed = Mathf.Max(Mathf.Lerp(this.speed, 0f, Time.deltaTime), 0f);
                }
                break;
        }
    }

	public void MoveTo(Vector2 direction, float speed, float timestep = 1f, bool useForce = false)
    {
        Assert.IsTrue(
            Mathf.Approximately(direction.magnitude, 1f),
            $"Trying to move {this.transform.name} towards non-normalized direction [v: {direction}, |v|: {direction.magnitude}] might lead to unexpected behaviour"
        );
        this.desiredDirection = direction;
        //Calculate whatever we need here for the AI shit and adjust the vel
        if (useForce) {
            this.m_rig.AddForce(direction * speed * timestep);
            Vector2 vel = this.m_rig.velocity;
            vel = Vector2.ClampMagnitude(vel, this.speed);
            this.m_rig.velocity = vel;
        }
        else {
            this.m_rig.velocity = direction * speed * timestep;
        }
        if (this.m_behaviourRef.Type == EnemyTypes.Health) {
            Debug.Log($"Vel: {this.m_rig.velocity}"); 
        }
    }

    public void Stop()
    {
        this.desiredDirection = Vector2.zero;
        this.m_rig.velocity = this.desiredDirection;
    }

	public void StopWithFriction() {
        this.m_rig.drag = 8f; //Some arbitrary drag coeficient
	}

	private void OnDrawGizmos()
    {
        Gizmos.color = this.desiredDirectionColor;
        const float visibleMultiplier = 2f;
        Gizmos.DrawLine(this.transform.position, (Vector2)this.transform.position + this.desiredDirection * visibleMultiplier);
    }

}
