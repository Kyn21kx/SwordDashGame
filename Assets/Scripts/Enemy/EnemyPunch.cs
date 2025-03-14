using System.Collections;
using UnityEngine;

/// @brief Controls the punch mechanic of the normal enemy, will handle damage detection and rotations
class EnemyPunch : MonoBehaviour {

    private Vector2 m_punchDirection;
    [SerializeField]
    private float m_punchRange;
    private IDamageable m_playerHealth;
    private int m_damage;

    private Vector2 PunchOrigin => (Vector2)this.transform.parent.position + (this.m_punchDirection * 0.5f);

    private void Start() {
        this.m_playerHealth = EntityFetcher.Instance.Player.GetComponent<IDamageable>();
    }

    private void FixedUpdate() {
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
    
    public void TriggerPunchHitbox(Vector2 punchDirection, int damage) {
        this.m_punchDirection = punchDirection;
        this.transform.LookAt(punchDirection);
        this.gameObject.SetActive(true);
        this.DisableAfter(2);
    }

    public void DisablePunchHitbox() {
        this.gameObject.SetActive(false);
    }

    private IEnumerator DisableAfter(float time) {
        yield return new WaitForSeconds(time);
        this.DisablePunchHitbox();
    }
    
    private void OnDrawGizmos() {
        Vector2 dir = m_punchDirection == Vector2.zero ? Vector2.right : this.m_punchDirection;
        GizmoExtensions.DrawWireCapsule(PunchOrigin, PunchOrigin + (dir * this.m_punchRange), 0.5f, 0.5f);
    }
}

