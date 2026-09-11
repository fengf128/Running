using System.Collections.Generic;
using UnityEngine;

public sealed class DamageNumberPool : MonoBehaviour
{
    [SerializeField] private DamageNumberView damageNumberPrefab;
    [SerializeField, Min(1)] private int initialSize = 8;
    [SerializeField] private Color healingColor = new Color(0.2f, 1f, 0.2f, 1f);

    private readonly Queue<DamageNumberView> availableViews = new Queue<DamageNumberView>();

    private void Awake()
    {
        if (damageNumberPrefab == null)
        {
            Debug.LogError("Damage number prefab is not assigned.", this);
            enabled = false;
            return;
        }

        for (int i = 0; i < initialSize; i++)
        {
            availableViews.Enqueue(CreateView());
        }
    }

    public void Show(float damage, Vector3 worldPosition)
    {
        if (damageNumberPrefab == null || damage <= 0f)
        {
            return;
        }

        DamageNumberView view = availableViews.Count > 0
            ? availableViews.Dequeue()
            : CreateView();

        view.Show(damage, worldPosition);
    }

    public void ShowHealing(float actualHeal, Vector3 worldPosition)
    {
        if (damageNumberPrefab == null || actualHeal <= 0f)
        {
            return;
        }

        DamageNumberView view = availableViews.Count > 0
            ? availableViews.Dequeue()
            : CreateView();

        view.ShowHealing(actualHeal, worldPosition, healingColor);
    }

    public void Release(DamageNumberView view)
    {
        if (view == null)
        {
            return;
        }

        view.gameObject.SetActive(false);
        view.transform.SetParent(transform);
        availableViews.Enqueue(view);
    }

    private DamageNumberView CreateView()
    {
        DamageNumberView view = Instantiate(damageNumberPrefab, transform);
        view.Initialize(this);
        view.gameObject.SetActive(false);
        return view;
    }
}
