using UnityEngine;

[RequireComponent(typeof(Health))]
public sealed class RangedEnemy : MonoBehaviour
{
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private Transform firePoint;
    [SerializeField, Min(0f)] private float attackRange = 12f;
    [SerializeField, Min(0f)] private float attackCooldown = 1.5f;

    private Transform target;
    private Health ownerHealth;
    private float nextAttackTime;

    private void Awake()
    {
        ownerHealth = GetComponent<Health>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player with Player tag was not found.", this);
            return;
        }

        target = player.transform;
    }

    private void Update()
    {
        if (target == null || projectilePool == null || firePoint == null)
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

        if (Time.time < nextAttackTime)
        {
            return;
        }

        projectilePool.Spawn(firePoint.position, firePoint.rotation, ownerHealth);
        nextAttackTime = Time.time + attackCooldown;
    }
}
