using System.Collections;
using TMPro;
using UnityEngine;

public sealed class PlayerDamageFeedback : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField, Min(0f)] private float flashDuration = 0.2f;

    private Color normalColor;
    private Coroutine flashCoroutine;
    private WaitForSeconds flashWait;

    private void Awake()
    {
        if (health == null || healthText == null)
        {
            Debug.LogError("Player damage feedback references are not assigned.", this);
            enabled = false;
            return;
        }

        normalColor = healthText.color;
        flashWait = new WaitForSeconds(flashDuration);
    }

    private void OnEnable()
    {
        if (health == null || healthText == null)
        {
            return;
        }

        health.Damaged += HandleDamaged;
        healthText.color = normalColor;
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.Damaged -= HandleDamaged;
        }

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        if (healthText != null)
        {
            healthText.color = normalColor;
        }
    }

    private void HandleDamaged(float actualDamage)
    {
        if (actualDamage <= 0f)
        {
            return;
        }

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashHealthText());
    }

    private IEnumerator FlashHealthText()
    {
        healthText.color = damageColor;
        yield return flashWait;
        healthText.color = normalColor;
        flashCoroutine = null;
    }
}
