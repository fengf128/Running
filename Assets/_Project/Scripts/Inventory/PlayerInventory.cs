using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerInventory : MonoBehaviour
{
    private const int SlotCount = 6;

    [SerializeField] private InventorySlot[] slots;

    public event Action InventoryChanged;

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
                InventoryChanged?.Invoke();
                return true;
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].TryFill(itemData))
            {
                LogPickup(itemData, i);
                InventoryChanged?.Invoke();
                return true;
            }
        }

        Debug.Log($"Inventory is full. Could not pick up {itemData.DisplayName}.", this);
        return false;
    }

    public bool TryConsume(int slotIndex)
    {
        if (slots == null || slotIndex < 0 || slotIndex >= slots.Length)
        {
            return false;
        }

        if (!slots[slotIndex].TryConsumeOne())
        {
            return false;
        }

        InventoryChanged?.Invoke();
        return true;
    }

    public bool TryGetItem(int slotIndex, out ItemData item)
    {
        item = null;

        if (slots == null || slotIndex < 0 || slotIndex >= slots.Length)
        {
            return false;
        }

        InventorySlot slot = slots[slotIndex];
        if (slot == null || slot.IsEmpty)
        {
            return false;
        }

        item = slot.Item;
        return true;
    }

    public bool HasItem(ItemData item)
    {
        if (item == null || slots == null)
        {
            return false;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];

            if (slot != null && !slot.IsEmpty && slot.Item == item)
            {
                return true;
            }
        }

        return false;
    }

    public bool RestoreFromSave(ItemData[] savedItems, int[] savedQuantities)
    {
        if (slots == null ||
            savedItems == null ||
            savedQuantities == null ||
            savedItems.Length != slots.Length ||
            savedQuantities.Length != slots.Length)
        {
            return false;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].RestoreFromSave(savedItems[i], savedQuantities[i]);
        }

        InventoryChanged?.Invoke();
        return true;
    }

    private void LogPickup(ItemData itemData, int slotIndex)
    {
        InventorySlot slot = slots[slotIndex];
        Debug.Log(
            $"Picked up {itemData.DisplayName}: slot {slotIndex + 1}, {slot.Quantity}/{itemData.MaxStack}.",
            this);
    }
}
