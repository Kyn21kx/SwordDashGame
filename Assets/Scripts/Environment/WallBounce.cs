using UnityEngine;
using UnityEngine.Assertions;

public class WallBounce : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";
    private const string ENEMY_TAG = "Enemy";

    private const float BOUNCE_OFF_FORCE = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string otherTag = collision.transform.tag;
        if (otherTag != PLAYER_TAG && otherTag != ENEMY_TAG)
        {
            return;
        }
        //Bounce them off
        var bounceable = collision.gameObject.GetComponent<IBounceable>();
        Assert.IsNotNull(bounceable, $"Bounce off target {collision.transform.name} should have an attached IBounceable component!");
        if (!bounceable.CanBounce()) return;
        Vector2 normalAvg = Vector2.zero;
        foreach (ContactPoint2D contact in collision.contacts)
        {
            normalAvg += contact.normal;
        }
        normalAvg *= 1f / collision.contactCount;
        bounceable.BounceOff(normalAvg, BOUNCE_OFF_FORCE);
    }
}
