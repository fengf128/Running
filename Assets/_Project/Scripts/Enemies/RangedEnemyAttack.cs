using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
public sealed class RangedEnemyAttack : MonoBehaviour, IEnemyAttack
{
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private Transform firePoint;
    [SerializeField, Min(0f)] private float attackCooldown = 1.5f;

    private Health ownerHealth;
    private float nextAttackTime;

    private void Awake()
    {
        ownerHealth = GetComponent<Health>();
    }

    public bool TryAttack(Transform target)
    {
        if (target == null || projectilePool == null || firePoint == null ||
            ownerHealth == null || Time.time < nextAttackTime)
        {
            return false;
        }

        Vector3 toTarget = target.position - firePoint.position;
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude < 0.001f)
        {
            return false;
        }

        Quaternion rotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        Projectile projectile = projectilePool.Spawn(firePoint.position, rotation, ownerHealth);
        if (projectile == null)
        {
            return false;
        }

        nextAttackTime = Time.time + attackCooldown;
        return true;
    }
}
