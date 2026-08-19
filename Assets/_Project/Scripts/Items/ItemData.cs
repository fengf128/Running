using UnityEngine;

public enum ItemType
{
    Water,
    Medical,
    Quest,
    Other
}

[CreateAssetMenu(fileName = "Item_", menuName = "Evacuation Test/Item Data")]
public sealed class ItemData : ScriptableObject
{
    [SerializeField] private string itemId;
    [SerializeField] private string displayName;
    [SerializeField] private ItemType itemType = ItemType.Other;
    [SerializeField, Min(1)] private int maxStack = 1;
    [SerializeField, Min(0f)] private float effectAmount;

    public string ItemId => itemId;
    public string DisplayName => displayName;
    public ItemType ItemType => itemType;
    public int MaxStack => maxStack;
    public float EffectAmount => effectAmount;
}
