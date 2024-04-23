using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreatureBase : MonoBehaviour
{
    public float Health { get; set; }
    public float Damage { get; set; }

    public CreatureBase(float health, float damage)
    {
        Health = health;
        Damage = damage;
    }

    public virtual void ConsumeDamage(float amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            Die();
        }
    }

    public abstract void PerformAttack();

    public virtual void Die()
    {
        //Debug.Log("IM DEAD");
        return;
    }
}
