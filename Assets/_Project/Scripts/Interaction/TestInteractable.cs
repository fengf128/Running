using UnityEngine;

public sealed class TestInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionName = "测试补给箱";

    public string InteractionName => interactionName;

    public void Interact(GameObject interactor)
    {
        Debug.Log($"{interactor.name} interacted with {InteractionName}.", this);
    }
}
