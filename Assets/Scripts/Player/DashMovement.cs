using Auxiliars;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DashMovement : MonoBehaviour
{
    private const int LEFT_MOUSE_BUTTON_INDEX = 0;
    //Limit in case the timer goes off to infinity
    private const float MAX_TIME_FOR_DASHING = 1f;
    [SerializeField]
    private float dashSpeed;

    [SerializeField] 
    private float maxDashDistance;

    [SerializeField]
    private FloatingAnimator floatAnimatorController;

    [SerializeField]
    private Animator burstAnimation;

    [SerializeField]
    private Transform spawnPivot;

    private Vector2 currMouseDirection;
    private Vector2 startingPosition;

    public Vector2 DashingDirection => this.dashingDirection;

    private Vector2 dashingDirection;
    private float currentMaxDashingDistance;
    
    private Rigidbody2D rig;
    public bool IsDashing => this.isDashing;

    [SerializeField]
    private bool isDashing;
    public bool IsAiming { get; private set; }

    //It is more reliable to use timers here tbh
    private SpartanTimer dashingTimer;

    private float initialDrag;

    private void Start()
    {
        this.isDashing = false;
        this.IsAiming = false;
        this.currMouseDirection = Vector2.zero;
        this.rig = GetComponent<Rigidbody2D>();
        this.initialDrag = this.rig.drag;
    }

    private void Update()
    {
        this.HandleInput();
        if (this.IsAiming)
        {
            this.Aim();
        }
    }

    private void Aim()
    {
        //Mouse look input to aim
        Camera mainCamRef = EntityFetcher.Instance.MainCamera;
        Vector2 mouseWorldPos = mainCamRef.ScreenToWorldPoint(Input.mousePosition);
        //Dest - Source
        this.currMouseDirection = (mouseWorldPos - (Vector2)this.transform.position).normalized;
        Quaternion targetRotation = SpartanMath.LookTowardsDirection(Vector3.forward, this.currMouseDirection);
        this.transform.rotation = targetRotation;
    }

    private void FixedUpdate()
    {
        if (!this.isDashing) return;
        this.DashTowards();
    }

    //Probably gonna refactor this into a different class
    public void BeginDash(Vector2 targetDirection, float travelAmount, Animator dashPropulsionAnimation, Vector2? normal = null)
    {
        this.rig.drag = 0f;
        this.IsAiming = false;
        this.rig.velocity = Vector2.zero;
        this.isDashing = true;
        this.startingPosition = this.rig.position;
        this.currentMaxDashingDistance = travelAmount;
        this.dashingDirection = targetDirection;
        //Instantiate the burst animation and rotate it towards the target direction
        Quaternion targetRotation = SpartanMath.LookTowardsDirection(Vector3.forward, targetDirection);
        this.transform.rotation = targetRotation;
        
        if (normal != null) {
			//Child rot = 90
			// Calculate the rotation that aligns with both the surface and the target direction
			Vector2 right = Vector2.Perpendicular(normal.Value); // Get the vector along the surface

			// Create a rotation matrix from the right and up vectors
			float angle = Mathf.Atan2(right.y, right.x) * Mathf.Rad2Deg;
			Quaternion finalRotation = Quaternion.Euler(0, 0, angle);

			// Apply the rotation
			Animator instance = TimedObject.InstantiateTimed(dashPropulsionAnimation, 1f, this.startingPosition + normal.Value.normalized, finalRotation);
        }
        else {
            Animator instance = TimedObject.InstantiateTimed(dashPropulsionAnimation, 1f, this.transform);
            instance.transform.parent = null;
            instance.transform.rotation = targetRotation;
        }

        this.dashingTimer.Reset();
        this.floatAnimatorController.StopAnimation();
    }

    public void StopDash()
    {
        this.isDashing = false;
        this.rig.velocity = Vector2.zero;
        this.dashingDirection = Vector2.zero;
        this.currMouseDirection = Vector2.zero;
        this.rig.drag = this.initialDrag;
        this.dashingTimer.Stop();
        this.floatAnimatorController.StartAnimation();
    }

    private void DashTowards()
    {
        //Get the velocity
        Vector2 movingVelocity = this.dashingDirection * dashSpeed;
        //Move towards that
        this.rig.velocity = movingVelocity;
        //Either count the time or the distance
        Vector2 targetPos = this.startingPosition + (this.dashingDirection * this.currentMaxDashingDistance);
        //Rotate the player's head towards the target

        float dis = Vector3.Distance(this.startingPosition, targetPos);
        //t = d / V
        float timeToStop = dis / this.rig.velocity.magnitude;
        if (float.IsNaN(timeToStop)) {
            timeToStop = 0f;
        }
        float elapsed = this.dashingTimer.CurrentTimeSeconds;
        
        if (elapsed >= timeToStop)
        {
            this.StopDash();
        }
    }

    private void HandleInput()
    {
        if (this.isDashing) return;
        //Make the user be able to aim with just a press of the mouse, shoot in release
        if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON_INDEX))
        {
            this.IsAiming = true;
        }
        //If the user clicks, go towards that direction, and disable the floating animation
        if (this.IsAiming && Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON_INDEX))
        {
            //Launch the sword towards the direction
            this.BeginDash(this.currMouseDirection, this.maxDashDistance, this.burstAnimation);
        }
    }

    private void OnDrawGizmos()
    {
        if (this.currMouseDirection == Vector2.zero || this.isDashing) return;
        Gizmos.color = Color.red;
        Vector2 dashEndDir = (this.currMouseDirection * maxDashDistance);
		Gizmos.DrawLine(this.rig.position, this.rig.position + dashEndDir);
    }

}
