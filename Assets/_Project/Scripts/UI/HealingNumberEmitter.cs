using UnityEngine;

[RequireComponent(typeof(Health))]
public sealed class HealingNumberEmitter : MonoBehaviour
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
            health.Healed += HandleHealed;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.Healed -= HandleHealed;
        }
    }

    private void HandleHealed(float actualHeal)
    {
        if (actualHeal <= 0f || damageNumberPool == null)
        {
            return;
        }

        damageNumberPool.ShowHealing(
            actualHeal,
            transform.position + positionOffset);
    }
}
