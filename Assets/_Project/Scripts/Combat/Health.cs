using System;
using UnityEngine;

public sealed class Health : MonoBehaviour
{
    [SerializeField, Min(1f)] private float maxHealth = 30f;

    public event Action<float> Damaged;
    public event Action<float> Healed;
    public event Action<float, float> HealthChanged;
    public event Action Died;

    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0f;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        HealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f)
        {
            return;
        }

        float previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        float actualDamage = previousHealth - CurrentHealth;

        Damaged?.Invoke(actualDamage);
        HealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (IsDead)
        {
            Died?.Invoke();
        }
    }

    public bool TryHeal(float amount)
    {
        if (IsDead || amount <= 0f || CurrentHealth >= maxHealth)
        {
            return false;
        }

        float previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        float actualHeal = CurrentHealth - previousHealth;

        Healed?.Invoke(actualHeal);
        HealthChanged?.Invoke(CurrentHealth, maxHealth);
        Debug.Log($"{name} HP: {CurrentHealth}/{maxHealth}", this);
        return true;
    }

    public void RestoreFromSave(float savedHealth)
    {
        bool wasDead = IsDead;
        float previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Clamp(savedHealth, 0f, maxHealth);
        Debug.Log($"{name} HP loaded: {CurrentHealth}/{maxHealth}", this);

        if (!Mathf.Approximately(previousHealth, CurrentHealth))
        {
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        if (!wasDead && IsDead)
        {
            Died?.Invoke();
        }
    }
}
