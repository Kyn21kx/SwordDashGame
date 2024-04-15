using Auxiliars;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Physics based floating animation controller using a sin function and noise
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class FloatingAnimator : MonoBehaviour, IPhysicsAnimation
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float maxHeight;

    private Rigidbody2D rig;
    private float currActiveTimeTarget;

    private bool isRunning;

    public bool IsRunning => isRunning;

    private void Start()
    {
        this.rig = GetComponent<Rigidbody2D>();
        this.StartAnimation();
    }

    private void FixedUpdate()
    {
        //Base condition to stop animations
        if (!this.isRunning) return;
        //So, we can apply a Sin(x) with an added noise
        this.ApplyAnimation();
    }

    private void ApplyAnimation()
    {
        Vector2 currVel = this.rig.velocity;
        float accelerationY = SpartanMath.Sin(this.currActiveTimeTarget * speed) * maxHeight;
        currVel.y = accelerationY;
        this.rig.velocity = currVel;
        //Apply perlin noise to the rotation, but not the position!!!
        this.currActiveTimeTarget += Time.fixedDeltaTime;
    }

    public void StopAnimation()
    {
        this.transform.localPosition = Vector2.zero;
        this.rig.velocity = Vector2.zero;
        this.isRunning = false;
    }

    public void StartAnimation()
    {
        //Apply it first to make it a bit smoother
        this.isRunning = true;
    }

}
