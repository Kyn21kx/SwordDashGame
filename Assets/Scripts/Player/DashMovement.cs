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

    private void Start()
    {
        this.isDashing = false;
        this.IsAiming = false;
        this.currMouseDirection = Vector2.zero;
        this.rig = GetComponent<Rigidbody2D>();
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
    public void BeginDash(Vector2 targetDirection, float travelAmount)
    {
        this.IsAiming = false;
        this.rig.velocity = Vector2.zero;
        this.isDashing = true;
        this.startingPosition = this.rig.position;
        this.currentMaxDashingDistance = travelAmount;
        this.dashingDirection = targetDirection;
        //Instantiate the burst animation and rotate it towards the target direction
        Quaternion targetRotation = SpartanMath.LookTowardsDirection(Vector3.forward, targetDirection);
        Instantiate(this.burstAnimation.gameObject, this.startingPosition, targetRotation);
        this.transform.rotation = targetRotation;
        this.dashingTimer.Reset();
        this.floatAnimatorController.StopAnimation();
    }

    public void StopDash()
    {
        this.isDashing = false;
        this.rig.velocity = Vector2.zero;
        this.dashingDirection = Vector2.zero;
        this.currMouseDirection = Vector2.zero;
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
            this.BeginDash(this.currMouseDirection, this.maxDashDistance);
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
