using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Health), typeof(PlayerHydration), typeof(PlayerInventory))]
public sealed class GameSaveSystem : MonoBehaviour
{
    [SerializeField] private ItemData[] knownItems;
    [SerializeField] private string saveFileName = "player-save.json";

    private readonly Dictionary<string, ItemData> itemsById =
        new Dictionary<string, ItemData>();

    private Health health;
    private PlayerHydration hydration;
    private PlayerInventory inventory;

    private string SaveDirectory => Path.GetFullPath(
        Path.Combine(Application.dataPath, "..", "SaveData"));

    public string SavePath => Path.Combine(SaveDirectory, saveFileName);

    private void Awake()
    {
        health = GetComponent<Health>();
        hydration = GetComponent<PlayerHydration>();
        inventory = GetComponent<PlayerInventory>();
        BuildItemIndex();
    }

    private void BuildItemIndex()
    {
        if (knownItems == null)
        {
            return;
        }

        foreach (ItemData item in knownItems)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.ItemId))
            {
                Debug.LogError("Known Items contains an empty item or item ID.", this);
                continue;
            }

            if (itemsById.ContainsKey(item.ItemId))
            {
                Debug.LogError($"Duplicate item ID in Known Items: {item.ItemId}", this);
                continue;
            }

            itemsById.Add(item.ItemId, item);
        }
    }

    public bool SaveGame()
    {
        if (health.IsDead)
        {
            Debug.LogWarning("A dead player cannot be saved.", this);
            return false;
        }

        GameSaveData saveData = CreateSaveData();
        string json = JsonUtility.ToJson(saveData, true);

        try
        {
            Directory.CreateDirectory(SaveDirectory);
            File.WriteAllText(SavePath, json);
            Debug.Log($"Game saved: {SavePath}", this);
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed to save game: {exception.Message}", this);
            return false;
        }
    }

    public bool LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning($"Save file does not exist: {SavePath}", this);
            return false;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

            if (!TryApplySaveData(saveData))
            {
                Debug.LogError("Save data is invalid and was not loaded.", this);
                return false;
            }

            Debug.Log($"Game loaded: {SavePath}", this);
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed to load game: {exception.Message}", this);
            return false;
        }
    }

    private GameSaveData CreateSaveData()
    {
        GameSaveData saveData = new GameSaveData
        {
            health = health.CurrentHealth,
            hydration = hydration.CurrentHydration,
            slots = new InventorySlotSaveData[inventory.Slots.Count]
        };

        for (int i = 0; i < inventory.Slots.Count; i++)
        {
            InventorySlot slot = inventory.Slots[i];
            InventorySlotSaveData slotData = new InventorySlotSaveData();

            if (slot != null && !slot.IsEmpty)
            {
                slotData.itemId = slot.Item.ItemId;
                slotData.quantity = slot.Quantity;
            }
            else
            {
                slotData.itemId = string.Empty;
                slotData.quantity = 0;
            }

            saveData.slots[i] = slotData;
        }

        return saveData;
    }

    private bool TryApplySaveData(GameSaveData saveData)
    {
        if (saveData == null ||
            saveData.slots == null ||
            saveData.slots.Length != inventory.Slots.Count)
        {
            return false;
        }

        ItemData[] savedItems = new ItemData[saveData.slots.Length];
        int[] savedQuantities = new int[saveData.slots.Length];

        for (int i = 0; i < saveData.slots.Length; i++)
        {
            InventorySlotSaveData slotData = saveData.slots[i];
            if (slotData == null || string.IsNullOrWhiteSpace(slotData.itemId))
            {
                continue;
            }

            ItemData item = FindItem(slotData.itemId);
            if (item == null)
            {
                Debug.LogWarning($"Unknown item ID in save: {slotData.itemId}", this);
                continue;
            }

            savedItems[i] = item;
            savedQuantities[i] = slotData.quantity;
        }

        if (!inventory.RestoreFromSave(savedItems, savedQuantities))
        {
            return false;
        }

        health.RestoreFromSave(saveData.health);
        hydration.RestoreFromSave(saveData.hydration);
        return true;
    }

    private ItemData FindItem(string itemId)
    {
        itemsById.TryGetValue(itemId, out ItemData item);
        return item;
    }
}
