using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO: Refactor most of this to the behaviour, and leave it just being public functions to interact with combat
public class EnemyCombat : MonoBehaviour, IDamageable
{
    [SerializeField]
    private EnemyTypes type;

    [SerializeField]
    private int health;

    [SerializeField]
    private float movementSpeed; // Only applicable if the enemy is of type normal
    private Rigidbody2D rig;

    public int Health => health;
    private IDamageable playerHealthRef;

    private void Start()
    {
        this.playerHealthRef = EntityFetcher.Instance.Player.GetComponent<PlayerHealth>();
        this.rig = GetComponent<Rigidbody2D>();
    }

    //For the enemy combat, determine the damageable actions on the type
    private void FixedUpdate()
    {
        switch (type)
        {
            case EnemyTypes.Normal:
                this.FollowPlayer(Time.fixedDeltaTime);
                break;
            case EnemyTypes.Spiked:
                break;
            default:
                throw new System.NotImplementedException($"Type {type} not implemented yet!");
        }
    }


    //Follow the player, with maybe a bit of noise, slowly
    private void FollowPlayer(float timeStep)
    {
        Transform playerTransform = EntityFetcher.Instance.Player.transform;
        //Get the position and slowly follow the player
        Vector2 playerPos = playerTransform.position;
        //Dest - source
        Vector2 direction = (playerPos - (Vector2)this.transform.position).normalized;
        this.rig.velocity = direction * movementSpeed * timeStep;
    }

    public void Damage(int value, Vector2 damageSourcePosition)
    {
        this.health -= value;
        if (this.health <= 0)
        {
            this.Die();
        }
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }
}
