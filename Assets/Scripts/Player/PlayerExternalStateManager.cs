using UnityEngine;

class PlayerExternalStateManager : MonoBehaviour, IBounceable {

    private Rigidbody2D rig;

    private DashMovement dashMovementRef;

    private void Start()
    {
        this.rig = GetComponent<Rigidbody2D>();
        this.dashMovementRef = GetComponent<DashMovement>();
    }

    public void BounceOff(Vector2 source, float amount)
    {
        //The player's bounce off behaviour will be to continue the dash but in the direction of the bounce
        Vector2 direction = ((Vector2)this.transform.position - source).normalized;
        this.dashMovementRef.BeginDash(direction, amount);
    }
}