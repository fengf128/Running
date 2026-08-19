using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Health))]
public sealed class ChaserEnemy : MonoBehaviour
{
    [SerializeField, Min(0f)] private float detectionRange = 12f;
    [SerializeField, Min(0f)] private float moveSpeed = 2.5f;
    [SerializeField, Min(0f)] private float stoppingDistance = 1.3f;
    [SerializeField, Min(0f)] private float attackDamage = 10f;
    [SerializeField, Min(0f)] private float attackCooldown = 1f;

    private CharacterController characterController;
    private Transform target;
    private Health targetHealth;
    private float nextAttackTime;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player with Player tag was not found.", this);
            return;
        }

        target = player.transform;
        targetHealth = player.GetComponent<Health>();
        if (targetHealth == null)
        {
            Debug.LogError("Player Health component was not found.", this);
        }
    }

    private void Update()
    {
        if (target == null || targetHealth == null || targetHealth.IsDead)
        {
            return;
        }

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        float squaredDistance = toTarget.sqrMagnitude;

        if (squaredDistance > detectionRange * detectionRange)
        {
            return;
        }

        if (squaredDistance > stoppingDistance * stoppingDistance)
        {
            Vector3 direction = toTarget.normalized;
            Vector3 velocity = direction * moveSpeed + Vector3.down * 2f;
            characterController.Move(velocity * Time.deltaTime);
            transform.forward = direction;
            return;
        }

        TryAttack();
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime)
        {
            return;
        }

        targetHealth.TakeDamage(attackDamage);
        nextAttackTime = Time.time + attackCooldown;
    }
}
