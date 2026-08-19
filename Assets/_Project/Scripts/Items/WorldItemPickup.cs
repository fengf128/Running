using UnityEngine;

public sealed class WorldItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData;

    public string InteractionName => itemData == null ? name : itemData.DisplayName;

    public void Interact(GameObject interactor)
    {
        if (itemData == null)
        {
            Debug.LogError("Item Data is not assigned.", this);
            return;
        }

        if (interactor == null)
        {
            return;
        }

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogError("Interactor does not have a PlayerInventory component.", interactor);
            return;
        }

        if (inventory.TryAdd(itemData))
        {
            gameObject.SetActive(false);
        }
    }
}
