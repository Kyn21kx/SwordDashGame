using Auxiliars;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class TimedObject : MonoBehaviour {
	public static T InstantiateTimed<T>(T original, float lifeTime, Transform parent)
	where T : Object {
		T res = Instantiate(original, parent);
		InitializeTimedObject(res, lifeTime);
		return res;
	}

	public static T InstantiateTimed<T>(T original, float lifeTime, Vector3 position, Quaternion rotation) 
		where T : Object {
		T res = Instantiate(original, position, rotation);
		InitializeTimedObject(res, lifeTime);
		return res;
	}

	public static T InstantiateTimed<T>(T original, float lifeTime) where T : Object {
		T res = Instantiate(original);
		InitializeTimedObject(res, lifeTime);
		return res;
	}

	private static void InitializeTimedObject<T>(T instance, float lifeTime) where T : Object {
		TimedObject timedComponent = instance.AddComponent<TimedObject>();
		timedComponent.m_lifeTime = lifeTime;
		timedComponent.m_lifeTimer = new SpartanTimer(TimeMode.Framed);
	}

	private float m_lifeTime;

	private SpartanTimer m_lifeTimer;

	private void Start() {
		this.m_lifeTimer.Start();
	}

	public void Update() {
		if (this.m_lifeTimer.CurrentTimeSeconds >= this.m_lifeTime) {
			Destroy(this.gameObject);
		}
	}

}