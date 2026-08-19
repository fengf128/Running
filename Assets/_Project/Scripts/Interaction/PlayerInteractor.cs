using UnityEngine;

public sealed class PlayerInteractor : MonoBehaviour
{
    [SerializeField, Min(0f)] private float interactionRange = 2f;
    [SerializeField] private LayerMask interactionLayers = ~0;

    private readonly Collider[] overlapResults = new Collider[16];

    public void TryInteract()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            interactionRange,
            overlapResults,
            interactionLayers,
            QueryTriggerInteraction.Collide);

        IInteractable closestInteractable = null;
        float closestSquaredDistance = float.PositiveInfinity;

        for (int i = 0; i < hitCount; i++)
        {
            Collider hitCollider = overlapResults[i];
            IInteractable interactable = hitCollider.GetComponentInParent<IInteractable>();

            if (interactable == null)
            {
                continue;
            }

            Vector3 closestPoint = hitCollider.ClosestPoint(transform.position);
            float squaredDistance = (closestPoint - transform.position).sqrMagnitude;

            if (squaredDistance >= closestSquaredDistance)
            {
                continue;
            }

            closestSquaredDistance = squaredDistance;
            closestInteractable = interactable;
        }

        closestInteractable?.Interact(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
