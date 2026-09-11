using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public sealed class DamageNumberView : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float lifetime = 0.8f;
    [SerializeField, Min(0f)] private float riseSpeed = 1f;

    private DamageNumberPool owner;
    private TextMeshPro damageText;
    private Transform cameraTransform;
    private Color initialColor;
    private Color displayColor;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private float elapsed;
    private bool isShowing;

    private void Awake()
    {
        damageText = GetComponent<TextMeshPro>();
        Camera mainCamera = Camera.main;
        cameraTransform = mainCamera != null ? mainCamera.transform : null;
        initialColor = damageText.color;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
    }

    public void Initialize(DamageNumberPool damageNumberPool)
    {
        owner = damageNumberPool;
    }

    public void Show(float damage, Vector3 worldPosition)
    {
        ShowInternal(damage, worldPosition, initialColor, false);
    }

    public void ShowHealing(float actualHeal, Vector3 worldPosition, Color healingColor)
    {
        ShowInternal(actualHeal, worldPosition, healingColor, true);
    }

    private void ShowInternal(
        float amount,
        Vector3 worldPosition,
        Color color,
        bool showPlusSign)
    {
        transform.position = worldPosition;
        transform.rotation = cameraTransform != null
            ? cameraTransform.rotation
            : initialRotation;
        transform.localScale = initialScale;

        elapsed = 0f;
        isShowing = true;
        displayColor = color;
        damageText.color = displayColor;

        if (showPlusSign)
        {
            damageText.SetText("+{0:0}", amount);
        }
        else
        {
            damageText.SetText("{0:0}", amount);
        }

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isShowing)
        {
            return;
        }

        elapsed += Time.deltaTime;
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        float duration = Mathf.Max(0.01f, lifetime);
        float progress = Mathf.Clamp01(elapsed / duration);
        Color currentColor = displayColor;
        currentColor.a = displayColor.a * (1f - progress);
        damageText.color = currentColor;

        if (progress >= 1f)
        {
            ReturnToPool();
        }
    }

    private void OnDisable()
    {
        isShowing = false;
    }

    private void ReturnToPool()
    {
        isShowing = false;

        if (owner == null)
        {
            gameObject.SetActive(false);
            return;
        }

        owner.Release(this);
    }
}
