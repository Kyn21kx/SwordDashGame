using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
	private Vector2 m_bounds;
	private float startposX;
	private float startposY;
	private Transform cam;

	[SerializeField]
	private float parallaxEffect;

	void Start() {
		startposX = transform.position.x;
		startposY = transform.position.y;
		var rect = GetComponent<RectTransform>().rect;
		this.m_bounds = new Vector2(rect.width, rect.height);
		this.cam = EntityFetcher.Instance.MainCamera.transform;
	}

	void FixedUpdate() {
		float tempX = (cam.position.x * (1 - parallaxEffect));
		float distX = (cam.position.y * parallaxEffect);

		float tempY = (cam.position.x * (1 - parallaxEffect));
		float distY = (cam.position.y * parallaxEffect);

		transform.position = new Vector3(startposX + distX, startposY + distY, transform.position.z);

		if (tempX > startposX + this.m_bounds.x) startposX += this.m_bounds.x;
		else if (tempX < startposX - this.m_bounds.x) startposX -= this.m_bounds.x;

		if (tempY > startposX + this.m_bounds.y) startposY += this.m_bounds.y;
		else if (tempY < startposX - this.m_bounds.y) startposY -= this.m_bounds.y;

	}
}
