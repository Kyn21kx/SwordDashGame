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

    [SerializeField]
    private GameObject m_aimGuideSprite;
	[SerializeField]
	private GameObject m_collisionAimGuideSprite;

	private Vector2 m_currMouseDirection;
    private Vector2 m_startingPosition;

    public Vector2 DashingDirection => this.m_dashingDirection;

    private Vector2 m_dashingDirection;
    private float m_currentMaxDashingDistance;
    
    private Rigidbody2D m_rig;
    public bool IsDashing => this.isDashing;

    [SerializeField]
    private bool isDashing;
    public bool IsAiming { get; private set; }

    //It is more reliable to use timers here tbh
    private SpartanTimer m_dashingTimer;

    private float m_initialDrag;

    private LineRenderer m_lineRenderer;

    private void Start()
    {
        this.isDashing = false;
        this.IsAiming = false;
        this.m_currMouseDirection = Vector2.zero;
        this.m_rig = GetComponent<Rigidbody2D>();
        this.m_lineRenderer = GetComponent<LineRenderer>();
        this.m_initialDrag = this.m_rig.drag;
    }

    private void Update()
    {
        this.HandleInput();
        if (this.IsAiming)
        {
            this.Aim();
            return;
        }
        this.LiftAim();
    }

    private void Aim()
    {
        //Mouse look input to aim
        Camera mainCamRef = EntityFetcher.Instance.MainCamera;
        Vector2 mouseWorldPos = mainCamRef.ScreenToWorldPoint(Input.mousePosition);
        //Dest - Source
        this.m_currMouseDirection = (mouseWorldPos - (Vector2)this.transform.position).normalized;
        Quaternion targetRotation = SpartanMath.LookTowardsDirection(Vector3.forward, this.m_currMouseDirection);
        this.transform.rotation = targetRotation;
        //Do a line renderer thingy
        this.RenderAimGuide(this.m_currMouseDirection, this.maxDashDistance);
    }

    private void LiftAim() {
        this.m_lineRenderer.positionCount = 0;
        this.m_aimGuideSprite.SetActive(false);
		this.m_collisionAimGuideSprite.SetActive(false);
	}

    private void FixedUpdate()
    {
        if (!this.isDashing) return;
        this.DashTowards();
    }

    private void RenderAimGuide(Vector2 dir, float maxDistance) {
		this.m_collisionAimGuideSprite.SetActive(false);
		this.m_aimGuideSprite.SetActive(true);
		this.m_aimGuideSprite.transform.localRotation = Quaternion.identity;
        const float maxDistanceErrorCorrection = 1.13f;
		RaycastHit2D hit = Physics2D.Raycast(this.m_rig.position, dir, maxDistance * maxDistanceErrorCorrection, ~EntityFetcher.PlayerLayer);
        //Render until we find a collider in the bounceable layer or the enemy layer
        //Save the position at the sword
        if (hit.transform == null) {
            this.m_aimGuideSprite.transform.position = this.transform.position + ((Vector3)dir * maxDistance);
            return;
        }
		//Get the reflection vector
		if ((1 << hit.transform.gameObject.layer) == EntityFetcher.BounceLayer) {
		    //Handle the collision aim guide (smaller)
            this.m_collisionAimGuideSprite.SetActive(true);
			this.m_collisionAimGuideSprite.transform.position = hit.point;

			Vector2 reflection = SpartanMath.ReflectionVector(dir, hit.normal).normalized;
            this.m_aimGuideSprite.transform.position = (hit.point + (reflection * maxDistance));
			Quaternion targetRotation = SpartanMath.LookTowardsDirection(Vector3.forward, reflection);
			this.m_aimGuideSprite.transform.rotation = targetRotation;
			return;
        }
        this.m_aimGuideSprite.transform.position = hit.point - dir; //offset a bit
	}

    //Probably gonna refactor this into a different class
    public void BeginDash(Vector2 targetDirection, float travelAmount, Animator dashPropulsionAnimation, Vector2? normal = null)
    {
        this.m_rig.drag = 0f;
        this.IsAiming = false;
        this.m_rig.velocity = Vector2.zero;
        this.isDashing = true;
        this.m_startingPosition = this.m_rig.position;
        this.m_currentMaxDashingDistance = travelAmount;
        this.m_dashingDirection = targetDirection;
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
			Animator instance = TimedObject.InstantiateTimed(dashPropulsionAnimation, 1f, this.m_startingPosition + normal.Value.normalized, finalRotation);
        }
        else {
            Animator instance = TimedObject.InstantiateTimed(dashPropulsionAnimation, 1f, this.transform);
            instance.transform.parent = null;
            instance.transform.rotation = targetRotation;
        }

        this.m_dashingTimer.Reset();
        this.floatAnimatorController.StopAnimation();
    }

    public void StopDash()
    {
        this.isDashing = false;
        this.m_rig.velocity = Vector2.zero;
        this.m_dashingDirection = Vector2.zero;
        this.m_currMouseDirection = Vector2.zero;
        this.m_rig.drag = this.m_initialDrag;
        this.m_dashingTimer.Stop();
        this.floatAnimatorController.StartAnimation();
    }

    private void DashTowards()
    {
        //Get the velocity
        Vector2 movingVelocity = this.m_dashingDirection * dashSpeed;
        //Move towards that
        this.m_rig.velocity = movingVelocity;
        //Either count the time or the distance
        Vector2 targetPos = this.m_startingPosition + (this.m_dashingDirection * this.m_currentMaxDashingDistance);
        //Rotate the player's head towards the target

        float dis = Vector3.Distance(this.m_startingPosition, targetPos);
        //t = d / V
        float timeToStop = dis / this.m_rig.velocity.magnitude;
        if (float.IsNaN(timeToStop)) {
            timeToStop = 0f;
        }
        float elapsed = this.m_dashingTimer.CurrentTimeSeconds;
        
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
            this.BeginDash(this.m_currMouseDirection, this.maxDashDistance, this.burstAnimation);
        }
    }

    private void OnDrawGizmos()
    {
        if (this.m_currMouseDirection == Vector2.zero || this.isDashing) return;
        Gizmos.color = Color.red;
        Vector2 dashEndDir = (this.m_currMouseDirection * maxDashDistance);
		Gizmos.DrawLine(this.m_rig.position, this.m_rig.position + dashEndDir);
    }

}
