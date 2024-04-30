using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour, IProjectile, IHurtable
{
    public event Action<IProjectileInfo> OnProjectileEvent;

    private Rigidbody rb;

    private Collider collider;

    private bool activated = false;

    public float Speed { get; set; } = 20f;

    private float explosionDamage = 40;

    public float Health { get; set; } = 1;

    public GameObject ExplosionPrefab;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = rb.GetComponent<Collider>();
    }

    public void Fire(Vector3 origin, Vector3 direction, Vector3 baseVelocity = default)
    {
        transform.position = origin;
        transform.forward = direction.normalized;

        rb.velocity = direction.normalized * Speed + baseVelocity;
        rb.AddTorque(UnityEngine.Random.insideUnitSphere * 10);
        activated = true;
    }

    public bool TryGetModificationInterface<T>(out T modifiable) where T : class
    {
        modifiable = this as T;
        if (modifiable != null)
        {
            return true;
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!activated) return;

        if (collision.gameObject.TryGetComponent<PlayerComponent>(out _))
        {
            return;
        }

        Explode();
    }

    private void Explode()
    {
        //Debug.Log("explode");
        if (!Instantiate(ExplosionPrefab).TryGetComponent<Explosion>(out var explosion))
        {
            throw new Exception("not an explosion");
        }
        explosion.SetParams(2, 8, 0.3f);
        explosion.DamageInfo = explosion.DamageInfo.SetAmount(explosionDamage);
        explosion.Fire(transform.position, transform.forward);
        OnProjectileEvent?.Invoke(
                new CompositionBasedProjectileInfo(new[]
                {
                    new ExpirationInfo(transform.position, transform.forward)
                })
            );
        Destroy(gameObject);
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        // immune to explosion
        if (damageInfo.Type == DamageType.ExplosionDamage)
        {
            return;
        }

        //Health -= damageInfo.Amount;

        collider.enabled = false;
        Explode();
    }

    private void Update()
    {
        if (!activated) return;
        if (Physics.Raycast(transform.position, rb.velocity.normalized, 
            out var hitInfo, rb.velocity.magnitude * Time.deltaTime * 2, LayersStorage.Pierceable)
            )//&& !hitInfo.collider.gameObject.TryGetComponent<PlayerComponent>(out _))
        {
            Explode();
        }
    }

    public void ConsumeDamage(float amount)
    {
        throw new NotImplementedException();
    }
}
