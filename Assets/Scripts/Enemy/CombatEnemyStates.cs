using UnityEngine;

public enum ECombatEnemyStates {
    Idle = 0,

    /// <summary>
    /// The enemy has just detected you and will engage combat
    /// </summary>
    Engaged,

    /// <summary>
    /// Currently executing attack animations
    /// </summary>
    Attacking,

    /// <summary>
    /// The enemy is strategically backing up
    /// </summary>
    BackingUp,

    /// <summary>
    /// The Enemy is approaching the player to attack them
    /// </summary>
    ClosingUp,

    /// <summary>
    /// Orbiting around the player to find an optimal attack position
    /// </summary>
    Surounding,

    Dead
}