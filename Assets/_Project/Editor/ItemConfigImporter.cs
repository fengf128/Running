using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class ItemConfigImporter
{
    private const string CsvPath = "Assets/_Project/Data/Config/Items.csv";
    private const string ItemFolder = "Assets/_Project/Data/Items";

    [MenuItem("Tools/Evacuation Test/Import Item Config CSV")]
    public static void Import()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogWarning("Stop Play Mode before importing item configuration.");
            return;
        }

        List<ItemConfigRow> rows;
        var targets = new List<ItemData>();
        try
        {
            // Validate the entire input and resolve every ID before changing any asset.
            rows = ItemConfigCsv.Parse(File.ReadAllText(CsvPath, Encoding.UTF8));
            var itemsById = new Dictionary<string, ItemData>(StringComparer.Ordinal);
            foreach (string guid in AssetDatabase.FindAssets("t:ItemData", new[] { ItemFolder }))
            {
                var item = AssetDatabase.LoadAssetAtPath<ItemData>(AssetDatabase.GUIDToAssetPath(guid));
                if (string.IsNullOrWhiteSpace(item.ItemId)) continue;
                if (itemsById.ContainsKey(item.ItemId))
                    throw new FormatException($"Duplicate asset itemId: {item.ItemId}");
                itemsById.Add(item.ItemId, item);
            }
            foreach (ItemConfigRow row in rows)
            {
                if (!itemsById.TryGetValue(row.ItemId, out ItemData item))
                    throw new FormatException($"Unknown itemId '{row.ItemId}'. This importer only updates existing assets.");
                targets.Add(item);
            }
        }
        catch (Exception exception)
        {
            Debug.LogError($"Item config import cancelled; no assets changed. {exception.Message}");
            return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Import Item Config CSV");
        for (int i = 0; i < rows.Count; i++)
        {
            ItemConfigRow row = rows[i];
            ItemData item = targets[i];
            Undo.RecordObject(item, "Import Item Config CSV");
            var serialized = new SerializedObject(item);
            serialized.FindProperty("displayName").stringValue = row.DisplayName;
            serialized.FindProperty("itemType").intValue = (int)row.ItemType;
            serialized.FindProperty("maxStack").intValue = row.MaxStack;
            serialized.FindProperty("effectAmount").floatValue = row.EffectAmount;
            serialized.ApplyModifiedProperties();
            // Preserve asset identity, its ID, and all unrelated assets.
            AssetDatabase.SaveAssetIfDirty(item);
        }
        Undo.CollapseUndoOperations(undoGroup);
        Debug.Log($"Imported {rows.Count} item configurations from {CsvPath}. Existing asset references preserved.");
    }
}
