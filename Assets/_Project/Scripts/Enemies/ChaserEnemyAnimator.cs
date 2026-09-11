using UnityEngine;

[RequireComponent(typeof(Animator))]
public sealed class ChaserEnemyAnimator : MonoBehaviour
{
    private static readonly int StateParameter = Animator.StringToHash("State");
    private static readonly int AttackParameter = Animator.StringToHash("Attack");

    [SerializeField] private ChaserEnemy enemy;
    [SerializeField] private MeleeEnemyAttack meleeAttack;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (enemy == null)
        {
            enemy = GetComponentInParent<ChaserEnemy>();
        }

        if (meleeAttack == null)
        {
            meleeAttack = GetComponentInParent<MeleeEnemyAttack>();
        }

        if (enemy == null)
        {
            Debug.LogError("ChaserEnemy was not found in the parent hierarchy.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (enemy == null || animator == null)
        {
            return;
        }

        enemy.StateChanged += HandleStateChanged;
        enemy.Attacked += HandleAttacked;
        HandleStateChanged(enemy.CurrentState);
    }

    private void OnDisable()
    {
        if (enemy != null)
        {
            enemy.StateChanged -= HandleStateChanged;
            enemy.Attacked -= HandleAttacked;
        }
    }

    private void HandleStateChanged(ChaserEnemy.EnemyState state)
    {
        animator.SetInteger(StateParameter, (int)state);
    }

    private void HandleAttacked()
    {
        animator.SetTrigger(AttackParameter);
    }

    public void ApplyAttackHit()
    {
        if (meleeAttack != null)
        {
            meleeAttack.ApplyPendingHit();
        }
    }
}
