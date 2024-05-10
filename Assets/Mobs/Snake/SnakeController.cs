using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class SnakeController : MonoBehaviour, IHurtable
{
    public float Health { get; private set; } = 1;

    public float MaxHealth {  get; private set; } = 100;

    public UnityEvent<float, float> OnHealthDecrease => throw new System.NotImplementedException();

    public UnityEvent<float, float> OnHealthIncrease => throw new System.NotImplementedException();

    public bool CanDie { get; set; } = true;

    public float JumpAwayAcceleration { get; set; } = 3;

    public float JumpAwaySpeed { get; set; } = 5;

    public float JumpAwayTimeLeft { get; set; } = 10;

    private bool isDead = false;

    private event Action OnSnakeDefeat;

    [SerializeField]
    private GameObject ApplePrefab;

    public void ConsumeDamage(float amount)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        Health -= damageInfo.Amount;
        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;

        OnSnakeDefeat?.Invoke();

        if (!CanDie)
        {
            // jump up
            StartCoroutine(JumpVeryHigh());
            return;
        }
        ExplodeWithFruts();
    }

    private IEnumerator JumpVeryHigh()
    {
        if (TryGetComponent<NavMeshAgent>(out var agent))
        {
            agent.enabled = false;
        }

        while (JumpAwayTimeLeft > 0)
        {
            JumpAwaySpeed += JumpAwayAcceleration * Time.deltaTime;
            transform.position += Vector3.up * JumpAwaySpeed * Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    private void ExplodeWithFruts()
    {
        var amount = 60;
        for (int i = 0; i < amount; i++)
        {
            var obj = Instantiate(ApplePrefab, transform.position + UnityEngine.Random.insideUnitSphere, UnityEngine.Random.rotation);
            var rb = obj.GetComponent<Rigidbody>();

            rb.AddExplosionForce(100, transform.position, 5, 3);
        }
        Destroy(gameObject);
    }
}
