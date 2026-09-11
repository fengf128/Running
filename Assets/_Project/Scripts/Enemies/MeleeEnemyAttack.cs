using UnityEngine;

[DisallowMultipleComponent]
public sealed class MeleeEnemyAttack : MonoBehaviour, IEnemyAttack
{
    [SerializeField] private MeleeAttackData attackData;
    [SerializeField, Min(0f)] private float hitRange = 1.5f;

    private float nextAttackTime;
    private Health pendingTargetHealth;

    public bool TryAttack(Transform target)
    {
        if (attackData == null || target == null ||
            attackData.Damage <= 0f || Time.time < nextAttackTime)
        {
            return false;
        }

        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth == null || targetHealth.IsDead)
        {
            return false;
        }

        pendingTargetHealth = targetHealth;
        nextAttackTime = Time.time + attackData.Cooldown;
        return true;
    }

    public void ApplyPendingHit()
    {
        Health targetHealth = pendingTargetHealth;
        pendingTargetHealth = null;

        if (targetHealth == null || targetHealth.IsDead ||
            !targetHealth.gameObject.activeInHierarchy)
        {
            return;
        }

        // 与AI的距离判断一致，只比较水平距离；挥空不退还攻击冷却。
        Vector3 toTarget = targetHealth.transform.position - transform.position;
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude > hitRange * hitRange)
        {
            return;
        }

        targetHealth.TakeDamage(attackData.Damage);
    }

    private void OnDisable()
    {
        pendingTargetHealth = null;
    }
}
