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

public class GunShot : ProjectileBase, IDamaging, IPierceable
{
    public override event Action<IProjectileInfo> OnProjectileEvent;

    public DamageInfo DamageInfo { get; set; } = new DamageInfo(10);

    [DoNotSerialize]
    public bool CanPierce { get; set; } = false;

    private readonly float range = 100;

    private LineRenderer lineRenderer;

    private bool used = false;

    private Vector3? overridenVisibleRayBegin;

    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));   // may be use composition? nah, i`d win
        lineRenderer.material.color = Color.yellow;
    }

    public override void Fire(Vector3 origin, Vector3 direction, Vector3 baseVelocity = default)
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
                Debug.Log(hitOnHardSurface.collider.gameObject);
            }

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
            if (Physics.Raycast(origin, direction.normalized, out var firstHit, rangeLimit, LayersStorage.Pierceable | LayersStorage.NotPierceableObstacles))
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

        if (overridenVisibleRayBegin == null)
        {
            lineRenderer.SetPosition(0, origin);
        }
        else
        {
            lineRenderer.SetPosition(0, (Vector3)overridenVisibleRayBegin);
        }
        
        lineRenderer.SetPosition(1, expirationPos);

        expirationInfos.Add(new ExpirationInfo(expirationPos, direction.normalized));

        StartCoroutine(ShowLaserAndExpire(
                new CompositionBasedProjectileInfo(expirationInfos)
            ));
    }

    /// <summary>
    /// does not change actual ray tracing, only visible trace
    /// </summary>
    /// <param name="point"></param>
    public void SetVisibleRayBeginning(Vector3 point)
    {
        overridenVisibleRayBegin = point;
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
        yield return new WaitForSeconds(0.05f); // ”меньшенное врем€ видимости дл€ имитации вспышки
        OnProjectileEvent?.Invoke(projectileInfo);
        yield return new WaitForSeconds(0.05f);
        lineRenderer.enabled = false;
        
        Destroy(gameObject);
    }
}
