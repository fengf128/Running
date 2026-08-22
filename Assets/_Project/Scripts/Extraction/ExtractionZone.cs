using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class ExtractionZone : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData requiredItem;
    [SerializeField] private ExtractionResultUI resultUI;
    [SerializeField] private GameMessageUI messageUI;

    private bool completed;

    public string InteractionName => "Extraction Zone";

    public void Interact(GameObject interactor)
    {
        if (completed || interactor == null)
        {
            return;
        }

        if (requiredItem == null)
        {
            Debug.LogError("Required Item is not assigned.", this);
            return;
        }

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogError("Interactor does not have a PlayerInventory component.", interactor);
            return;
        }

        if (!inventory.HasItem(requiredItem))
        {
            if (messageUI != null)
            {
                messageUI.Show($"缺少任务物品：{requiredItem.DisplayName}");
            }
            else
            {
                Debug.LogError("Game Message UI is not assigned.", this);
            }

            Debug.Log($"Extraction failed. Missing {requiredItem.DisplayName}.", this);
            return;
        }

        if (resultUI == null)
        {
            Debug.LogError("Extraction Result UI is not assigned.", this);
            return;
        }

        completed = true;
        resultUI.ShowSuccess(interactor, requiredItem.DisplayName);
        Debug.Log($"Extraction successful with {requiredItem.DisplayName}.", this);
    }

    private void Reset()
    {
        Collider zoneCollider = GetComponent<Collider>();
        zoneCollider.isTrigger = true;
    }
}
