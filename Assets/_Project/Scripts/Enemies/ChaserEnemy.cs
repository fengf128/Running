using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Health))]
public sealed class ChaserEnemy : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        Chase,
        Attack
    }

    [SerializeField, Min(0f)] private float detectionRange = 12f;
    [SerializeField, Min(0f)] private float moveSpeed = 2.5f;
    [SerializeField, Min(0f)] private float stoppingDistance = 1.3f;
    [SerializeField, Min(0f)] private float attackDamage = 10f;
    [SerializeField, Min(0f)] private float attackCooldown = 1f;
    [SerializeField] private EnemyState currentState = EnemyState.Idle;

    private CharacterController characterController;
    private Transform target;
    private Health targetHealth;
    private float nextAttackTime;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player with Player tag was not found.", this);
            return;
        }

        target = player.transform;
        targetHealth = player.GetComponent<Health>();
        if (targetHealth == null)
        {
            Debug.LogError("Player Health component was not found.", this);
        }
    }

    private void Update()
    {
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
                TryAttack();
                break;
        }
    }

    private void MoveTowardsTarget(Vector3 toTarget)
    {
        Vector3 direction = toTarget.normalized;
        Vector3 velocity = direction * moveSpeed + Vector3.down * 2f;
        characterController.Move(velocity * Time.deltaTime);
        transform.forward = direction;
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime)
        {
            return;
        }

        targetHealth.TakeDamage(attackDamage);
        nextAttackTime = Time.time + attackCooldown;
    }
}
