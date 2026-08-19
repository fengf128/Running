using System.Collections.Generic;
using UnityEngine;

public sealed class ProjectilePool : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField, Min(1)] private int initialSize = 20;

    private readonly Queue<Projectile> availableProjectiles = new Queue<Projectile>();

    private void Awake()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is not assigned.", this);
            return;
        }

        for (int i = 0; i < initialSize; i++)
        {
            availableProjectiles.Enqueue(CreateProjectile());
        }
    }

    public Projectile Spawn(Vector3 position, Quaternion rotation, Health projectileSource)
    {
        if (projectilePrefab == null)
        {
            return null;
        }

        Projectile projectile = availableProjectiles.Count > 0
            ? availableProjectiles.Dequeue()
            : CreateProjectile();

        projectile.Launch(position, rotation, projectileSource);
        return projectile;
    }

    public void Release(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        projectile.transform.SetParent(transform);
        availableProjectiles.Enqueue(projectile);
    }

    private Projectile CreateProjectile()
    {
        Projectile projectile = Instantiate(projectilePrefab, transform);
        projectile.Initialize(this);
        projectile.gameObject.SetActive(false);
        return projectile;
    }
}
