using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Auxiliars;

public class ShootyFellaAnim : MonoBehaviour {

	[SerializeField]
	private float midSpinRevPerSecond;
	[SerializeField]
	private float helixSpinRevPerSecond; //4 rots per second

	[SerializeField]
	private Transform helix;
	[SerializeField]
	private Transform midSpin;

	[Range(0f, 1f)]
	[SerializeField]
	private float followSpeed;

	[SerializeField]
	[Range(1f, 5f)]
	private float smoothStartAggressiveness;

	private Rigidbody2D rig;

	private void Start() {
		this.rig = GetComponent<Rigidbody2D>();
		//Unparent those bitches
		this.helix.SetParent(null);
		this.midSpin.SetParent(null);
	}

	private void Update() {
		this.helix.Rotate(Vector3.forward, Time.deltaTime * this.helixSpinRevPerSecond);
		this.midSpin.Rotate(Vector3.forward, -Time.deltaTime * this.midSpinRevPerSecond);
	}

	private void FixedUpdate() {
		//We do this in FixedUpdate because we want this to look like it's going at the same speed for both
		//When the player moves, Lerp a bit behind
		this.midSpin.position = Vector3.Lerp(this.midSpin.position, this.transform.position, this.followSpeed);
		this.helix.position = SpartanMath.SmoothStart(this.helix.position, this.transform.position, this.followSpeed, this.smoothStartAggressiveness);
	}


}
