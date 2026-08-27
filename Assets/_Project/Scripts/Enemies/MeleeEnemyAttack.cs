using UnityEngine;

[DisallowMultipleComponent]
public sealed class MeleeEnemyAttack : MonoBehaviour, IEnemyAttack
{
    [SerializeField, Min(0f)] private float attackDamage = 10f;
    [SerializeField, Min(0f)] private float attackCooldown = 1f;

    private float nextAttackTime;

    public bool TryAttack(Transform target)
    {
        if (target == null || attackDamage <= 0f || Time.time < nextAttackTime)
        {
            return false;
        }

        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth == null || targetHealth.IsDead)
        {
            return false;
        }

        targetHealth.TakeDamage(attackDamage);
        nextAttackTime = Time.time + attackCooldown;
        return true;
    }
}
