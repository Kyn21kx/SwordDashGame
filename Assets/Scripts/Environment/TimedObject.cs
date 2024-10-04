using Auxiliars;
using Unity.VisualScripting;
using UnityEngine;

public class TimedObject : MonoBehaviour {

	public static TimedObject InstantiateTimed<T>(T original, float lifeTime, Transform parent)
	where T : Object {
		TimedObject res = Instantiate(original, parent).AddComponent<TimedObject>();
		res.m_lifeTime = lifeTime;
		res.m_lifeTimer = new SpartanTimer(TimeMode.Framed);
		return res;
	}

	public static TimedObject InstantiateTimed<T>(T original, float lifeTime, Vector3 position, Quaternion rotation) 
		where T : Object {
		TimedObject res = Instantiate(original, position, rotation).AddComponent<TimedObject>();
		res.m_lifeTime = lifeTime;
		res.m_lifeTimer = new SpartanTimer(TimeMode.Framed);
		return res;
	}

	public static TimedObject InstantiateTimed<T>(T original, float lifeTime) where T : Object {
		TimedObject res = Instantiate(original).AddComponent<TimedObject>();
		res.m_lifeTime = lifeTime;
		res.m_lifeTimer = new SpartanTimer(TimeMode.Framed);
		return res;
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