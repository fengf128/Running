using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public sealed class DeactivateOnDeath : MonoBehaviour
{
    [Header("Fall Visual")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Vector3 fallEulerAngles = new(0f, 0f, 90f);
    [SerializeField] private Vector3 fallenPositionOffset = new(0f, -0.5f, 0f);
    [SerializeField, Min(0f)] private float fallDuration = 0.4f;
    [SerializeField, Min(0f)] private float remainDuration = 0.6f;

    [Header("Disable On Death")]
    [SerializeField] private Behaviour[] behavioursToDisable;
    [SerializeField] private Collider[] collidersToDisable;

    private Health health;
    private Coroutine deathCoroutine;
    private WaitForSeconds remainWait;
    private bool isDying;

    private void Awake()
    {
        health = GetComponent<Health>();

        if (remainDuration > 0f)
        {
            remainWait = new WaitForSeconds(remainDuration);
        }
    }

    private void OnEnable()
    {
        isDying = false;
        health.Died += HandleDied;
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.Died -= HandleDied;
        }

        if (deathCoroutine != null)
        {
            StopCoroutine(deathCoroutine);
            deathCoroutine = null;
        }
    }

    private void HandleDied()
    {
        if (isDying)
        {
            return;
        }

        isDying = true;
        DisableGameplay();

        if (visualRoot == null)
        {
            gameObject.SetActive(false);
            return;
        }

        deathCoroutine = StartCoroutine(PlayDeathSequence());
    }

    private void DisableGameplay()
    {
        foreach (Behaviour behaviour in behavioursToDisable)
        {
            if (behaviour != null && behaviour != this)
            {
                behaviour.enabled = false;
            }
        }

        foreach (Collider targetCollider in collidersToDisable)
        {
            if (targetCollider != null)
            {
                targetCollider.enabled = false;
            }
        }
    }

    private IEnumerator PlayDeathSequence()
    {
        Quaternion startRotation = visualRoot.localRotation;
        Quaternion fallenRotation =
            startRotation * Quaternion.Euler(fallEulerAngles);
        Vector3 startPosition = visualRoot.localPosition;
        Vector3 fallenPosition = startPosition + fallenPositionOffset;

        if (fallDuration <= 0f)
        {
            visualRoot.localRotation = fallenRotation;
            visualRoot.localPosition = fallenPosition;
        }
        else
        {
            float elapsed = 0f;

            while (elapsed < fallDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / fallDuration);
                float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

                visualRoot.localRotation = Quaternion.Slerp(
                    startRotation,
                    fallenRotation,
                    smoothProgress);
                visualRoot.localPosition = Vector3.Lerp(
                    startPosition,
                    fallenPosition,
                    smoothProgress);

                yield return null;
            }

            visualRoot.localRotation = fallenRotation;
            visualRoot.localPosition = fallenPosition;
        }

        if (remainWait != null)
        {
            yield return remainWait;
        }

        deathCoroutine = null;
        gameObject.SetActive(false);
    }
}
