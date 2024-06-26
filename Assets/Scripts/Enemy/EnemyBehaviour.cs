using Auxiliars;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyCombat))]
public class EnemyBehaviour : MonoBehaviour, IDamageable {

    [SerializeField]
    private float detectRange;
    [SerializeField]
    private EnemyTypes type;

    private PlayerHealth playerHealthReference;
    //Implement the damageable interface through this field
    private EnemyCombat enemyCombat;

    public int Health => ((IDamageable)enemyCombat).Health;

    private void Start()
    {
        this.playerHealthReference = EntityFetcher.Instance.Player.GetComponent<PlayerHealth>();
        this.enemyCombat = GetComponent<EnemyCombat>();
    }

    private void FixedUpdate()
    {
        //Detect the player
        if (this.enemyCombat.IsPlayerInRange(this.detectRange))
        {
            this.enemyCombat.Attack();
            this.Die();
        }
        switch (type)
        {
            case EnemyTypes.Normal:
                this.enemyCombat.FollowPlayer(Time.fixedDeltaTime);
                break;
            case EnemyTypes.Spiked:
                break;
            default:
                throw new System.NotImplementedException($"Type {type} not implemented yet!");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, detectRange);
    }

    public void Damage(int value, Vector2 damageSourcePosition)
    {
        ((IDamageable)enemyCombat).Damage(value, damageSourcePosition);
    }

    public void Die()
    {
        ((IDamageable)enemyCombat).Die();
    }
}
