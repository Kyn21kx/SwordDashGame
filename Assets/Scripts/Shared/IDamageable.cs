using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public int Health { get; }

    void Damage(int value, Vector2 damageSourcePosition, bool shouldKnockback = true);
	
    void Damage(int value, GameObject damageSource, bool shouldKnockback = true);

    void Die();

    void KnockbackForSeconds(Vector2 force, float seconds);
}
