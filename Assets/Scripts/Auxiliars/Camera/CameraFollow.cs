using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Auxiliars;
using TMPro;


[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(SpartanCamera))]
public class CameraFollow : MonoBehaviour {

	enum FollowMode {
		LINEAR,
		SMOOTH_START,
		SMOOTH_STOP,
		CONSTANT
	}

	public bool InDeadZone { get; private set; }
	public bool IsCentered => SpartanMath.ArrivedAt((Vector2)TargetPosition, (Vector2)this.transform.position);

	public Vector3 TargetPosition => camProperties.Target.position - offset;

    [SerializeField]
	private UpdateModes updateMode;

	[SerializeField]
	private FollowMode followMode;

	[SerializeField]
	private float followSpeed;

    [SerializeField]
    private float recenterSpeed;

    private SpartanCamera camProperties;

	private float followingBlend;

	private Vector3 offset;

	private SpartanTimer recenterTimer;

	private void Start() {
		camProperties = GetComponent<SpartanCamera>();
		//Right here we set an initial offset that will be used to maintain the distance relative to the player
		followingBlend = 0f;
		offset = camProperties.Target.position - transform.position;
		this.recenterTimer = new SpartanTimer(TimeMode.RealTime);
	}

	private void FixedUpdate() {
		if (updateMode != UpdateModes.FIXED) return;
		Follow(Time.fixedDeltaTime);
	}

	private void Update() {
		this.InDeadZone = IsInDeadZone(camProperties.Target.position);
		if (updateMode != UpdateModes.FRAMED) return;
		Follow(Time.deltaTime);
	}

	private void LateUpdate() {
		if (updateMode != UpdateModes.LATE) return;
		Follow(Time.deltaTime);
	}

	private void Follow(float timeStep) {
		if (!InDeadZone) {
            MoveTowards(TargetPosition, timeStep, this.followSpeed);
			return;
        }
		if (!this.recenterTimer.Started)
		{
			this.recenterTimer.Reset();
		}
		//We're in the dead zone, so, check if we should recenter first
        //Take care of recentering
        if (this.camProperties.ShouldRecenter) {
            this.HandleRecenter(timeStep);
			return;
        }
        followingBlend = 0f;
    }

	private void MoveTowards(Vector3 targetPosition,  float timeStep, float speed) {
        float addedT = speed * timeStep;
        followingBlend += addedT;
        if (followingBlend > 1f)
            followingBlend = 0f;

        switch (followMode)
        {
            case FollowMode.LINEAR:
                transform.position = SpartanMath.Lerp(transform.position, targetPosition, followingBlend);
                break;
            case FollowMode.SMOOTH_START:
                transform.position = SpartanMath.SmoothStart(transform.position, targetPosition, followingBlend, 2f);
                break;
            case FollowMode.SMOOTH_STOP:
                transform.position = SpartanMath.SmoothStop(transform.position, targetPosition, followingBlend, 2f);
                break;
            case FollowMode.CONSTANT:
                transform.position = SpartanMath.Lerp(transform.position, targetPosition, addedT);
                break;
        }
    }

	private void HandleRecenter(float timeStep) {
        float elapsedSeconds = this.recenterTimer.GetCurrentTime(TimeScaleMode.Seconds);
        if (elapsedSeconds < this.camProperties.RecenterTime)
        {
			this.followingBlend = 0f;
            return;
        }
        //Move the camera towards the center of the target
        this.MoveTowards(this.TargetPosition, timeStep, this.recenterSpeed);
		if (this.IsCentered)
		{
			this.recenterTimer.Stop();
		}

	}

	public bool IsInDeadZone(Vector3 target) {
		float dis = SpartanMath.DistanceSqr(target, camProperties.CenterPosition);
		Vector2 deadZoneMapped = camProperties.DeadZone / 2f;
		return dis <= (deadZoneMapped.x * deadZoneMapped.x) && dis <= (deadZoneMapped.y * deadZoneMapped.y);
	}
}
