using UnityEngine;

public sealed class PlayerShooter : MonoBehaviour
{
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private Transform firePoint;
    [SerializeField, Min(0f)] private float fireCooldown = 0.15f;

    private float nextFireTime;

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

        projectilePool.Spawn(firePoint.position, firePoint.rotation);
        nextFireTime = Time.time + fireCooldown;
    }
}
