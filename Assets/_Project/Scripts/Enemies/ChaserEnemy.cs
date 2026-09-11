using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Health))]
public sealed class ChaserEnemy : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack
    }

    [SerializeField, Min(0f)] private float detectionRange = 12f;
    [SerializeField, Min(0f)] private float moveSpeed = 2.5f;
    [SerializeField, Min(0f)] private float stoppingDistance = 1.3f;
    [SerializeField, Min(0f)] private float targetRefreshInterval = 0.25f;
    [SerializeField] private EnemyState currentState = EnemyState.Idle;

    public event Action<EnemyState> StateChanged;
    public event Action Attacked;

    public EnemyState CurrentState => currentState;

    private CharacterController characterController;
    private Health selfHealth;
    private IEnemyAttack attackStrategy;
    private HostileTargetFinder targetFinder;
    private Transform target;
    private Health targetHealth;
    private float nextTargetRefreshTime;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        selfHealth = GetComponent<Health>();
        attackStrategy = GetComponent<IEnemyAttack>();
        targetFinder = GetComponent<HostileTargetFinder>();
        if (attackStrategy == null)
        {
            Debug.LogError("Enemy attack component was not found.", this);
        }

        if (targetFinder == null)
        {
            Debug.LogError("Hostile target finder was not found.", this);
        }
    }

    private void Update()
    {
        if (selfHealth == null || selfHealth.IsDead ||
            characterController == null || !characterController.enabled)
        {
            return;
        }

        RefreshTargetIfNeeded();

        if (target == null || targetHealth == null || targetHealth.IsDead)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        float squaredDistance = toTarget.sqrMagnitude;

        EnemyState nextState = DecideState(squaredDistance);
        ChangeState(nextState);
        ExecuteState(toTarget);
    }

    private void RefreshTargetIfNeeded()
    {
        if (targetFinder == null)
        {
            target = null;
            targetHealth = null;
            return;
        }

        if (Time.time < nextTargetRefreshTime &&
            (targetHealth == null || !targetHealth.IsDead))
        {
            return;
        }

        targetHealth = targetFinder.FindNearestHostile(detectionRange);
        target = targetHealth != null ? targetHealth.transform : null;
        nextTargetRefreshTime = Time.time + targetRefreshInterval;
    }

    private EnemyState DecideState(float squaredDistance)
    {
        if (squaredDistance > detectionRange * detectionRange)
        {
            return EnemyState.Idle;
        }

        if (squaredDistance > stoppingDistance * stoppingDistance)
        {
            return EnemyState.Chase;
        }

        return EnemyState.Attack;
    }

    private void ChangeState(EnemyState nextState)
    {
        if (currentState == nextState)
        {
            return;
        }

        currentState = nextState;
        StateChanged?.Invoke(currentState);
    }

    private void ExecuteState(Vector3 toTarget)
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Chase:
                MoveTowardsTarget(toTarget);
                break;

            case EnemyState.Attack:
                FaceTarget(toTarget);
                if (attackStrategy?.TryAttack(target) == true)
                {
                    Attacked?.Invoke();
                }
                break;
        }
    }

    private void MoveTowardsTarget(Vector3 toTarget)
    {
        if (characterController == null || !characterController.enabled ||
            !characterController.gameObject.activeInHierarchy)
        {
            return;
        }

        Vector3 direction = toTarget.normalized;
        Vector3 velocity = direction * moveSpeed + Vector3.down * 2f;
        characterController.Move(velocity * Time.deltaTime);
        FaceTarget(toTarget);
    }

    private void FaceTarget(Vector3 toTarget)
    {
        if (toTarget.sqrMagnitude < 0.001f)
        {
            return;
        }

        transform.forward = toTarget.normalized;
    }
}
