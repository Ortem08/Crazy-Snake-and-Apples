using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class GunShot : MonoBehaviour, IProjectile, IDamaging, IPierceable
{
    public event Action<IProjectileInfo> OnProjectileEvent;

    public DamageInfo DamageInfo { get; set; } = new DamageInfo(10);

    [DoNotSerialize]
    public bool CanPierce { get; set; } = false;

    private readonly float range = 100;

    private LineRenderer lineRenderer;

    private bool used = false;

    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));   // may be use composition? nah, i`d win
        lineRenderer.material.color = Color.yellow;
    }

    public void Fire(Vector3 origin, Vector3 direction, Vector3 baseVelocity = default)
    {
        if (used)
        {
            throw new Exception("wtf maaan! its disposable");
        }
        used = true;

        var rangeLimit = range;
        var expirationPos = origin + direction.normalized * range;
        var expirationInfos = new List<object>();

        if (CanPierce)
        {
            if (Physics.Raycast(origin, direction.normalized, out var hitOnHardSurface, rangeLimit, LayersStorage.NotPierceableObstacles))
            {
                rangeLimit = hitOnHardSurface.distance;
                expirationPos = hitOnHardSurface.point;
                AttemptHurting(hitOnHardSurface.collider.gameObject);
                expirationInfos.Add(
                        new HitSomethingInfo(hitOnHardSurface.collider.gameObject,
                            hitOnHardSurface.point,
                            direction.normalized,
                            hitOnHardSurface.normal)
                        );
            }

            Physics.RaycastAll(
                    origin, direction.normalized, rangeLimit, LayersStorage.Pierceable
                ).Select(hit => AttemptHurting(hit.collider.gameObject))
                .ToList();

            foreach (var hitSoft in Physics.RaycastAll(
                    origin, direction.normalized, rangeLimit, LayersStorage.Pierceable))
            {
                AttemptHurting(hitSoft.collider.gameObject);
                OnProjectileEvent?.Invoke(new CompositionBasedProjectileInfo(new[]
                {
                    new HitSomethingInfo(hitSoft.collider.gameObject,
                        hitSoft.point, direction.normalized, hitSoft.normal)
                }));
            }
        }
        else
        {
            if (Physics.Raycast(origin, direction.normalized, out var firstHit, rangeLimit))
            {
                AttemptHurting(firstHit.collider.gameObject);
                expirationInfos.Add(
                        new HitSomethingInfo(firstHit.collider.gameObject,
                            firstHit.point,
                            direction.normalized,
                            firstHit.normal)
                        );
                expirationPos = firstHit.point;
            }
        }

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, expirationPos);

        expirationInfos.Add(new ExpirationInfo(expirationPos, direction.normalized));

        StartCoroutine(ShowLaserAndExpire(
                new CompositionBasedProjectileInfo(expirationInfos)
            ));
    }

    private bool AttemptHurting(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<IHurtable>(out var hurtable))
        {
            hurtable.TakeDamage(DamageInfo);
            return true;
        }
        return false;
    }

    private IEnumerator ShowLaserAndExpire(IProjectileInfo projectileInfo)
    {
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.1f); // ”меньшенное врем€ видимости дл€ имитации вспышки
        OnProjectileEvent?.Invoke(projectileInfo);
        yield return new WaitForSeconds(0.1f);
        lineRenderer.enabled = false;
        
        Destroy(gameObject);
    }

    public bool TryGetModificationInterface<T>(out T modifiable)
        where T : class
    {
        modifiable = this as T;
        if (modifiable != null)
        {
            return true;
        }
        return false;
    }
}
