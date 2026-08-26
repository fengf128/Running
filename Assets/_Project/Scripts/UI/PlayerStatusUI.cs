using TMPro;
using UnityEngine;

public sealed class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private PlayerHydration hydration;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text hydrationText;

    private int displayedHealth = -1;
    private int displayedHydration = -1;

    private void Awake()
    {
        if (health == null || hydration == null ||
            healthText == null || hydrationText == null)
        {
            Debug.LogError("Player status UI references are not assigned.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        displayedHealth = -1;
        displayedHydration = -1;
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        int currentHealth = Mathf.CeilToInt(health.CurrentHealth);
        int currentHydration = Mathf.CeilToInt(hydration.CurrentHydration);

        if (currentHealth != displayedHealth)
        {
            healthText.SetText(
                "生命：{0}/{1}",
                currentHealth,
                Mathf.CeilToInt(health.MaxHealth));
            displayedHealth = currentHealth;
        }

        if (currentHydration != displayedHydration)
        {
            hydrationText.SetText(
                "饮水：{0}/{1}",
                currentHydration,
                Mathf.CeilToInt(hydration.MaxHydration));
            displayedHydration = currentHydration;
        }
    }
}
