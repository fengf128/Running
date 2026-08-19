using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public sealed class Projectile : MonoBehaviour
{
    [SerializeField, Min(0f)] private float speed = 18f;
    [SerializeField, Min(0f)] private float lifetime = 2f;
    [SerializeField, Min(0f)] private float damage = 10f;

    private ProjectilePool owner;
    private Health sourceHealth;
    private Rigidbody cachedRigidbody;
    private float remainingLifetime;
    private bool isLaunched;

    private void Awake()
    {
        cachedRigidbody = GetComponent<Rigidbody>();
    }

    public void Initialize(ProjectilePool projectilePool)
    {
        owner = projectilePool;
    }

    public void Launch(Vector3 position, Quaternion rotation, Health projectileSource)
    {
        transform.SetPositionAndRotation(position, rotation);
        sourceHealth = projectileSource;
        remainingLifetime = lifetime;
        isLaunched = true;
        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (!isLaunched)
        {
            return;
        }

        Vector3 displacement = transform.forward * speed * Time.fixedDeltaTime;
        cachedRigidbody.MovePosition(cachedRigidbody.position + displacement);

        remainingLifetime -= Time.fixedDeltaTime;
        if (remainingLifetime <= 0f)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLaunched)
        {
            return;
        }

        Health targetHealth = other.GetComponentInParent<Health>();
        if (targetHealth == sourceHealth)
        {
            return;
        }

        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        isLaunched = false;
        sourceHealth = null;

        if (owner == null)
        {
            gameObject.SetActive(false);
            return;
        }

        owner.Release(this);
    }
}
