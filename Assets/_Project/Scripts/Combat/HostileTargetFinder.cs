using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health), typeof(FactionMember))]
public sealed class HostileTargetFinder : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayers = ~0;

    private readonly Collider[] overlapResults = new Collider[32];
    private Health ownerHealth;
    private FactionMember ownerFaction;

    private void Awake()
    {
        ownerHealth = GetComponent<Health>();
        ownerFaction = GetComponent<FactionMember>();
    }

    public Health FindNearestHostile(float range)
    {
        if (ownerFaction == null || range <= 0f)
        {
            return null;
        }

        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            range,
            overlapResults,
            targetLayers,
            QueryTriggerInteraction.Collide);

        Health nearestHealth = null;
        float nearestSquaredDistance = float.PositiveInfinity;

        for (int i = 0; i < hitCount; i++)
        {
            Collider candidateCollider = overlapResults[i];
            if (candidateCollider == null)
            {
                continue;
            }

            Health candidateHealth =
                candidateCollider.GetComponentInParent<Health>();

            if (candidateHealth == null || candidateHealth == ownerHealth ||
                candidateHealth.IsDead)
            {
                continue;
            }

            FactionMember candidateFaction =
                candidateHealth.GetComponent<FactionMember>();

            if (!ownerFaction.IsHostileTo(candidateFaction))
            {
                continue;
            }

            Vector3 closestPoint =
                candidateCollider.ClosestPoint(transform.position);
            float squaredDistance =
                (closestPoint - transform.position).sqrMagnitude;

            if (squaredDistance >= nearestSquaredDistance)
            {
                continue;
            }

            nearestHealth = candidateHealth;
            nearestSquaredDistance = squaredDistance;
        }

        return nearestHealth;
    }
}
