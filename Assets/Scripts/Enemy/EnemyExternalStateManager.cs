using Auxiliars;
using UnityEngine;

public class EnemyExternalStateManager : MonoBehaviour, IBounceable {

    private Rigidbody2D rig;

    private FloatingAnimator animator;

    private SpartanTimer restoreAnimTimer;

    private EnemyBehaviour behaviourRef;

    private void Start()
    {
        this.restoreAnimTimer = new SpartanTimer(TimeMode.Framed);
        this.rig = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<FloatingAnimator>();
        this.behaviourRef = GetComponent<EnemyBehaviour>();
    }

    public void BounceOff(Vector2 source, float amount)
    {
        Vector2 force = ((Vector2)this.transform.position - source).normalized * amount;
        if (this.animator != null) {
            this.animator.StopAnimation();
        }
        this.rig.AddForce(force, ForceMode2D.Impulse);
        this.restoreAnimTimer.Reset();
    }

	public bool CanBounce() {
        return this.behaviourRef.IsPlayerDetected;
	}
}