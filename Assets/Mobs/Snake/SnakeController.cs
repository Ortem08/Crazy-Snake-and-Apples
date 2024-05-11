using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class SnakeController : MonoBehaviour, IHurtable
{
    [SerializeField]
    private float initialHealth = 100;

    public float Health { get; private set; } = 10;

    public float MaxHealth => initialHealth;

    public UnityEvent<float, float> OnHealthDecrease { get; } = new();

    public UnityEvent<float, float> OnHealthIncrease { get; } = new();

    //public bool CanDie { get; set; } = true;



    private bool isDead = false;

    public event Action<Vector3> OnSnakeDefeat;

    [SerializeField]
    protected GameObject ApplePrefab;

    /*    public void SetStageOne()
        {
            Health = 100;
            MaxHealth = 100;
            CanDie = false;
        }*/

    protected void Start()
    {
        Health = initialHealth;
    }

    public void ConsumeDamage(float amount)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        Health -= damageInfo.Amount;
        OnHealthDecrease.Invoke(Mathf.Max(Health, 0), MaxHealth);
        if (Health <= 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;

        OnSnakeDefeat?.Invoke(transform.position);

        AfterDefeatAction();

/*        if (!CanDie)
        {
            // jump up
            StartCoroutine(JumpVeryHigh());
            return;
        }
        ExplodeWithFruts();*/
    }

    protected virtual void AfterDefeatAction()
    {

    }

/*    private IEnumerator JumpVeryHigh()
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
    }*/
}
