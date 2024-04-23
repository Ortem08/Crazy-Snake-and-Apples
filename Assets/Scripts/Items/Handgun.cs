using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handgun : MonoBehaviour, IInventoryItem
{
    [SerializeField]
    private GameObject onSceneAvatar;

    [SerializeField]
    private GameObject inHandAvatar;

    private IUser user;

    private Collider colliderForDetection;

    private LineRenderer lineRenderer;

    private readonly float damage = 10;

    private readonly float range = 100;

    private SoundController soundController;

    private void Awake()
    {
        colliderForDetection = GetComponent<Collider>();
    }

    private void Start()
    {
        soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        // Уменьшите ширину линии для большей реалистичности
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));   // may be use composition? whatever
        lineRenderer.material.color = Color.red;
    }

    public void DropOut()
    {
        transform.parent = null;
        onSceneAvatar.SetActive(true);
        colliderForDetection.enabled = true;
    }

    public void OnSelect()
    {
        inHandAvatar.SetActive(true);
    }

    public void OnUnselect()
    {
        inHandAvatar.SetActive(false);
    }

    public void SetUser(IUser user)
    {
        colliderForDetection.enabled = false;
        this.user = user;
        onSceneAvatar.SetActive(false);
        transform.parent = user.HandTransform;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = new Vector3(0, 90, 0);
    }

    public bool TryUsePrimaryAction()
    {
        Debug.Log("nieeeeaaayy!!!");
        RaycastHit hit;
        Vector3 shootDirection = user.CameraTransform.forward;
        // Добавьте небольшое смещение к startPosition, чтобы избежать перекрытия с камерой
        Vector3 startPosition = user.CameraTransform.position + shootDirection * 0.1f;

        //Debug.DrawLine(startPosition, startPosition + shootDirection * range, Color.red, 2.0f);

        Debug.Log(Physics.Raycast(startPosition, shootDirection, out hit, range));

        if (Physics.Raycast(startPosition, shootDirection, out hit, range))
        {
            Debug.Log(hit.rigidbody);

            if (hit.collider.gameObject.TryGetComponent<IHurtable>(out var hurtable))
            {
                hurtable.TakeDamage(new DamageInfo(damage));
            }

            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, startPosition + shootDirection * range);
        }

        soundController.PlaySound("PistolShot", startPosition, 0.8f);
        
        StartCoroutine(ShowLaser());
        return true;
    }

    private IEnumerator ShowLaser()
    {
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.1f); // Уменьшенное время видимости для имитации вспышки
        lineRenderer.enabled = false;
    }

    public bool TryUseSecondaryAction()
    {
        Debug.Log("Shoot yourself if the leg. Now.");
        return true;
    }
}
