using UnityEngine;

[RequireComponent(typeof(Health))]
public sealed class DamageNumberEmitter : MonoBehaviour
{
    [SerializeField] private DamageNumberPool damageNumberPool;
    [SerializeField] private Vector3 positionOffset = new Vector3(0f, 2f, 0f);

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();

        if (damageNumberPool == null)
        {
            Debug.LogError("Damage number pool is not assigned.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (health != null && damageNumberPool != null)
        {
            health.Damaged += HandleDamaged;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.Damaged -= HandleDamaged;
        }
    }

    private void HandleDamaged(float actualDamage)
    {
        if (actualDamage <= 0f || damageNumberPool == null)
        {
            return;
        }

        damageNumberPool.Show(actualDamage, transform.position + positionOffset);
    }
}
