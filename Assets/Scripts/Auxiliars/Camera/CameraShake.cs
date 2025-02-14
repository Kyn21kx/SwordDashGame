using UnityEngine;

public class CameraShake : MonoBehaviour {

	// How long the object should shake for.
	public float shakeDuration = 0f;

	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;

	private bool m_shouldFollow;
	Vector3 originalPos;

	private void OnEnable() {
		originalPos = transform.localPosition;
	}

	private void Update() {
		if (shakeDuration > 0) {
			transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

			shakeDuration -= Time.deltaTime * decreaseFactor;
		}
		else {
			shakeDuration = 0f;
		}
	}

	public void Shake(float duration, bool shouldFollow = false) {
		shakeDuration = duration;
		this.originalPos = transform.localPosition;
		this.m_shouldFollow = shouldFollow;
	}

}