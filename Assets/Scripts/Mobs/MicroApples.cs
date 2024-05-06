using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MicroApples : CreatureBase, IMob
{
    private GameObject player { get; set; }
    private PlayerComponent playerComponent { get; set; }
    private const float AttackDistance = 2f;
    public StateMachine StateMachine { get; set; }
    public float MaxHealth;
    public float CriticalHealthPercentage = 30f;

    [SerializeField] private LayerMask obstructionMask;
    private float fieldOfViewAngle = 90f;

    private float criticalDistance = 0f;

    private NavMeshAgent agent;
    private NavMeshPath path;

    [SerializeField] private float randomPointRadius = 5f;


    public float rotationSpeed = 1.0f;
    public float rotationAngle = 90.0f;
    public float rotationInterval = 2.0f;
    private float currentAngle = 0.0f;
    private bool isRotating = false;
    private float timer = 0.0f;

    private Animator animator;


    public MicroApples() : base(20, 1)
    {
    }
    
    private void Start()
    {
        animator = gameObject.GetComponentInParent<Animator>();
        agent = GetComponentInParent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerComponent = player.GetComponent<PlayerComponent>();
        StateMachine = new StateMachine();
        StateMachine.ChangeState(new IdleState(this));
        path = new NavMeshPath();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        StateMachine.Update();

        //Debug.Log(CanSeePlayer());

        if (StateMachine.currentState is IdleState)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("Idle", true);
            agent.stoppingDistance = 0;
            TryPatrolInPlace();
        }
        else if (StateMachine.currentState is WanderState)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsWalking", true);
            agent.stoppingDistance = 0;
            TryPickRandomDestination();
        }
        else if (StateMachine.currentState is ChaseState)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("Idle", false);
            animator.SetBool("IsRunning", true);
            agent.stoppingDistance = 2;
            ChasePlayer();
        }
        else if (StateMachine.currentState is AttackState)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("Idle", false);
            animator.SetBool("IsRunning", false);
            animator.SetTrigger("Attack");
            PerformAttack();
        }
        else if (StateMachine.currentState is PanicState)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("Idle", true);
            RunAway();
        }
    }
    
    private void TryPatrolInPlace()
    {
        if (timer >= rotationInterval && !isRotating)
        {
            isRotating = true;
            currentAngle = 0.0f;
            timer = 0.0f;
        }
        if (isRotating)
        {
            RotateMob();
        }
    }

    void RotateMob()
    {
        float maxRadians = rotationAngle * Mathf.Deg2Rad;
        currentAngle += rotationSpeed * Time.deltaTime;
        float radians = currentAngle * Mathf.Deg2Rad;

        // Используем синус для модуляции скорости
        float sinusoidalSpeed = Mathf.Sin(Mathf.PI * radians / maxRadians);

        // Применяем скорость поворота
        transform.Rotate(Vector3.up, sinusoidalSpeed * Time.deltaTime * rotationSpeed);

        // Проверяем, достигли ли мы конечного угла
        if (radians >= maxRadians)
        {
            isRotating = false;
        }
    }

    public override void PerformAttack()
    {
        playerComponent.ConsumeDamage(Damage);
    }

    public bool CanSeePlayer()
    {
        // Получаем направление к цели, игнорируя компоненту y
        Vector3 directionToTargetFlat = player.transform.position - transform.position;
        directionToTargetFlat.y = 0;
        directionToTargetFlat.Normalize();

        // Используем плоский вектор направления вперёд, игнорируя компоненту y
        Vector3 forwardFlat = transform.forward;
        forwardFlat.y = 0;
        forwardFlat.Normalize();

        // Вычисляем угол между плоскими векторами
        var angle = Vector3.Angle(forwardFlat, directionToTargetFlat);
        Debug.Log(angle);

        if (angle <= fieldOfViewAngle / 2) // Проверяем, находится ли игрок в поле зрения
        {
            var distanceToTarget = Vector3.Distance(transform.position, player.transform.position);
            // Пускаем луч, начиная немного выше пола, чтобы избежать коллизии с землёй
            var rayStart = transform.position + Vector3.up * 0.5f;
            var let1 = Physics.Raycast(rayStart, directionToTargetFlat, distanceToTarget, obstructionMask);
            if (!let1) // Если луч не встретил препятствий, значит игрок виден
            {
                return true;
            }
        }

        return false;
    }



    public bool CanAttackPlayer()
    {
        var distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        return distanceToPlayer <= AttackDistance ? true : false;
    }

    public float GetHealthPercentage()
    {
        return (Health / MaxHealth) * 100;
    }

    public void RunAway()
    {
        agent.SetDestination(Vector3.Normalize(transform.position - player.transform.position) * 5);
    }

    public float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    public float GetCriticalDistance()
    {
        return criticalDistance;
    }

    public float GetCriticalHealthPercentage()
    {
        return CriticalHealthPercentage;
    }

    public void ChasePlayer()
    {
        agent.SetDestination(player.transform.position);
    }

    public void TryPickRandomDestination()
    {
        var randomPoint = transform.position + Random.insideUnitSphere * randomPointRadius;
        var IsCorrectPath = NavMesh.SamplePosition(randomPoint, out var hit, randomPointRadius, 1);
        while (!IsCorrectPath)
        {
            randomPoint = transform.position + Random.insideUnitSphere * randomPointRadius;
            IsCorrectPath = NavMesh.SamplePosition(randomPoint, out hit, randomPointRadius, 1);
        }

        var destination = hit.position;

        agent.CalculatePath(destination, path);
        
        if (path.status == NavMeshPathStatus.PathComplete)
            agent.SetPath(path);
    }
}
