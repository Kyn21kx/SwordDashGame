using UnityEngine;

public class EnemyCombatConfig : MonoBehaviour {
    [SerializeField]
    private float detectionRadius;
    [SerializeField]
    private Color detectionColor;

    [SerializeField]
    private float alertRadius;
    [SerializeField]
    private Color alertColor;

    [SerializeField]
    private float idleMovementSpeed;

    [SerializeField]
    private float aggressiveMovementSpeed;

    [SerializeField]
    private float defensiveMovementSpeed;


    private void OnDrawGizmos()
    {
        DrawCircleGizmos(this.transform.position, this.detectionRadius, this.detectionColor);
        DrawCircleGizmos(this.transform.position, this.alertRadius, this.alertColor);
    }

    private void DrawCircleGizmos(Vector2 center, float radius, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(center, radius);
    }

}
