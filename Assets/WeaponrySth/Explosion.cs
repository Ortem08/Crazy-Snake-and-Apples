using System;
using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour, IProjectile, IDamaging
{
    [SerializeField]
    private GameObject lightSource;

    [SerializeField]
    private GameObject explosionSurface;

    public DamageInfo DamageInfo { get; set; } = new DamageInfo(10);

    public event Action<IProjectileInfo> OnProjectileEvent;


    /// <param name="damage"></param>
    public void Explode(float damage, float damagingRadius=2.5f, float lifetimeSeconds = 0.6f, 
        float postExplosionRadiusRatio = 1.2f)
    {
        if (damagingRadius <= 0)
        {
            throw new ArgumentException($"{nameof(damagingRadius)} must be > 0");
        }

        lightSource.SetActive(true);
        explosionSurface.transform.localScale = Vector3.one * damagingRadius;
        explosionSurface.SetActive(true);
        
        var damageInfo = new DamageInfo(damage);    //not very neat, no time to refactor though
        DealDamage(damageInfo, damagingRadius);

        StartCoroutine(ExpandAndDieOut(damagingRadius,
            Mathf.Max(0.001f, postExplosionRadiusRatio), lifetimeSeconds));
    }

    public void Fire(Vector3 origin, Vector3 direction)
    {
        transform.position = origin;
        Explode(DamageInfo.Amount, lifetimeSeconds:0.05f);
    }

    public bool TryGetModificationInterface<T>(out T modifiable) where T : class
    {
        if (this is T t)
        {
            modifiable = t;
            return true;
        }
        modifiable = default;
        return false;
    }

    private void DealDamage(DamageInfo damageInfo, float damagingRadius)
    {
        foreach (var collision in Physics.OverlapSphere(transform.position, damagingRadius))    //use damagable layer to optimize
        {
            if (collision.gameObject.TryGetComponent<IHurtable>(out var hurtable))
            {
                hurtable.TakeDamage(damageInfo);
            }
        }
    }

    private IEnumerator ExpandAndDieOut(float initialRadius, float ratio, float lifetimeSeconds)
    {
        var timePassed = 0f;
        while (lifetimeSeconds > 0)
        {
            timePassed += Time.deltaTime;
            if (timePassed > lifetimeSeconds)
            {
                break;
            }
            explosionSurface.transform.localScale = Vector3.one * (
                    (initialRadius * (lifetimeSeconds - timePassed) 
                    + initialRadius * ratio * timePassed
                ) / (2.0f * lifetimeSeconds) );
            yield return null;
        }

        OnProjectileEvent?.Invoke(
                new CompositionBasedProjectileInfo(new object[]
                {
                    new LocationInfo(transform.position, UnityEngine.Random.onUnitSphere)
                })
            );
        Destroy(gameObject);
    }
}
