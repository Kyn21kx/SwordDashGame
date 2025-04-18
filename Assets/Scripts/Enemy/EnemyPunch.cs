using UnityEngine;

/// @brief Controls the punch mechanic of the normal enemy, will handle damage detection and rotations
class EnemyPunch : MonoBehaviour {

    private Transform m_target;
    private Vector2 m_punchDirection;
    [SerializeField]
    private float m_punchRange;
    private IDamageable m_playerHealth;
    private int m_damage;
    [SerializeField]
    private Animator m_animator;
    [SerializeField]
    private EnemyCombat m_combatRef;
    private float m_travelBlend;

    private Vector2 PunchOrigin => (Vector2)this.transform.parent.position + (this.m_punchDirection * 0.5f);

    private void Start() {
        this.m_playerHealth = EntityFetcher.Instance.Player.GetComponent<IDamageable>();
        this.m_animator.StopPlayback();
    }
    
    private void FixedUpdate() {
        if (this.m_combatRef.AttackState != EAttackStates.Attacking) return;
        // Do a Lerp and disable the traveling when getting to the end
        if (this.m_travelBlend > 1f) {
            this.DisablePunchHitbox();
            return;
        }
        this.transform.localPosition = Vector2.Lerp(Vector2.zero, this.m_punchDirection * this.m_punchRange, this.m_travelBlend);
        this.m_travelBlend += Time.fixedDeltaTime * 10.0f;
        

        // Perform the raycast if we still can
        Vector2 origin = PunchOrigin;
        Vector2 target = origin + (this.m_punchDirection * this.m_punchRange);
        float angle = Vector2.SignedAngle(origin, target);
        Vector2 size = new Vector2(0.01f, 0.5f);
        RaycastHit2D hit = Physics2D.BoxCast(origin, size, angle, this.m_punchDirection, this.m_punchRange, EntityFetcher.PlayerLayer);

        if (hit.transform == null) {
            return;
        }
        this.m_playerHealth.Damage(this.m_damage, this.transform.position);
        this.DisablePunchHitbox();
    }
    
    public void TriggerPunchHitbox(Transform target, int damage) {
        this.m_target = target;
        // TODO: Make this additive
        this.transform.LookAt(this.m_target);
        this.gameObject.SetActive(true);
        this.m_animator.enabled = true;
        this.m_combatRef.AttackState = EAttackStates.Preparing;
        this.m_punchDirection = (this.m_target.position - this.m_combatRef.transform.position).normalized;
        this.transform.localPosition = Vector2.zero + this.m_punchDirection * 1.0f;
    }

    public void CommitPunch() {
        this.transform.LookAt(this.m_target);
        this.m_animator.enabled = false;
        this.m_punchDirection = (this.m_target.position - this.m_combatRef.transform.position).normalized;
        this.m_combatRef.AttackState = EAttackStates.Attacking;
    }

    public void DisablePunchHitbox() {
        this.m_travelBlend = 0f;
        this.transform.localPosition = Vector2.zero;
        this.m_combatRef.AttackState = EAttackStates.Cooldown;
        this.gameObject.SetActive(false);
    }
    
    private void OnDrawGizmos() {
        Vector2 dir = m_punchDirection == Vector2.zero ? Vector2.right : this.m_punchDirection;
        GizmoExtensions.DrawWireCapsule(PunchOrigin, PunchOrigin + (dir * this.m_punchRange), 0.5f, 0.5f);
    }
}

