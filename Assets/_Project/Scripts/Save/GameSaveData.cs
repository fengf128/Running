using System;

[Serializable]
public sealed class GameSaveData
{
    public float health;
    public float hydration;
    public InventorySlotSaveData[] slots;
}

[Serializable]
public sealed class InventorySlotSaveData
{
    public string itemId;
    public int quantity;
}
