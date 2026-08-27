using UnityEngine;

[RequireComponent(typeof(Health))]
public sealed class RangedEnemy : MonoBehaviour
{
    [SerializeField, Min(0f)] private float attackRange = 12f;

    private IEnemyAttack attackStrategy;
    private Transform target;

    private void Awake()
    {
        attackStrategy = GetComponent<IEnemyAttack>();
        if (attackStrategy == null)
        {
            Debug.LogError("Enemy attack component was not found.", this);
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player with Player tag was not found.", this);
            return;
        }

        target = player.transform;
    }

    private void Update()
    {
        if (target == null || attackStrategy == null)
        {
            return;
        }

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude > attackRange * attackRange)
        {
            return;
        }

        if (toTarget.sqrMagnitude < 0.001f)
        {
            return;
        }

        transform.forward = toTarget.normalized;
        attackStrategy.TryAttack(target);
    }
}
