using Auxiliars;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyCombat))]
public class EnemyBehaviour : MonoBehaviour, IDamageable {

    [SerializeField]
    private float detectRange;
    private PlayerHealth playerHealthReference;
    //Implement the damageable interface through this field
    private EnemyCombat enemyCombat;

    public int Health => ((IDamageable)enemyCombat).Health;

    private void Start()
    {
        this.playerHealthReference = EntityFetcher.Instance.Player.GetComponent<PlayerHealth>();
        this.enemyCombat = GetComponent<EnemyCombat>();
    }

    private void Update()
    {
        //Detect the player
        if (IsPlayerWithinRange(this.detectRange))
        {
            this.Attack();
            this.Die();
        }
    }

    private void Attack()
    {
        //Increase the scale of the object and do a sphere cast or something
        playerHealthReference.Damage(1, this.transform.position);
    }

    private bool IsPlayerWithinRange(float threshold)
    {
        return SpartanMath.ArrivedAt(this.transform.position, EntityFetcher.Instance.Player.transform.position, threshold);
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
