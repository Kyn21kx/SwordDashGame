using UnityEngine;
using UnityEditor;
using Auxiliars;

[ExecuteInEditMode]
public class SpartanCamera : MonoBehaviour {

	public float Distance => distance;

	public Transform Target => target;

	public Vector2 DeadZone => deadZone;
	
	public Vector2 SoftZone => deadZone;

	public bool ShouldRecenter => this.shouldRecenter;

	public float RecenterTime => this.recenterTime;

    public Vector3 CenterPosition => new Vector3(transform.position.x, transform.position.y, Mathf.Abs(transform.position.z) - Distance);

	[SerializeField]
	private Transform target;	
	
	[Header("Zoning info")]
	[SerializeField]
	private Vector2 deadZone;

    [SerializeField]
    private Vector2 softZone;

    [SerializeField]
	private Color deadZoneColor = Color.black;

	[SerializeField]
	private bool showDisplayInfo = true;

	[SerializeField]
	private bool shouldRecenter = false;

    [SerializeField]
    private float recenterTime = 1f;

    [SerializeField]
	private float distance;

	private void OnValidate() {
		if (Application.isPlaying || target == null) return;
		Vector3 adjustedPosition = transform.position;
		adjustedPosition.z = Target.position.z - Distance;
		transform.position = adjustedPosition;
	}

	private void OnDrawGizmos() {
		if (!showDisplayInfo) return;
		//There's actually something to draw
		Gizmos.color = this.deadZoneColor;
		Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y, 0f), deadZone);
	}
}
