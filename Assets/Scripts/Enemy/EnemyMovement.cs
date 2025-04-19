using Auxiliars;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyMovement : MonoBehaviour
{

    public enum EMovementStates
    {
        Stationary = 0,
        Walking,
        FollowingPlayer,
        Retreating,
        Circling
    }

    private const int DIRECTIONS_COUNT = 8;

    public float Speed => this.m_currentSpeed;
    public float CombatSpeed => this.m_combatSpeed;
    public float IdleSpeed => this.m_idleSpeed;

    private GameObject PlayerRef => EntityFetcher.Instance.Player;

    public Rigidbody2D Rig => this.m_rig;

    [SerializeField]
    private LayerMask collisionLayer;

    [SerializeField]
    private EMovementStates m_movementState;

    private float m_currentSpeed;

    [SerializeField]
    private float m_idleSpeed;

    [SerializeField]
    private float m_combatSpeed;

    private EnemyBehaviour m_behaviourRef;

    [Header("Gizmos settings")]
    [SerializeField]
    private Color desiredDirectionColor;
    [SerializeField]
    private Color otherDirectionsColor;

    private float m_initialSpeed;
    private bool m_safeZone;

    /// <summary>
    /// The most likely direction the enemy will move towards
    /// </summary>
    private Vector2 desiredDirection;

    private Rigidbody2D m_rig;

    private RaycastHit2D[] directionHits;

    private SpartanTimer m_followPlayerTimer = new SpartanTimer(TimeMode.Framed);

    private void Start()
    {
        this.m_safeZone = false;
        this.m_rig = GetComponent<Rigidbody2D>();
        this.m_behaviourRef = GetComponent<EnemyBehaviour>();
        this.m_initialSpeed = this.m_currentSpeed;
        this.m_behaviourRef.OnPlayerDetectedCallback.AddListener(() =>
        {
            if (this.m_behaviourRef.Type == EnemyTypes.Health)
            {
            }
        });
    }

    private void FixedUpdate()
    {
        if (this.m_behaviourRef.IsPlayerDetected && this.desiredDirection != Vector2.zero)
        {
            this.MoveTo(this.desiredDirection, this.m_currentSpeed, 1f, this.m_behaviourRef.Type == EnemyTypes.Health);
        }
        if (Mathf.Approximately(this.m_rig.velocity.magnitude, 0f))
        {
            this.m_rig.drag = 0.05f;
        }
    }

    private void Update()
    {
        if (!this.m_behaviourRef.IsPlayerDetected) return;
        Vector2 enemyToPlayerDiff = this.PlayerRef.transform.position - this.transform.position;
        float distanceToPlayer = enemyToPlayerDiff.magnitude;
        switch (this.m_behaviourRef.Type)
        {
            case EnemyTypes.Shooting:
                this.HandleShootingMovement(enemyToPlayerDiff, distanceToPlayer);
                break;
            case EnemyTypes.Health:
                this.HandleHealthMovement(enemyToPlayerDiff, distanceToPlayer);
                break;
        }
    }

    private void HandleHealthMovement(Vector2 enemyToPlayerDiff, float distanceToPlayer)
    {
        if (distanceToPlayer > this.m_behaviourRef.DetectionRange)
        {
            Debug.Log($"Reducing speed to {this.m_currentSpeed}");
            this.m_currentSpeed = Mathf.Max(Mathf.Lerp(this.m_currentSpeed, 0f, Time.deltaTime), 0f);
            this.MoveOverrideSpeed(-enemyToPlayerDiff.normalized, this.m_currentSpeed);
            return;
        }
        this.MoveOverrideSpeed(-enemyToPlayerDiff.normalized, this.m_combatSpeed);
    }

    private void HandleShootingMovement(Vector2 enemyToPlayerDiff, float distanceToPlayer)
    {
        // Also don't go over the detection distance and keep moving around the player
        if (distanceToPlayer > m_behaviourRef.AttackRange)
        {
            // Go towards the player now
            if (!this.m_followPlayerTimer.Started)
            {
                this.m_followPlayerTimer.Reset();
            }
            float elapsed = this.m_followPlayerTimer.CurrentTimeSeconds;
            const float timeToStartFollowing = 0.8f;
            if (elapsed > timeToStartFollowing)
            {
                this.MoveOverrideSpeed(enemyToPlayerDiff.normalized, this.m_combatSpeed);
            }
            return;
        }
        this.m_followPlayerTimer.Stop();
        if (distanceToPlayer < m_behaviourRef.EscapeRange)
        {
            this.MoveOverrideSpeed(-enemyToPlayerDiff.normalized, this.m_combatSpeed);
        }
        else
        {
            float errRange = Mathf.Abs(this.m_behaviourRef.EscapeRange - this.m_behaviourRef.AttackRange);
            float currRange = Mathf.Abs(distanceToPlayer - this.m_behaviourRef.AttackRange);
            // this.Stop();
            this.CircleAroundPlayer(enemyToPlayerDiff, distanceToPlayer);
        }


    }

    private void CircleAroundPlayer(Vector2 enemyToPlayerDiff, float currentDistance)
    {
        // Calculate tangent direction
        Vector2 toPlayerNormalized = enemyToPlayerDiff.normalized;
        // Vector2 tangent = m_circlingClockwise 
        // ? new Vector2(-toPlayerNormalized.y, toPlayerNormalized.x) 
        // : new Vector2(toPlayerNormalized.y, -toPlayerNormalized.x);

        Vector2 tangent = new Vector2(-toPlayerNormalized.y, toPlayerNormalized.x);

        // Combine directions and move
        Vector2 combinedDirection = tangent.normalized;
        Debug.DrawLine(transform.position, transform.position + (Vector3)combinedDirection, Color.yellow);
        this.MoveOverrideSpeed(combinedDirection, this.m_combatSpeed);

        // Occasionally switch direction
        // if (Random.value < 0.005f) m_circlingClockwise = !m_circlingClockwise;
    }

    private void MoveOverrideSpeed(Vector2 direction, float speed)
    {
        this.m_rig.drag = 0.05f;
        this.m_currentSpeed = speed;
        this.desiredDirection = direction;
    }

    public void MoveTo(Vector2 direction, float speed, float timestep = 1f, bool useForce = false)
    {
        Assert.IsTrue(
            Mathf.Approximately(direction.magnitude, 1f),
            $"Trying to move {this.transform.name} towards non-normalized direction [v: {direction}, |v|: {direction.magnitude}] might lead to unexpected behaviour"
        );
        this.desiredDirection = direction;
        //Calculate whatever we need here for the AI shit and adjust the vel
        if (useForce)
        {
            this.m_rig.AddForce(direction * speed * timestep);
            Vector2 vel = this.m_rig.velocity;
            vel = Vector2.ClampMagnitude(vel, this.m_currentSpeed);
            this.m_rig.velocity = vel;
        }
        else
        {
            this.m_rig.velocity = direction * speed * timestep;
        }
        if (this.m_behaviourRef.Type == EnemyTypes.Health)
        {
            Debug.Log($"Vel: {this.m_rig.velocity}");
        }
    }

    public void Stop()
    {
        this.desiredDirection = Vector2.zero;
        this.m_rig.velocity = this.desiredDirection;
    }

    public void StopWithFriction()
    {
        this.m_rig.drag = 8f; //Some arbitrary drag coeficient
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = this.desiredDirectionColor;
        const float visibleMultiplier = 2f;
        Gizmos.DrawLine(this.transform.position, (Vector2)this.transform.position + this.desiredDirection * visibleMultiplier);
    }

}
