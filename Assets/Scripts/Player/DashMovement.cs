using Auxiliars;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DashMovement : MonoBehaviour
{
    const int LEFT_MOUSE_BUTTON_INDEX = 0;

    [SerializeField]
    private float dashSpeed;

    [SerializeField] 
    private float maxDashDistance;

    [SerializeField]
    private FloatingAnimator floatAnimatorController;

    private Vector2 currMouseDirection;
    private Vector2 startingPosition;
    private Rigidbody2D rig;
    public bool IsDashing { get; private set; }

    private void Start()
    {
        this.IsDashing = false;
        this.currMouseDirection = Vector2.zero;
        this.rig = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        this.HandleInput();
    }

    private void FixedUpdate()
    {
        if (!this.IsDashing) return;
        this.DashTowards();
    }

    //Probably gonna refactor this into a different class
    private void BeginDash()
    {
        this.rig.velocity = Vector2.zero;
        this.IsDashing = true;
        this.startingPosition = this.rig.position;
        this.floatAnimatorController.StopAnimation();
    }

    public void StopDash()
    {
        this.IsDashing = false;
        this.rig.velocity = Vector2.zero;
        this.currMouseDirection = Vector2.zero;
    }

    private void DashTowards()
    {
        const float startAnimThreshold = 3f * 3f;
        const float stopDashThreshold = 0.2f * 0.2f;
        //Get the velocity
        Vector2 movingVelocity = this.currMouseDirection * dashSpeed;
        //Move towards that
        this.rig.velocity = movingVelocity;
        //Either count the time or the distance
        Vector2 targetPos = this.startingPosition + (this.currMouseDirection * this.maxDashDistance);
        //Rotate the player's head towards the target

        float disSqr = SpartanMath.DistanceSqr(this.rig.position, targetPos);

        if (disSqr <= startAnimThreshold && !this.floatAnimatorController.IsRunning)
        {
            this.floatAnimatorController.StartAnimation();
        }
        if (disSqr <= stopDashThreshold)
        {
            this.StopDash();
        }
    }

    private void HandleInput()
    {
        if (this.IsDashing) return;
        //Mouse look input to aim
        Camera mainCamRef = EntityFetcher.Instance.MainCamera;
        Vector2 mouseWorldPos = mainCamRef.ScreenToWorldPoint(Input.mousePosition);
        //Dest - Source
        this.currMouseDirection = (mouseWorldPos - (Vector2)this.transform.position).normalized;

        //If the user clicks, go towards that direction, and disable the floating animation
        if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON_INDEX))
        {
            //Launch the sword towards the direction
            this.BeginDash();
        }
    }

    private void OnDrawGizmos()
    {
        if (this.currMouseDirection == Vector2.zero || this.IsDashing) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.rig.position, this.rig.position + (this.currMouseDirection * maxDashDistance));
    }

}
