using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroApples : CreatureBase
{
    private GameObject player { get; set; }
    private const float AttackDistance = 2f;

    public MicroApples() : base(20, 1)
    {
    }
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        var distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        if (distanceToPlayer <= AttackDistance)
        {
            PerformAttack();
        }
    }

    public override void PerformAttack()
    {
        Debug.Log("MicroApple Atakin");
        //player.ConsumeDamage(Damage);
    }
}
