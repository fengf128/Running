using UnityEngine;

public enum CombatFaction
{
    Neutral,
    Player,
    Enemy
}

[DisallowMultipleComponent]
public sealed class FactionMember : MonoBehaviour
{
    [SerializeField] private CombatFaction faction = CombatFaction.Neutral;

    public CombatFaction Faction => faction;

    public bool IsHostileTo(FactionMember other)
    {
        if (other == null ||
            faction == CombatFaction.Neutral ||
            other.faction == CombatFaction.Neutral)
        {
            return false;
        }

        return faction != other.faction;
    }
}
