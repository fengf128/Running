using UnityEngine;

public sealed class PlayerHydration : MonoBehaviour
{
    [SerializeField, Min(1f)] private float maxHydration = 100f;
    [SerializeField, Min(0f)] private float startingHydration = 50f;
    [SerializeField, Min(0f)] private float drainPerSecond = 1f;

    public float MaxHydration => maxHydration;
    public float CurrentHydration { get; private set; }

    private void Awake()
    {
        CurrentHydration = Mathf.Clamp(startingHydration, 0f, maxHydration);
    }

    private void Update()
    {
        if (CurrentHydration <= 0f || drainPerSecond <= 0f)
        {
            return;
        }

        CurrentHydration = Mathf.Max(
            0f,
            CurrentHydration - drainPerSecond * Time.deltaTime);
    }

    public bool TryRestore(float amount)
    {
        if (amount <= 0f || CurrentHydration >= maxHydration)
        {
            return false;
        }

        CurrentHydration = Mathf.Min(maxHydration, CurrentHydration + amount);
        Debug.Log($"{name} Hydration: {CurrentHydration}/{maxHydration}", this);
        return true;
    }

    public void RestoreFromSave(float savedHydration)
    {
        CurrentHydration = Mathf.Clamp(savedHydration, 0f, maxHydration);
        Debug.Log($"{name} Hydration loaded: {CurrentHydration}/{maxHydration}", this);
    }
}
