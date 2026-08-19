using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerInventory : MonoBehaviour
{
    private const int SlotCount = 6;

    [SerializeField] private InventorySlot[] slots;

    public IReadOnlyList<InventorySlot> Slots => slots;

    private void Awake()
    {
        slots = new InventorySlot[SlotCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new InventorySlot();
        }
    }

    public bool TryAdd(ItemData itemData)
    {
        if (itemData == null)
        {
            return false;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].TryStack(itemData))
            {
                LogPickup(itemData, i);
                return true;
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].TryFill(itemData))
            {
                LogPickup(itemData, i);
                return true;
            }
        }

        Debug.Log($"Inventory is full. Could not pick up {itemData.DisplayName}.", this);
        return false;
    }

    private void LogPickup(ItemData itemData, int slotIndex)
    {
        InventorySlot slot = slots[slotIndex];
        Debug.Log(
            $"Picked up {itemData.DisplayName}: slot {slotIndex + 1}, {slot.Quantity}/{itemData.MaxStack}.",
            this);
    }
}
