using UnityEngine;

[System.Serializable]
public sealed class InventorySlot
{
    [SerializeField] private ItemData item;
    [SerializeField, Min(0)] private int quantity;

    public ItemData Item => item;
    public int Quantity => quantity;
    public bool IsEmpty => item == null || quantity <= 0;

    public bool TryStack(ItemData itemToAdd)
    {
        if (IsEmpty || item != itemToAdd || quantity >= item.MaxStack)
        {
            return false;
        }

        quantity++;
        return true;
    }

    public bool TryFill(ItemData itemToAdd)
    {
        if (!IsEmpty || itemToAdd == null)
        {
            return false;
        }

        item = itemToAdd;
        quantity = 1;
        return true;
    }

    public bool TryConsumeOne()
    {
        if (IsEmpty)
        {
            return false;
        }

        quantity--;

        if (quantity == 0)
        {
            item = null;
        }

        return true;
    }
}
