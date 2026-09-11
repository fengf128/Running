using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack_", menuName = "Evacuation Test/Melee Attack Data")]
public sealed class MeleeAttackData : ScriptableObject
{
    [SerializeField, Min(0f)] private float damage = 10f;
    [SerializeField, Min(0f)] private float cooldown = 1f;

    public float Damage => damage;
    public float Cooldown => cooldown;
}
