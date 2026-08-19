using UnityEngine;

[RequireComponent(typeof(Health))]
public sealed class PlayerShooter : MonoBehaviour
{
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private Transform firePoint;
    [SerializeField, Min(0f)] private float fireCooldown = 0.15f;

    private float nextFireTime;
    private Health ownerHealth;

    private void Awake()
    {
        ownerHealth = GetComponent<Health>();
    }

    public void TryFire()
    {
        if (projectilePool == null || firePoint == null)
        {
            return;
        }

        if (Time.time < nextFireTime)
        {
            return;
        }

        projectilePool.Spawn(firePoint.position, firePoint.rotation, ownerHealth);
        nextFireTime = Time.time + fireCooldown;
    }
}
