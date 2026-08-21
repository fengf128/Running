using UnityEngine;

[RequireComponent(typeof(PlayerInventory), typeof(Health), typeof(PlayerHydration))]
public sealed class PlayerItemUser : MonoBehaviour
{
    private PlayerInventory inventory;
    private Health health;
    private PlayerHydration hydration;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        health = GetComponent<Health>();
        hydration = GetComponent<PlayerHydration>();
    }

    public void TryUseSlot(int slotIndex)
    {
        if (!inventory.TryGetItem(slotIndex, out ItemData item))
        {
            return;
        }

        bool used;

        switch (item.ItemType)
        {
            case ItemType.Water:
                used = hydration.TryRestore(item.EffectAmount);
                break;

            case ItemType.Medical:
                used = health.TryHeal(item.EffectAmount);
                break;

            default:
                Debug.Log($"{item.DisplayName} cannot be used directly.", this);
                return;
        }

        if (!used)
        {
            Debug.Log($"{item.DisplayName} had no effect and was not consumed.", this);
            return;
        }

        if (inventory.TryConsume(slotIndex))
        {
            Debug.Log($"Used {item.DisplayName} from slot {slotIndex + 1}.", this);
        }
    }
}
