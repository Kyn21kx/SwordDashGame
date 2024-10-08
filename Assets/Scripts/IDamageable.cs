using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public int Health { get; }

    void Damage(int value, Vector2 damageSourcePosition);

    void Die();

    void KnockbackForSeconds(Vector2 force, float seconds);
}
