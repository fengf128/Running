using UnityEngine;

[RequireComponent(typeof(Health))]
public sealed class RangedEnemy : MonoBehaviour
{
    [SerializeField, Min(0f)] private float attackRange = 12f;
    [SerializeField, Min(0f)] private float targetRefreshInterval = 0.25f;

    private IEnemyAttack attackStrategy;
    private HostileTargetFinder targetFinder;
    private Transform target;
    private Health targetHealth;
    private float nextTargetRefreshTime;

    private void Awake()
    {
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
        RefreshTargetIfNeeded();

        if (target == null || attackStrategy == null)
        {
            return;
        }

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude > attackRange * attackRange)
        {
            return;
        }

        if (toTarget.sqrMagnitude < 0.001f)
        {
            return;
        }

        transform.forward = toTarget.normalized;
        attackStrategy.TryAttack(target);
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

        targetHealth = targetFinder.FindNearestHostile(attackRange);
        target = targetHealth != null ? targetHealth.transform : null;
        nextTargetRefreshTime = Time.time + targetRefreshInterval;
    }
}
