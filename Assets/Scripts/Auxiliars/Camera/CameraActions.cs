using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CameraShake))]
public class CameraActions : MonoBehaviour {
	private CameraShake m_shakeController;

	private void Start() {
		this.m_shakeController = GetComponent<CameraShake>();
	}

	public void SendCameraShake(float shakeAmount, float shakeDuration) {
		this.m_shakeController.shakeAmount = shakeAmount;
		this.m_shakeController.Shake(shakeDuration);
	}
}
