using UnityEngine;

public interface IInteractable
{
    string InteractionName { get; }

    void Interact(GameObject interactor);
}
