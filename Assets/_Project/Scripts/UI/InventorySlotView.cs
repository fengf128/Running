using TMPro;
using UnityEngine;

public sealed class InventorySlotView : MonoBehaviour
{
    [SerializeField] private TMP_Text contentText;

    public void Refresh(int slotNumber, InventorySlot slot)
    {
        if (contentText == null)
        {
            return;
        }

        if (slot == null || slot.IsEmpty)
        {
            contentText.text = $"{slotNumber}. Empty";
            return;
        }

        contentText.text =
            $"{slotNumber}. {slot.Item.DisplayName} {slot.Quantity}/{slot.Item.MaxStack}";
    }
}
