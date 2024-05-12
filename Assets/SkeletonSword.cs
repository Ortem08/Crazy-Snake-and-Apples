using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSword : MonoBehaviour
{

    private PlayerComponent playerComponent;
    private float swordDamage = 10;
    private Animator animator;

    [SerializeField]
    private Transform skeleton;

    private float swordImpulseModule = 10;

    void Start()
    {
        //swordDamage = 10;
        playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
        animator = transform.root.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        var hash = stateInfo.shortNameHash;
        var attackStateHash = Animator.StringToHash("Attack");

        if (other.CompareTag("Player") && hash == attackStateHash)
        {
            var delta = other.gameObject.transform.position - skeleton.position;
            var deltaNorm = skeleton.forward;
            if (delta.magnitude > 0)
            {
                deltaNorm = delta.normalized;
            }
            var pushDirection = (deltaNorm + Vector3.up / 2).normalized;

            playerComponent.TakeDamage(new DamageInfo(swordDamage, DamageType.MeleeDamage, pushDirection * swordImpulseModule));

            playerComponent.Push(pushDirection * swordImpulseModule);
        }
    }
}
