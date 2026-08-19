using UnityEngine;

public sealed class Health : MonoBehaviour
{
    [SerializeField, Min(1f)] private float maxHealth = 30f;

    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0f;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f)
        {
            return;
        }

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        Debug.Log($"{name} HP: {CurrentHealth}/{maxHealth}", this);

        if (IsDead)
        {
            gameObject.SetActive(false);
        }
    }
}
