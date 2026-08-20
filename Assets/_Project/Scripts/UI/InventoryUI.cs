using System.Collections.Generic;
using UnityEngine;

public sealed class InventoryUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private InventorySlotView[] slotViews;

    private void OnEnable()
    {
        if (inventory != null)
        {
            inventory.InventoryChanged += Refresh;
        }

        Refresh();
    }

    private void Start()
    {
        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.InventoryChanged -= Refresh;
        }
    }

    private void Refresh()
    {
        if (inventory == null || slotViews == null)
        {
            return;
        }

        IReadOnlyList<InventorySlot> slots = inventory.Slots;
        if (slots == null)
        {
            return;
        }

        for (int i = 0; i < slotViews.Length; i++)
        {
            InventorySlot slot = i < slots.Count ? slots[i] : null;
            slotViews[i]?.Refresh(i + 1, slot);
        }
    }
}
